using Apps.GlobalLinkAI.Extensions;
using Apps.GlobalLinkAI.Invocables;
using Apps.GlobalLinkAI.Models.Entities;
using Apps.GlobalLinkAI.Models.Response.Translation;
using Apps.GlobalLinkAI.Services.Translation.Models;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using Blackbird.Filters.Constants;
using Blackbird.Filters.Enums;
using Blackbird.Filters.Extensions;
using Blackbird.Filters.Transformations;
using RestSharp;

namespace Apps.GlobalLinkAI.Services.Translation.Concrete;

public class BlackbirdFileTranslationStrategy(InvocationContext context) : AppInvocable(context), IFileTranslationStrategy
{
    public async Task<TranslatedFileResult> Translate(TranslationStrategyRequest translateInput)
    {
        var loadResult = Transformation.Load(translateInput.InputFileStream, translateInput.InputFileName, translateInput.InputFileContentType);
        if (!loadResult.Success)
            throw new PluginMisconfigurationException(loadResult.Error);
        
        var content = loadResult.Value;
        content.SourceLanguage ??= translateInput.SourceLanguage;
        content.TargetLanguage ??= translateInput.TargetLanguage;

        string sourceLanguage = translateInput.SourceLanguage ?? "auto";
        var sourceLanguages = new HashSet<string>();
        
        var errors = new List<string>();
        
        var units = content.GetUnits().ToList();
        var translatableUnits = units.Where(x => x.IsInitial).ToList();
        
        int batchCounter = 0;
        var processedBatches = await translatableUnits.Batch(50).Process(BatchTranslate);
        
        foreach (var (unit, results) in processedBatches)
        {
            int words = 0;
            foreach (var (segment, result) in results)
            {
                if (result is null || string.IsNullOrEmpty(result.Text))
                    continue;

                var shouldTranslate = segment.State is null || segment.State == SegmentState.Initial;
                if (!shouldTranslate)
                    continue;
                
                segment.SetTarget(result.Text);

                segment.State = SegmentState.Translated;
                words += result.Words;

                if (!string.IsNullOrEmpty(result.DetectedLanguage))
                    sourceLanguages.Add(result.DetectedLanguage.ToLower());
            }

            unit.Provenance.Translation.Tool = "GlobalLink NOW";
            unit.AddUsage("GlobalLink NOW", words, UsageUnit.Words);
        }

        if (content.SourceLanguage is null && sourceLanguages.Count == 1)
            content.SourceLanguage = sourceLanguages.Single();

        return new TranslatedFileResult(content.Serialize().ToStream(), MediaTypes.Xliff2, content.BilingualFileName, errors.ToArray());
        
        async Task<IEnumerable<TranslationMessageResponse?>> BatchTranslate(IEnumerable<(Unit Unit, Segment Segment)> batch)
        {
            batchCounter++;
            
            var batchList = batch.ToList();
            var sources = batchList.Select(x => x.Segment.GetSource()).ToArray();
            
            var request = new RestRequest("/apigateway/texttranslator", Method.Post)
                .AddQueryParameter("to", translateInput.TargetLanguage)
                .AddQueryParameter("from", sourceLanguage)
                .AddQueryParameter("textType", "text")
                .AddNullableQueryParameter("domain", translateInput.Domain)
                .AddNullableQueryParameter("engineId", translateInput.EngineId)
                .WithJsonBody(sources.Select(text => new TextTranslationEntity { Text = text }).ToArray());
            
            var response = await Client.ExecuteWithErrorHandling<TextTranslationResponse>(request);
            var messages = response.Message.ToArray();

            if (messages.Length == sources.Length) 
                return messages;
            
            errors.Add(
                $"The response from GlobalLink (batch {batchCounter}) was incomplete. " +
                $"Got {messages.Length} results, expected {sources.Length}");
            return sources.Select((_, i) => i < messages.Length ? messages[i] : null);
        }
    }
}