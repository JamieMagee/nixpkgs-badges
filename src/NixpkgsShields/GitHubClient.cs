using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using NixpkgsShields.Models;

namespace NixpkgsShields;

public sealed class GitHubClient(HttpClient httpClient, ILogger<GitHubClient> logger)
    : IGitHubClient
{
    public async Task<(bool IsMerged, string? MergeCommitSha)> IsPullRequestMergedAsync(int pullRequestNumber)
    {
        var url = $"repos/NixOS/nixpkgs/pulls/{pullRequestNumber}";
        var response = await httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError(
                "Failed to get pull request {PullRequestNumber} from GitHub: {StatusCode}",
                pullRequestNumber,
                response.StatusCode);
            return (false, null);
        }

        var result = await response.Content.ReadFromJsonAsync<GitHubPullRequestResponse>();
        return (result!.Merged, result.MergeCommitSha);
    }

    public async Task<bool> IsMergedIntoBranchAsync(string branchName, string pullRequestSha)
    {
        var url = $"repos/NixOS/nixpkgs/compare/{pullRequestSha}...{branchName}";
        var response = await httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError(
                "Failed to get pull request {PullRequestNumber} from GitHub: {StatusCode}",
                pullRequestSha,
                response.StatusCode);
            return false;
        }

        var result = await response.Content.ReadFromJsonAsync<GitHubCompareResponse>();
        return result!.BehindBy == 0;
    }
}
