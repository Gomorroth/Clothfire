using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace gomoru.su.clothfire
{
    internal interface IAdditionalControlProvider
    {
        void GetAdditionalControls(List<(SingleOrArray<Condition> Conditions, SingleOrArray<AdditionalControl> Controls)> destination);
    }

    internal readonly struct SingleOrArray<T>
    {
        public readonly T Value;
        public readonly IEnumerable<T> Values;
        private readonly int _count;

        public SingleOrArray(T value, T[] values) : this(value, values, values?.Length ?? 1) { }

        public SingleOrArray(T value, IEnumerable<T> values, int count)
        {
            Value = value;
            Values = values;
            _count = count;
        }

        public int Length => _count;

        public static implicit operator SingleOrArray<T>(T value)
        {
            return new SingleOrArray<T>(value, null, 1);
        }

        public static implicit operator SingleOrArray<T>(T[] value)
        {
            return new SingleOrArray<T>(default, value, value.Length);
        }

        public static implicit operator SingleOrArray<T>(List<T> value)
        {
            return new SingleOrArray<T>(default, value, value.Count);
        }
    }
}
