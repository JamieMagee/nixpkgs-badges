namespace NixpkgsShields;

public interface IBadgeService
{
    Task<string> GetShieldAsync(int pullRequestNumber);
}
