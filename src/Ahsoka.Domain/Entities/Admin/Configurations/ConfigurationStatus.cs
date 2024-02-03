using Ahsoka.Domain.Common;

namespace Ahsoka.Domain.Entities.Admin.Configurations;

public record ConfigurationStatus : Enumeration<int>
{
    protected ConfigurationStatus(int id, string name) : base(id, name)
    {
    }

    public static ConfigurationStatus Undefined { get; } = new(0, nameof(Undefined));
    public static ConfigurationStatus Awaiting { get; } = new(1, nameof(Awaiting));
    public static ConfigurationStatus Active { get; } = new(2, nameof(Active));
    public static ConfigurationStatus Expired { get; } = new(3, nameof(Expired));
}
