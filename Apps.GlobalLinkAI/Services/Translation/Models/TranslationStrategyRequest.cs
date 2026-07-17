using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.GlobalLinkAI.Services.Translation.Models;

public record TranslationStrategyRequest(
    Stream InputFileStream,
    string InputFileName,
    string InputFileContentType,
    string? SourceLanguage, 
    string TargetLanguage,
    bool? Ocr,
    string? Domain,
    string? EngineId);