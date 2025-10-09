using Apps.GlobalLinkAI.Api;
using Apps.GlobalLinkAI.Invocables;
using Apps.GlobalLinkAI.Models.Entities;
using Apps.GlobalLinkAI.Models.Request;
using Apps.GlobalLinkAI.Models.Request.Translation;
using Apps.GlobalLinkAI.Models.Response;
using Apps.GlobalLinkAI.Models.Response.Translation;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using RestSharp;

namespace Apps.GlobalLinkAI.Actions;

[ActionList("Translate")]
public class TranslateActions : AppInvocable
{
    private readonly IFileManagementClient _fileManagementClient;

    public TranslateActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : base(
        invocationContext)
    {
        _fileManagementClient = fileManagementClient;
    }

    [Action("Translate", Description = "Translate text")]
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

    [Action("Translate document", Description = "Translate a specific document")]
    public async Task<FileResponse> TranslateDocument([ActionParameter] TranslateDocumentRequest input,
        [ActionParameter] FileRequest file)
    {
        input.From ??= "auto";
        var endpoint = "/uploadapigateway/upload/documentTranslate".WithQuery(input);

        var fileStream = await _fileManagementClient.DownloadAsync(file.File);
        var request = new AppRequest(endpoint, Method.Post, Creds)
            .AddFile("filename", () => fileStream, file.File.Name);

        var response = await Client.ExecuteWithErrorHandling<TranslateDocumentResponse>(request);
        var fileId = response.FileIds.First().FileId;

        var translationStatus = string.Empty;
        while (translationStatus != "Translated")
        {
            await Task.Delay(1000);

            request = new AppRequest($"apigateway/storage/info/file/{fileId}", Method.Get, Creds);
            var fileResponse = await Client.ExecuteWithErrorHandling<TranslateFileResponse>(request);

            translationStatus = fileResponse.Status;

            if (translationStatus == "Error")
                throw new(fileResponse.ErrorCode);
        }

        request = new AppRequest($"apigateway/storage/{fileId}?fileType=translated_file", Method.Get, Creds);
        var fileContentResponse = await Client.ExecuteWithErrorHandling(request);

        return new()
        {
            File = await _fileManagementClient.UploadAsync(new MemoryStream(fileContentResponse.RawBytes),
                file.File.ContentType, file.File.Name)
        };
    }
}