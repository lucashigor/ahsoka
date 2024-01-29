namespace Ahsoka.Application;

public interface IFeatureFlagService
{
    Task<bool> IsEnabledAsync(CurrentFeatures feature);
    Task<bool> IsEnabledAsync(CurrentFeatures feature, Dictionary<string, string> att);
}
