// src/BlazyUI/Components/Modal/ModalInstance.cs
namespace BlazyUI;

public class ModalInstance
{
    private readonly TaskCompletionSource<ModalResult> _tcs;
    private readonly Action<ModalInstance> _onClose;

    internal ModalInstance(
        TaskCompletionSource<ModalResult> tcs,
        Action<ModalInstance> onClose)
    {
        _tcs = tcs;
        _onClose = onClose;
    }

    public void Close(bool confirmed = true)
    {
        _tcs.TrySetResult(new ModalResult { Confirmed = confirmed });
        _onClose(this);
    }

    public void Close<T>(T? data, bool confirmed = true)
    {
        _tcs.TrySetResult(new ModalResult<T>
        {
            Confirmed = confirmed,
            Data = data
        });
        _onClose(this);
    }
}
