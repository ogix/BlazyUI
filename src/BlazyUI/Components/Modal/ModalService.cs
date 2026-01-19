// src/BlazyUI/Components/Modal/ModalService.cs
using Microsoft.AspNetCore.Components;

namespace BlazyUI;

public class ModalService : IModalService
{
    private readonly List<ModalReference> _modals = new();

    public event Action? OnChange;

    internal IReadOnlyList<ModalReference> Modals => _modals;

    public Task Info(string title, string message, string? okText = "OK")
    {
        return ShowSimpleDialog(ModalType.Info, title, message, okText ?? "OK", closeOnBackdropClick: true);
    }

    public Task Warning(string title, string message, string? okText = "OK")
    {
        return ShowSimpleDialog(ModalType.Warning, title, message, okText ?? "OK", closeOnBackdropClick: true);
    }

    public Task Error(string title, string message, string? okText = "OK")
    {
        return ShowSimpleDialog(ModalType.Error, title, message, okText ?? "OK", closeOnBackdropClick: true);
    }

    public async Task<bool> Confirm(
        string title,
        string message,
        string? confirmText = "Confirm",
        string? cancelText = "Cancel")
    {
        var tcs = new TaskCompletionSource<ModalResult>();

        var modalRef = new ModalReference
        {
            Type = ModalType.Confirm,
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

    public Task<ModalResult<TResult>> Show<TComponent, TResult>(
        Dictionary<string, object>? parameters = null,
        Action<ModalOptions>? configure = null)
        where TComponent : IComponent
    {
        return ShowCustomModal<TComponent, TResult>(parameters, configure);
    }

    public async Task<ModalResult> Show<TComponent>(
        Dictionary<string, object>? parameters = null,
        Action<ModalOptions>? configure = null)
        where TComponent : IComponent
    {
        var result = await ShowCustomModal<TComponent, object>(parameters, configure);
        return new ModalResult { Confirmed = result.Confirmed };
    }

    private async Task ShowSimpleDialog(
        ModalType type,
        string title,
        string message,
        string okText,
        bool closeOnBackdropClick)
    {
        var tcs = new TaskCompletionSource<ModalResult>();

        var modalRef = new ModalReference
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

    private async Task<ModalResult<TResult>> ShowCustomModal<TComponent, TResult>(
        Dictionary<string, object>? parameters,
        Action<ModalOptions>? configure)
        where TComponent : IComponent
    {
        var options = new ModalOptions();
        configure?.Invoke(options);

        var tcs = new TaskCompletionSource<ModalResult>();

        var modalRef = new ModalReference
        {
            Type = ModalType.Custom,
            ComponentType = typeof(TComponent),
            Parameters = parameters,
            CloseOnBackdropClick = options.CloseOnBackdropClick,
            CssClass = options.CssClass,
            TaskCompletionSource = tcs
        };

        modalRef.Instance = new ModalInstance(tcs, CloseModal);

        _modals.Add(modalRef);
        OnChange?.Invoke();

        var result = await tcs.Task;

        if (result is ModalResult<TResult> typedResult)
        {
            return typedResult;
        }

        return new ModalResult<TResult> { Confirmed = result.Confirmed };
    }

    internal void CloseModal(ModalReference modal, bool confirmed)
    {
        modal.TaskCompletionSource.TrySetResult(new ModalResult { Confirmed = confirmed });
        CloseModal(modal.Instance!);
    }

    private void CloseModal(ModalInstance instance)
    {
        var modal = _modals.FirstOrDefault(m => m.Instance == instance);
        if (modal != null)
        {
            _modals.Remove(modal);
            OnChange?.Invoke();
        }
    }

    internal void CloseModalById(Guid id, bool confirmed)
    {
        var modal = _modals.FirstOrDefault(m => m.Id == id);
        if (modal != null)
        {
            modal.TaskCompletionSource.TrySetResult(new ModalResult { Confirmed = confirmed });
            _modals.Remove(modal);
            OnChange?.Invoke();
        }
    }
}
