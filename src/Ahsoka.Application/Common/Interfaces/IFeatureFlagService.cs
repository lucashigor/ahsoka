using Ahsoka.Application.Common.Models.FeatureFlag;

namespace Ahsoka.Application.Common.Interfaces;

public interface IFeatureFlagService
{
    Task<bool> IsEnabledAsync(CurrentFeatures feature);
    Task<bool> IsEnabledAsync(CurrentFeatures feature, Dictionary<string, string> att);
}
