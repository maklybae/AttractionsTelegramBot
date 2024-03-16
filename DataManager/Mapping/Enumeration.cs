using System.Reflection;

namespace DataManager.Mapping;

public abstract class Enumeration : IComparable
{
    public string Title { get; private set; }

    public int Id { get; private set; }

    protected Enumeration(int id, string title) => (Id, Title) = (id, title);

    public override string ToString() => Title;

    public static IEnumerable<T> GetAll<T>() where T : Enumeration =>
        typeof(T).GetFields(BindingFlags.Public |
                            BindingFlags.Static |
                            BindingFlags.DeclaredOnly)
                 .Select(f => f.GetValue(null))
                 .Cast<T>();

    public override bool Equals(object? obj)
    {
        if (obj is not Enumeration otherValue)
        {
            return false;
        }

        var typeMatches = GetType().Equals(obj.GetType());
        var valueMatches = Id.Equals(otherValue.Id);

        return typeMatches && valueMatches;
    }

    public int CompareTo(object? other) => Id.CompareTo((other as Enumeration)?.Id);
}
