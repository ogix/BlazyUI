namespace BlazyUI;

/// <summary>
/// Context for cascading tab state and callbacks to TabItem components.
/// </summary>
public class BlazyTabsContext
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
    /// The load mode for tab content rendering.
    /// </summary>
    public BlazyTabLoadMode LoadMode { get; }

    /// <summary>
    /// Set of tab IDs that have been activated at least once. Used by <see cref="BlazyTabLoadMode.LazyOnce"/>.
    /// </summary>
    public HashSet<string> ActivatedTabs { get; }

    /// <summary>
    /// Callback to notify the parent Tabs component when a tab is selected.
    /// </summary>
    public Action<string?>? OnTabSelected { get; }

    /// <summary>
    /// Creates a new TabsContext.
    /// </summary>
    public BlazyTabsContext(string groupName, string? activeTab, Action<string?>? onTabSelected, BlazyTabLoadMode loadMode, HashSet<string> activatedTabs)
    {
        GroupName = groupName;
        ActiveTab = activeTab;
        OnTabSelected = onTabSelected;
        LoadMode = loadMode;
        ActivatedTabs = activatedTabs;
    }
}
