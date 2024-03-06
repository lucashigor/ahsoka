namespace Ahsoka.Domain.Common;

public interface ISoftDeletableEntity
{
    bool IsDeleted { get; }
}
