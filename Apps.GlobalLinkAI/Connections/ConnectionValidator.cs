using Apps.GlobalLinkAI.Api;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using RestSharp;

namespace Apps.GlobalLinkAI.Connections;

public class ConnectionValidator : IConnectionValidator
{
    public async ValueTask<ConnectionValidationResponse> ValidateConnection(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        CancellationToken cancellationToken)
    {
        var creds = authenticationCredentialsProviders.ToArray();
        
        var request = new RestRequest("/apigateway/mtengine/organization/information/all");
        await new AppClient(creds).ExecuteWithErrorHandling(request);
        
        return new()
        {
            IsValid = true
        };
    }
}