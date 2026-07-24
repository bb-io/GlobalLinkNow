using Newtonsoft.Json;

namespace Apps.GlobalLinkAI.Models.Response.Translation;

public class TranslationMessageResponse
{
    [JsonProperty("Text")]
    public string Text { get; set; } = string.Empty;

    [JsonProperty("detectedLanguage")]
    public string? DetectedLanguage { get; set; }

    [JsonProperty("words")]
    public int Words { get; set; }
}