using Apps.GlobalLinkAI.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using RestSharp;
using RestSharp.Authenticators;

namespace Apps.GlobalLinkAI.Authenticators;

public class ApiKeyAuthenticator(IEnumerable<AuthenticationCredentialsProvider> creds) : IAuthenticator
{
    public ValueTask Authenticate(IRestClient client, RestRequest request)
    {
        string accessToken = creds.Get(CredsNames.ApiKey).Value;
        request.AddHeader("subscription-key", accessToken);
        return ValueTask.CompletedTask;
    }
}