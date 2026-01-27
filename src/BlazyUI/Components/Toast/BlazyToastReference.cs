namespace BlazyUI;

internal class BlazyToastReference
{
    public Guid Id { get; } = Guid.NewGuid();
    public required BlazyToastType Type { get; init; }
    public required string Message { get; init; }
    public required BlazyToastOptions Options { get; init; }
}
