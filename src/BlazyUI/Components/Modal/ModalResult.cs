// src/BlazyUI/Components/Modal/ModalResult.cs
namespace BlazyUI;

public class ModalResult
{
    public bool Confirmed { get; init; }
}

public class ModalResult<T> : ModalResult
{
    public T? Data { get; init; }
}
