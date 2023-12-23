using Microsoft.Azure.Functions.Worker.Http;

namespace NixpkgsShields;

public static class HttpResponseDataExtensions
{
    public static HttpResponseData AddCacheHeaders(this HttpResponseData response)
    {
        response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
        response.Headers.Add("Pragma", "no-cache");
        response.Headers.TryAddWithoutValidation("Expires", "0");
        return response;
    }

    public static HttpResponseData AddTextContentType(this HttpResponseData response)
    {
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
        return response;
    }

    public static HttpResponseData AddSvgContentType(this HttpResponseData response)
    {
        response.Headers.Add("Content-Type", "image/svg+xml; charset=utf-8");
        return response;
    }
}
