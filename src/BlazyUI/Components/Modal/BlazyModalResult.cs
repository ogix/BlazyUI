// src/BlazyUI/Components/Modal/BlazyModalResult.cs
namespace BlazyUI;

public class BlazyModalResult
{
    public bool Confirmed { get; init; }
}

public class BlazyModalResult<T> : BlazyModalResult
{
    public T? Data { get; init; }
}
