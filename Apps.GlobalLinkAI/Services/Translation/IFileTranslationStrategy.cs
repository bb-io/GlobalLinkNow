using Apps.GlobalLinkAI.Services.Translation.Models;

namespace Apps.GlobalLinkAI.Services.Translation;

public interface IFileTranslationStrategy
{
    Task<TranslatedFileResult> Translate(TranslationStrategyRequest translateInput);
}