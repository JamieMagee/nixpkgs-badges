using System.Text.Json.Serialization;

namespace NixpkgsShields.Models;

public sealed record GitHubPullRequestResponse(
    [property: JsonPropertyName("merged")] bool Merged,
    [property: JsonPropertyName("merge_commit_sha")]
    string MergeCommitSha);
