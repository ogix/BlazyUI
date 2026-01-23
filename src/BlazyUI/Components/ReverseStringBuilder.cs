using System.Buffers;
using System.Globalization;

namespace BlazyUI.Components;

/// <summary>
/// A ref struct that builds strings by prepending characters efficiently.
/// Based on ASP.NET Core's ReverseStringBuilder implementation.
/// </summary>
internal ref struct ReverseStringBuilder
{
    public const int MinimumRentedArraySize = 1024;

    private static readonly ArrayPool<char> ArrayPool = ArrayPool<char>.Shared;

    private int _nextEndIndex;
    private Span<char> _currentBuffer;
    private SequenceSegment? _fallbackSequenceSegment;

    public ReverseStringBuilder(Span<char> initialBuffer)
    {
        _currentBuffer = initialBuffer;
        _nextEndIndex = _currentBuffer.Length;
    }

    public readonly bool Empty => _nextEndIndex == _currentBuffer.Length;

    public void InsertFront(scoped ReadOnlySpan<char> span)
    {
        var startIndex = _nextEndIndex - span.Length;
        if (startIndex >= 0)
        {
            span.CopyTo(_currentBuffer[startIndex..]);
            _nextEndIndex = startIndex;
            return;
        }

        if (_fallbackSequenceSegment is null)
        {
            var remainingLength = -startIndex;
            var sizeToRent = _currentBuffer.Length + Math.Max(MinimumRentedArraySize, remainingLength * 2);
            var newBuffer = ArrayPool.Rent(sizeToRent);
            _fallbackSequenceSegment = new(newBuffer);

            var newEndIndex = newBuffer.Length - _currentBuffer.Length + _nextEndIndex;
            _currentBuffer[_nextEndIndex..].CopyTo(newBuffer.AsSpan(newEndIndex));
            newEndIndex -= span.Length;
            span.CopyTo(newBuffer.AsSpan(newEndIndex));

            _currentBuffer = newBuffer;
            _nextEndIndex = newEndIndex;
        }
        else
        {
            var remainingLength = -startIndex;
            span[remainingLength..].CopyTo(_currentBuffer);
            span = span[..remainingLength];

            var sizeToRent = Math.Max(MinimumRentedArraySize, remainingLength * 2);
            var newBuffer = ArrayPool.Rent(sizeToRent);
            _fallbackSequenceSegment = new(newBuffer, _fallbackSequenceSegment);
            _currentBuffer = newBuffer;

            startIndex = _currentBuffer.Length - remainingLength;
            span.CopyTo(_currentBuffer[startIndex..]);
            _nextEndIndex = startIndex;
        }
    }

    public void InsertFront<T>(T value) where T : ISpanFormattable
    {
        Span<char> result = stackalloc char[11];

        if (value.TryFormat(result, out var charsWritten, format: default, CultureInfo.InvariantCulture))
        {
            InsertFront(result[..charsWritten]);
        }
        else
        {
            InsertFront((IFormattable)value);
        }
    }

    public void InsertFront(IFormattable formattable)
        => InsertFront(formattable.ToString(null, CultureInfo.InvariantCulture));

    public override readonly string ToString()
        => _fallbackSequenceSegment is null
            ? new(_currentBuffer[_nextEndIndex..])
            : _fallbackSequenceSegment.ToString(_nextEndIndex);

    public readonly void Dispose()
    {
        _fallbackSequenceSegment?.Dispose();
    }

    private sealed class SequenceSegment : ReadOnlySequenceSegment<char>, IDisposable
    {
        private readonly char[] _array;

        public SequenceSegment(char[] array, SequenceSegment? next = null)
        {
            _array = array;
            Memory = array;
            Next = next;
        }

        public string ToString(int startIndex)
        {
            RunningIndex = 0;

            var tail = this;
            while (tail.Next is SequenceSegment next)
            {
                next.RunningIndex = tail.RunningIndex + tail.Memory.Length;
                tail = next;
            }

            var sequence = new ReadOnlySequence<char>(this, startIndex, tail, tail.Memory.Length);
            return sequence.ToString();
        }

        public void Dispose()
        {
            for (var current = this; current is not null; current = current.Next as SequenceSegment)
            {
                ArrayPool.Return(current._array);
            }
        }
    }
}
