namespace Ahsoka.Domain;

public record SearchOutput<TAggregate>(
    int CurrentPage,
    int PerPage,
    int Total,
    ICollection<TAggregate> Items
);
