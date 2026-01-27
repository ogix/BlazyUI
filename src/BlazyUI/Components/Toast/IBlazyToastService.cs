namespace BlazyUI;

public interface IBlazyToastService
{
    event Action? OnChange;
    void Info(string message, Action<BlazyToastOptions>? configure = null);
    void Success(string message, Action<BlazyToastOptions>? configure = null);
    void Warning(string message, Action<BlazyToastOptions>? configure = null);
    void Error(string message, Action<BlazyToastOptions>? configure = null);
    void Clear();
}
