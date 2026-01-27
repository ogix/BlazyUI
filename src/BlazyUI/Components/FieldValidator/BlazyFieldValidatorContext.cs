namespace BlazyUI;

/// <summary>
/// Context cascaded from FieldValidator to child input components.
/// </summary>
public class BlazyFieldValidatorContext
{
    /// <summary>
    /// Whether child inputs should add the 'validator' CSS class.
    /// </summary>
    public bool EnableValidatorClass { get; }

    public BlazyFieldValidatorContext(bool enableValidatorClass = true)
    {
        EnableValidatorClass = enableValidatorClass;
    }
}
