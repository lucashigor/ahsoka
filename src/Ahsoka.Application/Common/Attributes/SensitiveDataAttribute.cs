namespace Ahsoka.Application;

[AttributeUsage(AttributeTargets.Property)]
public class SensitiveDataAttribute : Attribute
{
    public bool IsSensitive { get; }

    public SensitiveDataAttribute(bool isSensitive = false)
    {
        IsSensitive = isSensitive;
    }
}
