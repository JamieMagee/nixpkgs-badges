using System.Text.Json.Serialization;

namespace NixpkgsShields.Models;

public sealed record GitHubCompareResponse(
    [property: JsonPropertyName("behind_by")]
    int BehindBy);
