namespace Ahsoka.Domain;

public record ConfigurationStatus : Enumeration<int>
{
    protected ConfigurationStatus(int id, string name) : base(id, name)
    {
    }

    public static ConfigurationStatus Undefined { get; } = new(0, nameof(Undefined));
    public static ConfigurationStatus Previous { get; } = new(1, nameof(Previous));
    public static ConfigurationStatus Active { get; } = new(2, nameof(Active));
    public static ConfigurationStatus Disable { get; } = new(3, nameof(Disable));
}
