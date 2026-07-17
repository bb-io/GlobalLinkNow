using Apps.GlobalLinkAI.Invocables;
using Apps.GlobalLinkAI.Models.Response.Engines;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.GlobalLinkAI.DataSourceHandlers;

public class EngineDataSourceHandler(InvocationContext invocationContext) : AppInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var request = new RestRequest("/apigateway/mtengine/organization/information/all");
        var response = await Client.ExecuteWithErrorHandling<ListEnginesResponse>(request);

        return response.Engines
            .Where(x => context.SearchString is null ||
                        x.Alias.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Select(x => new DataSourceItem(x.Id, x.Alias));
    }
}