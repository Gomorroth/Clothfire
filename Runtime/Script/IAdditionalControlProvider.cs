using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace gomoru.su.clothfire
{
    internal interface IAdditionalControlProvider
    {
        GameObject GameObject { get; }
        void GetAdditionalControls(AdditionalControlContainer destination);
    }

    internal sealed class AdditionalControlContainer
    {
        private Dictionary<IEnumerable<AdditionalControlCondition>, List<AdditionalControl>> _dictionary = new Dictionary<IEnumerable<AdditionalControlCondition>, List<AdditionalControl>>(new Comparer());

        public GameObject CurrentRootObject;
        public GameObject AvatarRootObject;

        public void Add(IEnumerable<AdditionalControlCondition> key, AdditionalControl control)
        {
            control.Root = CurrentRootObject;
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

        public ILookup<IEnumerable<AdditionalControlCondition>, AdditionalControl> Items => _dictionary.SelectMany(x => x.Value, Tuple.Create).ToLookup(x => x.Item1.Key, x => x.Item2);

        private sealed class Comparer : IEqualityComparer<IEnumerable<AdditionalControlCondition>>
        {
            public bool Equals(IEnumerable<AdditionalControlCondition> x, IEnumerable<AdditionalControlCondition> y)
            {
                return (x == null && y == null) || GetHashCode(x) == GetHashCode(y);
            }

            public int GetHashCode(IEnumerable<AdditionalControlCondition> obj)
            {
                var hash = new HashCode();
                foreach(var condition in obj)
                {
                    hash = hash.Append(condition.Object);
                    hash = hash.Append(condition.State);
                }
                return hash.GetHashCode();
            }
        }
    }
}
