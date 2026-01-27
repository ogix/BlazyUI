// src/BlazyUI/Components/Modal/IBlazyModalService.cs
using Microsoft.AspNetCore.Components;

namespace BlazyUI;

public interface IBlazyModalService
{
    /// <summary>
    /// Event raised when modals collection changes.
    /// </summary>
    event Action? OnChange;

    /// <summary>
    /// Shows an info dialog and waits for it to close.
    /// </summary>
    Task Info(string title, string message, string? okText = "OK");

    /// <summary>
    /// Shows a warning dialog and waits for it to close.
    /// </summary>
    Task Warning(string title, string message, string? okText = "OK");

    /// <summary>
    /// Shows an error dialog and waits for it to close.
    /// </summary>
    Task Error(string title, string message, string? okText = "OK");

    /// <summary>
    /// Shows a confirmation dialog and returns true if confirmed.
    /// </summary>
    Task<bool> Confirm(
        string title,
        string message,
        string? confirmText = "Confirm",
        string? cancelText = "Cancel");

    /// <summary>
    /// Shows a custom component modal and returns the typed result.
    /// </summary>
    Task<BlazyModalResult<TResult>> Show<TComponent, TResult>(
        Dictionary<string, object>? parameters = null,
        Action<BlazyModalOptions>? configure = null)
        where TComponent : IComponent;

    /// <summary>
    /// Shows a custom component modal and waits for it to close.
    /// </summary>
    Task<BlazyModalResult> Show<TComponent>(
        Dictionary<string, object>? parameters = null,
        Action<BlazyModalOptions>? configure = null)
        where TComponent : IComponent;
}
