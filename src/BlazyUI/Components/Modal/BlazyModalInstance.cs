// src/BlazyUI/Components/Modal/BlazyModalInstance.cs
namespace BlazyUI;

public class BlazyModalInstance
{
    private readonly TaskCompletionSource<BlazyModalResult> _tcs;
    private readonly Action<BlazyModalInstance> _onClose;

    internal BlazyModalInstance(
        TaskCompletionSource<BlazyModalResult> tcs,
        Action<BlazyModalInstance> onClose)
    {
        _tcs = tcs;
        _onClose = onClose;
    }

    public void Close(bool confirmed = true)
    {
        _tcs.TrySetResult(new BlazyModalResult { Confirmed = confirmed });
        _onClose(this);
    }

    public void Close<T>(T? data, bool confirmed = true)
    {
        _tcs.TrySetResult(new BlazyModalResult<T>
        {
            Confirmed = confirmed,
            Data = data
        });
        _onClose(this);
    }
}
