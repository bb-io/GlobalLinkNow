using RestSharp;

namespace Apps.GlobalLinkAI.Extensions;

public static class RestRequestExtensions
{
    public static RestRequest AddNullableQueryParameter(this RestRequest request, string paramName, string? paramValue)
    {
        if (string.IsNullOrWhiteSpace(paramValue) || string.IsNullOrWhiteSpace(paramName))
            return request;

        return request.AddQueryParameter(paramName, paramValue);
    }
    
    public static RestRequest AddNullableQueryParameter(this RestRequest request, string paramName, bool? paramValue)
    {
        if (!paramValue.HasValue || string.IsNullOrWhiteSpace(paramName))
            return request;

        return request.AddQueryParameter(paramName, paramValue.Value);
    }
}