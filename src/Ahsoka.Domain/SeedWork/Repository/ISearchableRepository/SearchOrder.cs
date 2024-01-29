namespace Ahsoka.Domain;

public record SearchOrder : Enumeration<int>
{
    private SearchOrder(int id, string name) : base(id, name)
    {
    }

    public static SearchOrder Undefined { get; } = new(0, nameof(Undefined));
    public static SearchOrder Asc { get; } = new(1, nameof(Asc));
    public static SearchOrder Desc { get; } = new(2, nameof(Desc));
}