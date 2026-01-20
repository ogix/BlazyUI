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
    /// Creates a new DrawerContext.
    /// </summary>
    public DrawerContext(string id)
    {
        Id = id;
    }
}
