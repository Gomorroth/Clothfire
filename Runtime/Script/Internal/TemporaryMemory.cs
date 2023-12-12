using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gomoru.su.clothfire
{
    internal readonly struct TemporaryMemory<T> : IDisposable
    {
        private readonly T[] _buffer;
        private readonly int _size;

        public Memory<T> Memory => _buffer.AsMemory(0, _size);
        public Span<T> Span => _buffer.AsSpan(0, _size);

        public bool IsEmpty => _size == 0;

        private TemporaryMemory(int size)
        {
            _buffer = ArrayPool<T>.Shared.Rent(size);
            _size = size;
        }

        public static TemporaryMemory<T> Allocate(int size) => new TemporaryMemory<T>(size);

        public void Dispose()
        {
            if (_buffer != null)
                ArrayPool<T>.Shared.Return(_buffer);
        }
    }

    internal static class TemporaryMemoryExtension
    {
        public static TemporaryMemory<T> ToTemporaryMemory<T>(this IEnumerable<T> enumerable)
        {
            int count;
            if (enumerable is T[] array)
                count = array.Length;
            else if (enumerable is ICollection<T> collection)
                count = collection.Count;
            else
                count = enumerable.Count();

            var memory = TemporaryMemory<T>.Allocate(count);

            var span = memory.Span;
            int idx = 0;
            foreach(var x in enumerable)
            {
                span[idx++] = x;
            }

            return memory;
        }
    }
}
