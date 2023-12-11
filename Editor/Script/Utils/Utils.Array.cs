using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gomoru.su.clothfire
{
    partial class Utils
    {
        public static T[] Single<T>() => SingleArrayCache<T>.Single;

        private static class SingleArrayCache<T>
        {
            public static readonly T[] Single = new T[1];
        }
    }
}
