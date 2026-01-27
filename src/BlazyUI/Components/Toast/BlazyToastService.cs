namespace BlazyUI;

public class BlazyToastService : IBlazyToastService
{
    private readonly List<BlazyToastReference> _toasts = new();

    public event Action? OnChange;
    internal IReadOnlyList<BlazyToastReference> Toasts => _toasts;

    public void Info(string message, Action<BlazyToastOptions>? configure = null)
        => Show(BlazyToastType.Info, message, configure);

    public void Success(string message, Action<BlazyToastOptions>? configure = null)
        => Show(BlazyToastType.Success, message, configure);

    public void Warning(string message, Action<BlazyToastOptions>? configure = null)
        => Show(BlazyToastType.Warning, message, configure);

    public void Error(string message, Action<BlazyToastOptions>? configure = null)
        => Show(BlazyToastType.Error, message, configure);

    public void Clear()
    {
        _toasts.Clear();
        OnChange?.Invoke();
    }

    private void Show(BlazyToastType type, string message, Action<BlazyToastOptions>? configure)
    {
        var options = new BlazyToastOptions();
        configure?.Invoke(options);
        _toasts.Add(new BlazyToastReference { Type = type, Message = message, Options = options });
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
