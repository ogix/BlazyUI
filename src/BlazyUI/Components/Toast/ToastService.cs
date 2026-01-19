namespace BlazyUI;

public class ToastService : IToastService
{
    private readonly List<ToastReference> _toasts = new();

    public event Action? OnChange;
    internal IReadOnlyList<ToastReference> Toasts => _toasts;

    public void Info(string message, Action<ToastOptions>? configure = null)
        => Show(ToastType.Info, message, configure);

    public void Success(string message, Action<ToastOptions>? configure = null)
        => Show(ToastType.Success, message, configure);

    public void Warning(string message, Action<ToastOptions>? configure = null)
        => Show(ToastType.Warning, message, configure);

    public void Error(string message, Action<ToastOptions>? configure = null)
        => Show(ToastType.Error, message, configure);

    public void Clear()
    {
        _toasts.Clear();
        OnChange?.Invoke();
    }

    private void Show(ToastType type, string message, Action<ToastOptions>? configure)
    {
        var options = new ToastOptions();
        configure?.Invoke(options);
        _toasts.Add(new ToastReference { Type = type, Message = message, Options = options });
        OnChange?.Invoke();
    }

    internal void Remove(Guid id)
    {
        var toast = _toasts.FirstOrDefault(t => t.Id == id);
        if (toast != null)
        {
            _toasts.Remove(toast);
            OnChange?.Invoke();
        }
    }
}
