namespace Apps.GlobalLinkAI.Services.Translation.Models;

public record TranslatedFileResult(Stream Stream, string MediaType, string FileName, string[] Errors);