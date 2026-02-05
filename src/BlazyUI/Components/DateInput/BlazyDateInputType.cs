namespace BlazyUI;

/// <summary>
/// The type of date input to render.
/// </summary>
public enum BlazyDateInputType
{
    /// <summary>
    /// A date picker (yyyy-MM-dd).
    /// </summary>
    Date,

    /// <summary>
    /// A date and time picker (yyyy-MM-ddTHH:mm:ss).
    /// </summary>
    DateTimeLocal,

    /// <summary>
    /// A month picker (yyyy-MM).
    /// </summary>
    Month,

    /// <summary>
    /// A time picker (HH:mm:ss).
    /// </summary>
    Time
}
