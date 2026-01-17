using Microsoft.AspNetCore.Components;
using TailwindMerge;

namespace BlazyUI;

/// <summary>
/// Base component class for all BlazyUI components.
/// Provides common functionality for class merging and additional attributes.
/// </summary>
public abstract class BlazyComponentBase : ComponentBase
{
    [Inject]
    protected TwMerge TwMerge { get; set; } = default!;

    /// <summary>
    /// Additional CSS classes to apply to the component.
    /// These classes are merged with the component's default classes using TailwindMerge.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional HTML attributes to apply to the component's root element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Merges the provided CSS classes using TailwindMerge.
    /// Later classes override earlier ones when there are conflicts.
    /// </summary>
    /// <param name="defaultClasses">The component's default CSS classes.</param>
    /// <param name="additionalClasses">Optional additional classes to merge before the Class parameter.</param>
    /// <returns>The merged CSS class string.</returns>
    protected string MergeClasses(string? defaultClasses, string? additionalClasses = null)
    {
        return TwMerge.Merge(defaultClasses, additionalClasses, Class) ?? string.Empty;
    }
}
