// src/BlazyUI/Components/Modal/BlazyModalService.cs
using Microsoft.AspNetCore.Components;

namespace BlazyUI;

public class BlazyModalService : IBlazyModalService
{
    private readonly List<BlazyModalReference> _modals = new();

    public event Action? OnChange;

    internal IReadOnlyList<BlazyModalReference> Modals => _modals;

    public Task Info(string title, string message, string? okText = "OK")
    {
        return ShowSimpleDialog(BlazyModalType.Info, title, message, okText ?? "OK", closeOnBackdropClick: true);
    }

    public Task Warning(string title, string message, string? okText = "OK")
    {
        return ShowSimpleDialog(BlazyModalType.Warning, title, message, okText ?? "OK", closeOnBackdropClick: true);
    }

    public Task Error(string title, string message, string? okText = "OK")
    {
        return ShowSimpleDialog(BlazyModalType.Error, title, message, okText ?? "OK", closeOnBackdropClick: true);
    }

    public async Task<bool> Confirm(
        string title,
        string message,
        string? confirmText = "Confirm",
        string? cancelText = "Cancel")
    {
        var tcs = new TaskCompletionSource<BlazyModalResult>();

        var modalRef = new BlazyModalReference
        {
            Type = BlazyModalType.Confirm,
            Title = title,
            Message = message,
            OkText = confirmText ?? "Confirm",
            CancelText = cancelText ?? "Cancel",
            CloseOnBackdropClick = false,
            TaskCompletionSource = tcs
        };

        _modals.Add(modalRef);
        OnChange?.Invoke();

        var result = await tcs.Task;
        return result.Confirmed;
    }

    public Task<BlazyModalResult<TResult>> Show<TComponent, TResult>(
        Dictionary<string, object>? parameters = null,
        Action<BlazyModalOptions>? configure = null)
        where TComponent : IComponent
    {
        return ShowCustomModal<TComponent, TResult>(parameters, configure);
    }

    public async Task<BlazyModalResult> Show<TComponent>(
        Dictionary<string, object>? parameters = null,
        Action<BlazyModalOptions>? configure = null)
        where TComponent : IComponent
    {
        var result = await ShowCustomModal<TComponent, object>(parameters, configure);
        return new BlazyModalResult { Confirmed = result.Confirmed };
    }

    private async Task ShowSimpleDialog(
        BlazyModalType type,
        string title,
        string message,
        string okText,
        bool closeOnBackdropClick)
    {
        var tcs = new TaskCompletionSource<BlazyModalResult>();

        var modalRef = new BlazyModalReference
        {
            Type = type,
            Title = title,
            Message = message,
            OkText = okText,
            CloseOnBackdropClick = closeOnBackdropClick,
            TaskCompletionSource = tcs
        };

        _modals.Add(modalRef);
        OnChange?.Invoke();

        await tcs.Task;
    }

    private async Task<BlazyModalResult<TResult>> ShowCustomModal<TComponent, TResult>(
        Dictionary<string, object>? parameters,
        Action<BlazyModalOptions>? configure)
        where TComponent : IComponent
    {
        var options = new BlazyModalOptions();
        configure?.Invoke(options);

        var tcs = new TaskCompletionSource<BlazyModalResult>();

        var modalRef = new BlazyModalReference
        {
            Type = BlazyModalType.Custom,
            ComponentType = typeof(TComponent),
            Parameters = parameters,
            CloseOnBackdropClick = options.CloseOnBackdropClick,
            CssClass = options.CssClass,
            TaskCompletionSource = tcs
        };

        modalRef.Instance = new BlazyModalInstance(tcs, CloseModal);

        _modals.Add(modalRef);
        OnChange?.Invoke();

        var result = await tcs.Task;

        if (result is BlazyModalResult<TResult> typedResult)
        {
            return typedResult;
        }

        return new BlazyModalResult<TResult> { Confirmed = result.Confirmed };
    }

    internal void CloseModal(BlazyModalReference modal, bool confirmed)
    {
        modal.TaskCompletionSource.TrySetResult(new BlazyModalResult { Confirmed = confirmed });
        CloseModal(modal.Instance!);
    }

    private async void CloseModal(BlazyModalInstance instance)
    {
        var modal = _modals.FirstOrDefault(m => m.Instance == instance);
        if (modal != null)
        {
            modal.IsClosing = true;
            OnChange?.Invoke();
            await Task.Delay(300); // Wait for CSS closing animation
            _modals.Remove(modal);
            OnChange?.Invoke();
        }
    }

    internal void CloseModalById(Guid id, bool confirmed)
    {
        var modal = _modals.FirstOrDefault(m => m.Id == id);
        if (modal != null)
        {
            modal.TaskCompletionSource.TrySetResult(new BlazyModalResult { Confirmed = confirmed });
            _modals.Remove(modal);
            OnChange?.Invoke();
        }
    }
}
