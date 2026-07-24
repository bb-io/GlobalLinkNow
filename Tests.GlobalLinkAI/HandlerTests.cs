using Apps.GlobalLinkAI.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Tests.GlobalLinkAI.Base;

namespace Tests.GlobalLinkAI;

[TestClass]
public class HandlerTests : TestBase
{
    [TestMethod]
    public async Task LanguageDataSourceHandler_ReturnsLanguages()
    {
        // Arrange
        var handler = new LanguageDataSourceHandler(InvocationContext);

        // Act
        var result = await handler.GetDataAsync(new() { SearchString = "" }, CancellationToken.None);

        // Assert
        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }
}