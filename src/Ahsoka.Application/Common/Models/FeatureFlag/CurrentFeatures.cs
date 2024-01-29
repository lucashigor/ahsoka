namespace Ahsoka.Application;

public sealed record CurrentFeatures
{
    private CurrentFeatures(string value)
    {
        Value = value;
    }

    public string Value { get; init; }

    public static CurrentFeatures FeatureFlagToTest = new("FeatureFlagToTest");

    public override string ToString() => Value;
}
