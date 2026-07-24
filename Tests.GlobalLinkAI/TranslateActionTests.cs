using Apps.GlobalLinkAI.Actions;
using Apps.GlobalLinkAI.Constants;
using Apps.GlobalLinkAI.Models.Request.Translation;
using Blackbird.Applications.Sdk.Common.Files;
using Tests.GlobalLinkAI.Base;

namespace Tests.GlobalLinkAI;

[TestClass]
public class TranslateActionTests : TestBase
{
    private readonly TranslateActions _actions;

    public TranslateActionTests() => _actions = new TranslateActions(InvocationContext, FileManager);
    
    [TestMethod]
    public async Task TranslateText_ReturnsTranslatedText()
    {
        // Arrange
        var input = new TranslateTextRequest
        {
            Text = "Hello world!",
            TargetLanguage = "uk"
        };

        // Act
        var result = await _actions.Translate(input);

        // Assert
        Console.WriteLine(result.TranslatedText);
        Assert.IsNotEmpty(result.TranslatedText);
    }

    [TestMethod]
    public async Task TranslateContent_IsSuccess()
    {
        // Arrange
        var input = new TranslateDocumentRequest
        {
            File = new FileReference { Name = "test.html" },
            TargetLanguage = "hu",
            OutputFileHandling = ProcessFileFormat.Original,
            FileTranslationStrategy = TranslationStrategy.GlobalLinkNative
        };

        // Act
        var result = await _actions.TranslateDocument(input);

        // Assert
        PrintJsonResult(result);
        Assert.IsEmpty(result.Errors);
    }
}