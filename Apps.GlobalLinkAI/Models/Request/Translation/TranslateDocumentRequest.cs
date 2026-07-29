using Apps.GlobalLinkAI.DataSourceHandlers;
using Apps.GlobalLinkAI.DataSourceHandlers.StaticDataSourceHandlers;
using Blackbird.Applications.SDK.Blueprints.Handlers;
using Blackbird.Applications.SDK.Blueprints.Interfaces.Translate;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.GlobalLinkAI.Models.Request.Translation;

public class TranslateDocumentRequest : ITranslateFileInput
{
    [Display("Content")] 
    public FileReference File { get; set; } = null!;

    [Display("Target language"), DataSource(typeof(LanguageDataSourceHandler))]
    public string TargetLanguage { get; set; } = string.Empty;
    
    [Display("Source language"), DataSource(typeof(LanguageDataSourceHandler))]
    public string? SourceLanguage { get; set; }
    
    [Display("OCR")]
    public bool? Ocr { get; set; }

    [Display("Domain")]
    public string? Domain { get; set; }
    
    [Display("Engine ID"), DataSource(typeof(EngineDataSourceHandler))]
    public string? EngineId { get; set; }
    
    [Display("Output file handling"), StaticDataSource(typeof(ProcessFileFormatHandler))]
    public string? OutputFileHandling { get; set; }

    [Display("File translation strategy"), StaticDataSource(typeof(TranslationStrategyDataSourceHandler))]
    public string? FileTranslationStrategy { get; set; }
}