namespace BlazyUI;

/// <summary>
/// Controls how tab content is rendered.
/// </summary>
public enum BlazyTabLoadMode
{
    /// <summary>
    /// All tab content is rendered upfront (default). Tab visibility is controlled by CSS.
    /// </summary>
    Eager,

    /// <summary>
    /// Only the active tab's content is rendered. Switching away destroys the content.
    /// Requires ActiveTab binding.
    /// </summary>
    Lazy,

    /// <summary>
    /// Tab content is rendered on first activation and kept in the DOM.
    /// Requires ActiveTab binding.
    /// </summary>
    LazyOnce
}
