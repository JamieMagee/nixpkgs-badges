using System.Reflection;
using Svg;
using Svg.Transforms;

namespace NixpkgsShields;

public sealed class BadgeService(IGitHubClient gitHubClient) : IBadgeService
{
    private const int BadgeHeight = 28;
    private static readonly IEnumerable<string> Branches = ["master", "nixpkgs-unstable", "nixos-unstable-small", "nixos-unstable"];

    public async Task<string> GetShieldAsync(int pullRequestNumber)
    {
        var (isMerged, mergeCommitSha) = await gitHubClient.IsPullRequestMergedAsync(pullRequestNumber);
        if (!isMerged)
        {
            return (await LoadBadgeAsync("pr", false)).GetXML();
        }

        var parent = new SvgDocument
        {
            Height = Branches.Count() * BadgeHeight,
        };

        var tasks = Branches.Select(branch => (branch, gitHubClient.IsMergedIntoBranchAsync(branch, mergeCommitSha!)))
            .ToDictionary();
        foreach (var kv in tasks)
        {
            var isMergedIntoBranch = await kv.Value;
            var shield = await LoadBadgeAsync(kv.Key, isMergedIntoBranch);
            shield.Transforms = [new SvgTranslate(0, Branches.TakeWhile(x => x != kv.Key).Count() * BadgeHeight)];
            parent.Width = Math.Max(parent.Width, shield.Width);
            parent.Children.Add(shield);
        }

        return parent.GetXML();
    }

    private static async Task<SvgDocument> LoadBadgeAsync(string branchName, bool isMerged)
    {
        var resourceName = $"{branchName}-{(isMerged ? "merged" : "not-merged")}.svg";
        var location = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "Resources", resourceName);
        await using var stream = File.OpenRead(location);
        return SvgDocument.Open<SvgDocument>(stream);
    }
}
