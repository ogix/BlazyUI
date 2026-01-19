namespace BlazyUI;

public interface IToastService
{
    event Action? OnChange;
    void Info(string message, Action<ToastOptions>? configure = null);
    void Success(string message, Action<ToastOptions>? configure = null);
    void Warning(string message, Action<ToastOptions>? configure = null);
    void Error(string message, Action<ToastOptions>? configure = null);
    void Clear();
}
