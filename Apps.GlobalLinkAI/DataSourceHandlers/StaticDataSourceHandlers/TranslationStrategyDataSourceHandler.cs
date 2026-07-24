using Apps.GlobalLinkAI.Constants;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.GlobalLinkAI.DataSourceHandlers.StaticDataSourceHandlers;

public class TranslationStrategyDataSourceHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return TranslationStrategy.SupportedStrategies.Select(x => new DataSourceItem(x, x));
    }
}