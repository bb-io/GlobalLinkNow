using Blackbird.Applications.SDK.Blueprints.Interfaces.Translate;
using Blackbird.Applications.Sdk.Common;

namespace Apps.GlobalLinkAI.Models.Response.Translation;

public record TranslateTextResponse(string TranslatedText) : ITranslateTextOutput
{
    [Display("Translated text")] 
    public string TranslatedText { get; set; } = TranslatedText;
}