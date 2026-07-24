using Apps.GlobalLinkAI.DataSourceHandlers;
using Blackbird.Applications.SDK.Blueprints.Interfaces.Translate;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.GlobalLinkAI.Models.Request.Translation;

public class TranslateTextRequest : ITranslateTextInput
{
    [Display("Text")] 
    public string Text { get; set; } = string.Empty;
    
    [Display("Target language"), DataSource(typeof(LanguageDataSourceHandler))]
    public string TargetLanguage { get; set; } = string.Empty;
    
    [Display("Source language"), DataSource(typeof(LanguageDataSourceHandler))]
    public string? SourceLanguage { get; set; }
    
    [Display("Domain")]
    public string? Domain { get; set; }
    
    [Display("Engine ID"), DataSource(typeof(EngineDataSourceHandler))]
    public string? EngineId { get; set; }
}