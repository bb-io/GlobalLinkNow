using Newtonsoft.Json;

namespace Apps.GlobalLinkAI.Models.Response.Translation;

public class TextTranslationResponse
{
    [JsonProperty("message")]
    public List<TranslationMessageResponse> Message { get; set; } = [];
}