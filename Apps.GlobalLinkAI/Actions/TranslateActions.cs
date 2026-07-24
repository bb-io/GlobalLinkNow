using Apps.GlobalLinkAI.Constants;
using Apps.GlobalLinkAI.Extensions;
using Apps.GlobalLinkAI.Invocables;
using Apps.GlobalLinkAI.Models.Entities;
using Apps.GlobalLinkAI.Models.Request.Translation;
using Apps.GlobalLinkAI.Models.Response.Translation;
using Apps.GlobalLinkAI.Services.Translation;
using Apps.GlobalLinkAI.Services.Translation.Models;
using Blackbird.Applications.SDK.Blueprints;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using RestSharp;

namespace Apps.GlobalLinkAI.Actions;

[ActionList("Translate")]
public class TranslateActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : AppInvocable(invocationContext)
{
    [BlueprintActionDefinition(BlueprintAction.TranslateText)]
    [Action("Translate text", Description = "Translate the provided text.")]
    public async Task<TranslateTextResponse> Translate([ActionParameter] TranslateTextRequest input)
    {
        string sourceLanguage = string.IsNullOrEmpty(input.SourceLanguage) ? "auto" : input.SourceLanguage;
        var request = new RestRequest("/apigateway/texttranslator", Method.Post)
            .AddQueryParameter("to", input.TargetLanguage)
            .AddNullableQueryParameter("from", sourceLanguage)
            .AddNullableQueryParameter("textType", "text")
            .AddNullableQueryParameter("domain", input.Domain)
            .AddNullableQueryParameter("engineId", input.EngineId)
            .WithJsonBody(new TextTranslationEntity[] { new() { Text = input.Text } });
        
        var response = await Client.ExecuteWithErrorHandling<TextTranslationResponse>(request);
        return new(string.Join(" ", response.Message.Select(x => x.Text)));
    }

    [BlueprintActionDefinition(BlueprintAction.TranslateFile)]
    [Action("Translate", Description = "Translate file content retrieved from a CMS or file storage.")]
    public async Task<TranslateResponse> TranslateDocument([ActionParameter] TranslateDocumentRequest input)
    {
        await using var fileStream = await fileManagementClient.DownloadAsync(input.File);

        var translateInput = new TranslationStrategyRequest(
            fileStream, 
            input.File.Name, 
            input.File.ContentType,
            input.OutputFileHandling ?? ProcessFileFormat.InteroperableXliff,
            input.SourceLanguage, 
            input.TargetLanguage,
            input.Ocr,
            input.Domain,
            input.EngineId);
        
        var strategy = FileTranslationStrategyFactory.Create(input.FileTranslationStrategy, InvocationContext);
        var result = await strategy.Translate(translateInput);

        await using var outputStream = result.Stream;
        var outputFile = await fileManagementClient.UploadAsync(outputStream, result.MediaType, result.FileName);
        return new(outputFile, result.Errors);
    }
}