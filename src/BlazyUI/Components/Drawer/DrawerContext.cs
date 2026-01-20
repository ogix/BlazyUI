namespace BlazyUI;

/// <summary>
/// Context for cascading drawer state to child components.
/// </summary>
public class DrawerContext
{
    /// <summary>
    /// The unique identifier for the drawer checkbox toggle.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Whether the drawer is contained within its parent (uses absolute positioning).
    /// </summary>
    public bool Contained { get; }

    /// <summary>
    /// Creates a new DrawerContext.
    /// </summary>
    public DrawerContext(string id, bool contained = false)
    {
        Id = id;
        Contained = contained;
    }
}
