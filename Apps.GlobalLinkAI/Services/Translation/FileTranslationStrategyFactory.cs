using Apps.GlobalLinkAI.Constants;
using Apps.GlobalLinkAI.Services.Translation.Concrete;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.GlobalLinkAI.Services.Translation;

public static class FileTranslationStrategyFactory
{
    public static IFileTranslationStrategy Create(string? strategy, InvocationContext invocationContext)
    {
        return strategy switch
        {
            TranslationStrategy.BlackbirdInteroperable or null => new BlackbirdFileTranslationStrategy(invocationContext),
            TranslationStrategy.GlobalLinkNative => new NativeFileTranslationStrategy(invocationContext),
            _ => throw new PluginMisconfigurationException($"Unknown translation strategy: {strategy}")
        };
    }
}