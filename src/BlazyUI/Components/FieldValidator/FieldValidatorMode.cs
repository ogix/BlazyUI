namespace BlazyUI;

/// <summary>
/// Determines which validation feedback is shown.
/// </summary>
public enum FieldValidatorMode
{
    /// <summary>
    /// Shows both HTML5 validator-hint and Blazor ValidationMessage.
    /// </summary>
    Both,

    /// <summary>
    /// Shows only HTML5 validator-hint (CSS-based, immediate feedback).
    /// </summary>
    Html5Only,

    /// <summary>
    /// Shows only Blazor ValidationMessage (EditContext-based).
    /// </summary>
    BlazorOnly
}
