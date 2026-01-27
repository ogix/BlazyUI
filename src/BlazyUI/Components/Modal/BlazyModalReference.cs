// src/BlazyUI/Components/Modal/BlazyModalReference.cs
namespace BlazyUI;

internal class BlazyModalReference
{
    public Guid Id { get; } = Guid.NewGuid();
    public BlazyModalType Type { get; init; }
    public string? Title { get; init; }
    public string? Message { get; init; }
    public string OkText { get; init; } = "OK";
    public string CancelText { get; init; } = "Cancel";
    public bool CloseOnBackdropClick { get; init; }
    public Type? ComponentType { get; init; }
    public Dictionary<string, object>? Parameters { get; init; }
    public string? CssClass { get; init; }
    public TaskCompletionSource<BlazyModalResult> TaskCompletionSource { get; init; } = default!;
    public BlazyModalInstance? Instance { get; set; }
    public bool IsClosing { get; set; }
}
