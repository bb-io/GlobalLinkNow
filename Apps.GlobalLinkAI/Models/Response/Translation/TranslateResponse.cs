using Blackbird.Applications.SDK.Blueprints.Interfaces.Translate;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.GlobalLinkAI.Models.Response.Translation;

public record TranslateResponse(FileReference File, string[] Errors) : ITranslateFileOutput
{
    [Display("File")] 
    public FileReference File { get; set; } = File;

    [Display("Errors")] 
    public string[] Errors { get; set; } = Errors;
}