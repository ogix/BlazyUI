namespace BlazyUI;

/// <summary>
/// Specifies how the label should be rendered.
/// </summary>
public enum BlazyLabelVariant
{
    /// <summary>
    /// Renders as a standalone label element.
    /// </summary>
    Standalone,

    /// <summary>
    /// Wraps an input with the label inside a container with "input" class.
    /// </summary>
    Input,

    /// <summary>
    /// Wraps a select with the label inside a container with "select" class.
    /// </summary>
    Select,

    /// <summary>
    /// Wraps content with a floating label effect.
    /// </summary>
    Floating
}
