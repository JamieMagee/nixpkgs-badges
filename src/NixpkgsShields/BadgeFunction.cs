using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace NixpkgsShields;

public sealed class BadgeFunction(BadgeService badgeService, ILogger<BadgeFunction> logger)
{
    [Function("badge")]
    public async Task<HttpResponseData> BadgeAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
    {
        HttpResponseData response;

        var queryParam = req.Query.Get("pr");
        req.Headers.TryGetValues("Referer", out var header);

        if (queryParam == null && header == null)
        {
            response = req.CreateResponse(HttpStatusCode.BadRequest)
                .AddCacheHeaders()
                .AddTextContentType();
            await response.WriteStringAsync("Missing query parameter 'pr' or 'Referer' header");
            return response;
        }

        var pr = queryParam ?? header!.First().Split('/').Last();
        if (!int.TryParse(pr, out var prNumber))
        {
            response = req.CreateResponse(HttpStatusCode.BadRequest)
                .AddCacheHeaders()
                .AddTextContentType();
            await response.WriteStringAsync("Invalid query parameter 'pr' or 'Referer' header");
            return response;
        }

        response = req.CreateResponse(HttpStatusCode.OK)
            .AddCacheHeaders()
            .AddSvgContentType();
        await response.WriteStringAsync(await badgeService.GetShieldAsync(prNumber));
        return response;
    }
}
