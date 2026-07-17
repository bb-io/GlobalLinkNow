using Apps.GlobalLinkAI.DataSourceHandlers;
using Blackbird.Applications.SDK.Blueprints.Interfaces.Translate;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Newtonsoft.Json;

namespace Apps.GlobalLinkAI.Models.Request.Translation;

public class TranslateTextRequest : ITranslateTextInput
{
    [Display("Text")] 
    public string Text { get; set; } = string.Empty;
    
    [Display("Target language")]
    [JsonProperty("to")]
    [DataSource(typeof(LanguageDataSourceHandler))]
    public string TargetLanguage { get; set; } = string.Empty;
    
    [Display("Source language")]
    [JsonProperty("from")]
    [DataSource(typeof(LanguageDataSourceHandler))]
    public string? From { get; set; }
    
    [JsonProperty("domain")]
    public string? Domain { get; set; }
    
    [Display("Engine ID")]
    [JsonProperty("engineId")]
    [DataSource(typeof(EngineDataSourceHandler))]
    public string? EngineId { get; set; }
}