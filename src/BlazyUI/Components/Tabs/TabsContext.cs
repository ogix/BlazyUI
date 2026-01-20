namespace BlazyUI;

/// <summary>
/// Context for cascading tab state and callbacks to TabItem components.
/// </summary>
public class TabsContext
{
    /// <summary>
    /// The radio group name for the tabs.
    /// </summary>
    public string GroupName { get; }

    /// <summary>
    /// The currently active tab ID.
    /// </summary>
    public string? ActiveTab { get; }

    /// <summary>
    /// Callback to notify the parent Tabs component when a tab is selected.
    /// </summary>
    public Action<string?>? OnTabSelected { get; }

    /// <summary>
    /// Creates a new TabsContext.
    /// </summary>
    public TabsContext(string groupName, string? activeTab, Action<string?>? onTabSelected)
    {
        GroupName = groupName;
        ActiveTab = activeTab;
        OnTabSelected = onTabSelected;
    }
}
