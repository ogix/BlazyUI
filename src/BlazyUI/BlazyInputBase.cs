using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using TailwindMerge;

namespace BlazyUI;

/// <summary>
/// Base class for form input components that integrate with Blazor's EditForm validation.
/// Provides TwMerge integration and automatic validation color support.
/// </summary>
/// <typeparam name="T">The type of value the input handles.</typeparam>
public abstract class BlazyInputBase<T> : InputBase<T>
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
    /// Returns the DaisyUI error class for validation state (e.g., "input-error", "select-error").
    /// Override this in derived components to return the appropriate error class.
    /// </summary>
    protected abstract string GetErrorColorClass();

    /// <summary>
    /// Gets the validation color class based on the current validation state.
    /// Returns the error class when there are validation messages, null otherwise.
    /// </summary>
    protected string? ValidationColorClass
    {
        get
        {
            if (EditContext is null)
                return null;

            var hasMessages = EditContext.GetValidationMessages(FieldIdentifier).Any();
            return hasMessages ? GetErrorColorClass() : null;
        }
    }

    /// <summary>
    /// Merges the provided CSS classes using TailwindMerge.
    /// Includes validation color class and the Class parameter for consumer overrides.
    /// Later classes override earlier ones when there are conflicts.
    /// </summary>
    /// <param name="classes">CSS classes to merge. Null values are ignored.</param>
    /// <returns>The merged CSS class string.</returns>
    protected string MergeClasses(params string?[] classes)
    {
        var allClasses = classes
            .Append(ValidationColorClass)
            .Append(Class)
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .ToArray();
        return TwMerge.Merge(allClasses) ?? string.Empty;
    }
}
