namespace BlazyUI;

/// <summary>
/// Context cascaded from FieldValidator to child input components.
/// </summary>
public class FieldValidatorContext
{
    /// <summary>
    /// Whether child inputs should add the 'validator' CSS class.
    /// </summary>
    public bool EnableValidatorClass { get; }

    public FieldValidatorContext(bool enableValidatorClass = true)
    {
        EnableValidatorClass = enableValidatorClass;
    }
}
