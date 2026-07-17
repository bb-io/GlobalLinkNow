using Apps.GlobalLinkAI.Extensions;
using Apps.GlobalLinkAI.Invocables;
using Apps.GlobalLinkAI.Models.Response.Translation;
using Apps.GlobalLinkAI.Services.Translation.Models;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.GlobalLinkAI.Services.Translation.Concrete;

public class NativeFileTranslationStrategy(InvocationContext context) : AppInvocable(context), IFileTranslationStrategy
{
    public async Task<TranslatedFileResult> Translate(TranslationStrategyRequest translateInput)
    {
        string sourceLanguage = string.IsNullOrEmpty(translateInput.SourceLanguage)
            ? "auto"
            : translateInput.SourceLanguage;
        
        var request = new RestRequest("/uploadapigateway/upload/documentTranslate", Method.Post)
            .AddQueryParameter("to", translateInput.TargetLanguage)
            .AddQueryParameter("from", sourceLanguage)
            .AddNullableQueryParameter("ocr", translateInput.Ocr)
            .AddNullableQueryParameter("domain", translateInput.Domain)
            .AddNullableQueryParameter("engineId", translateInput.EngineId)
            .AddFile("filename", () => translateInput.InputFileStream, translateInput.InputFileName);

        var response = await Client.ExecuteWithErrorHandling<TranslateDocumentResponse>(request);
        string fileId = response.FileIds.First().FileId;

        string translationStatus = string.Empty;
        while (translationStatus != "Translated")
        {
            await Task.Delay(1000);

            request = new RestRequest($"apigateway/storage/info/file/{fileId}");
            var fileResponse = await Client.ExecuteWithErrorHandling<TranslateFileResponse>(request);

            translationStatus = fileResponse.Status;
            if (translationStatus == "Error")
                throw new PluginApplicationException($"Translation error occured. Error code: {fileResponse.ErrorCode}");
        }

        request = new RestRequest($"apigateway/storage/{fileId}?fileType=translated_file");
        var fileContentResponse = await Client.ExecuteWithErrorHandling(request);
        if (fileContentResponse.RawBytes == null)
            throw new PluginApplicationException("No content returned by GlobalLink");
        
        var outputStream = new MemoryStream(fileContentResponse.RawBytes);
        return new(outputStream, translateInput.InputFileContentType, translateInput.InputFileName, []);
    }
}