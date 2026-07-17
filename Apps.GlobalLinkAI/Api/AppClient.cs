using Apps.GlobalLinkAI.Authenticators;
using Apps.GlobalLinkAI.Constants;
using Apps.GlobalLinkAI.Models.Response;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.GlobalLinkAI.Api;

public class AppClient(AuthenticationCredentialsProvider[] creds) : BlackBirdRestClient(new()
{
    BaseUrl = creds.Get(CredsNames.Host).Value.ToUri(),
    Authenticator = new ApiKeyAuthenticator(creds)
})
{
    protected override Exception ConfigureErrorException(RestResponse response)
    {
        var error = JsonConvert.DeserializeObject<ErrorResponse>(response.Content);
        throw new PluginApplicationException(error.Message);
    }
}