using Apps.GlobalLinkAI.Api;
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
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using RestSharp;

namespace Apps.GlobalLinkAI.Actions;

[ActionList("Translate")]
public class TranslateActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : AppInvocable(invocationContext)
{
    [Action("Translate text", Description = "Translate text")]
    public async Task<TextTranslationEntity> Translate([ActionParameter] TranslateTextRequest input,
        [ActionParameter] [Display("Text")] string text)
    {
        input.From ??= "auto";
        var endpoint = "/apigateway/texttranslator".WithQuery(input);

        var request = new AppRequest(endpoint, Method.Post, Creds).WithJsonBody(new TextTranslationEntity[]
        {
            new()
            {
                Text = text
            }
        });
        var response = await Client.ExecuteWithErrorHandling<TextTranslationResponse>(request);

        return new()
        {
            Text = string.Join(" ", response.Message.Select(x => x.Text))
        };
    }

    [BlueprintActionDefinition(BlueprintAction.TranslateFile)]
    [Action("Translate", Description = "Translate a specific document")]
    public async Task<TranslateResponse> TranslateDocument([ActionParameter] TranslateDocumentRequest input)
    {
        await using var fileStream = await fileManagementClient.DownloadAsync(input.File);

        var translateInput = new TranslationStrategyRequest(
            fileStream, 
            input.File.Name, 
            input.File.ContentType,
            input.SourceLanguage, 
            input.TargetLanguage,
            input.Ocr,
            input.Domain,
            input.EngineId);
        
        var strategy = FileTranslationStrategyFactory.Create(input.FileTranslationStrategy, invocationContext);
        var result = await strategy.Translate(translateInput);

        var outputFile = await fileManagementClient.UploadAsync(result.Stream, result.MediaType, result.FileName);
        return new(outputFile);
    }
}