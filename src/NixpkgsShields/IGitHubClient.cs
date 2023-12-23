namespace NixpkgsShields;

public interface IGitHubClient
{
    Task<(bool IsMerged, string? MergeCommitSha)> IsPullRequestMergedAsync(int pullRequestNumber);

    Task<bool> IsMergedIntoBranchAsync(string branchName, string pullRequestSha);
}
