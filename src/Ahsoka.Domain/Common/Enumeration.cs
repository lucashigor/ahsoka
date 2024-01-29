namespace Ahsoka.Domain;

using System.Data;
using System.Reflection;
public abstract record Enumeration<TKey>
{
    public string Name { get; init; }
    public TKey Key { get; init; }

    protected Enumeration(TKey id, string name) => (Key, Name) = (id, name);

    public override string ToString() => Name;

    public static IEnumerable<T> GetAll<T>() where T : Enumeration<TKey> =>
        typeof(T).GetFields(BindingFlags.Public |
                            BindingFlags.Static |
                            BindingFlags.DeclaredOnly)
            .Select(f => f.GetValue(null))
            .Cast<T>();

    public static T GetByKey<T>(TKey key) where T : Enumeration<TKey>
    {
        return GetAll<T>().First(x => x.Key.Equals(key));
    }
}
