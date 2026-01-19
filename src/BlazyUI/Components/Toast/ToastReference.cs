namespace BlazyUI;

internal class ToastReference
{
    public Guid Id { get; } = Guid.NewGuid();
    public required ToastType Type { get; init; }
    public required string Message { get; init; }
    public required ToastOptions Options { get; init; }
}
