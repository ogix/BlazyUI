// src/BlazyUI/Components/Modal/ModalReference.cs
namespace BlazyUI;

internal class ModalReference
{
    public Guid Id { get; } = Guid.NewGuid();
    public ModalType Type { get; init; }
    public string? Title { get; init; }
    public string? Message { get; init; }
    public string OkText { get; init; } = "OK";
    public string CancelText { get; init; } = "Cancel";
    public bool CloseOnBackdropClick { get; init; }
    public Type? ComponentType { get; init; }
    public Dictionary<string, object>? Parameters { get; init; }
    public string? CssClass { get; init; }
    public TaskCompletionSource<ModalResult> TaskCompletionSource { get; init; } = default!;
    public ModalInstance? Instance { get; set; }
    public bool IsClosing { get; set; }
}
