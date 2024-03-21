using System.Reflection;

namespace DataManager.Mapping;

/// <summary>
/// Base class for implementing enumerations with ID and title properties.
/// </summary>
public abstract class Enumeration : IComparable
{
    /// <summary>
    /// Gets the title of the enumeration value.
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// Gets the ID of the enumeration value.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Initializes a new instance of the Enumeration class with the specified ID and title.
    /// </summary>
    /// <param name="id">The ID of the enumeration value.</param>
    /// <param name="title">The title of the enumeration value.</param>
    protected Enumeration(int id, string title) => (Id, Title) = (id, title);

    /// <summary>
    /// Returns the title of the enumeration value.
    /// </summary>
    /// <returns>The title of the enumeration value.</returns>
    public override string ToString() => Title;

    /// <summary>
    /// Retrieves all values of the specified enumeration type.
    /// </summary>
    /// <typeparam name="T">The type of enumeration.</typeparam>
    /// <returns>All values of the specified enumeration type.</returns>
    public static IEnumerable<T> GetAll<T>() where T : Enumeration =>
        typeof(T).GetFields(BindingFlags.Public |
                            BindingFlags.Static |
                            BindingFlags.DeclaredOnly)
                 .Select(f => f.GetValue(null))
                 .Cast<T>();

    /// <summary>
    /// Determines whether the specified object is equal to the current enumeration value.
    /// </summary>
    /// <param name="obj">The object to compare with the current enumeration value.</param>
    /// <returns>True if the specified object is equal to the current enumeration value; otherwise, false.</returns>
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

    /// <summary>
    /// Compares the current enumeration value with another object and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
    /// </summary>
    /// <param name="other">The object to compare with the current enumeration value.</param>
    /// <returns>A value that indicates the relative order of the objects being compared.</returns>
    public int CompareTo(object? other) => Id.CompareTo((other as Enumeration)?.Id);

    /// <summary>
    /// Compares the current enumeration value with another object and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
    /// </summary>
    /// <param name="other">The object to compare with the current enumeration value.</param>
    /// <returns>A value that indicates the relative order of the objects being compared.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Title, Id);
    }
}
