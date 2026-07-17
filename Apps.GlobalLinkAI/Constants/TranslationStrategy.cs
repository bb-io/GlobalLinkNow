namespace Apps.GlobalLinkAI.Constants;

public static class TranslationStrategy
{
    public const string BlackbirdInteroperable = "Blackbird interoperable (default)";
    public const string GlobalLinkNative = "GlobalLink native";

    public static readonly string[] SupportedStrategies = [BlackbirdInteroperable, GlobalLinkNative];
}