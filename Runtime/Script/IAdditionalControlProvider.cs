using System;
using System.Collections;
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
        void GetAdditionalControls(AdditionalControlContainer destination);
    }

    internal sealed class AdditionalControlContainer
    {
        private Dictionary<IEnumerable<Condition>, List<AdditionalControl>> _dictionary = new Dictionary<IEnumerable<Condition>, List<AdditionalControl>>(new Comparer());


        public void Add(IEnumerable<Condition> key, AdditionalControl control)
        {
            if (!_dictionary.TryGetValue(key, out var list))
            {
                list = new List<AdditionalControl>() { control };
                _dictionary.Add(key, list);
            }
            else
            {
                list.Add(control);
            }
        }

        public ILookup<IEnumerable<Condition>, AdditionalControl> Items => _dictionary.SelectMany(x => x.Value, Tuple.Create).ToLookup(x => x.Item1.Key, x => x.Item2);

        private sealed class Comparer : IEqualityComparer<IEnumerable<Condition>>
        {
            public bool Equals(IEnumerable<Condition> x, IEnumerable<Condition> y)
            {
                return (x == null && y == null) || GetHashCode(x) == GetHashCode(y);
            }

            public int GetHashCode(IEnumerable<Condition> obj)
            {
                var hash = new HashCode();
                foreach(var condition in obj)
                {
                    hash = hash.Append(condition.Path);
                    hash = hash.Append(condition.State);
                }
                return hash.GetHashCode();
            }
        }
    }
}
