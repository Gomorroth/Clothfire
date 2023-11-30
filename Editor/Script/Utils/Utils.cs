using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace gomoru.su.clothfire
{
    internal static class Utils
    {
        public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> valueFactory)
        {
            if (!dictionary.TryGetValue(key, out var value))
            {
                value = valueFactory(key);
                dictionary.Add(key, value);
            }
            return value;
        }

        public static IEnumerable<(int Index, string Name)> EnumerateBlendshapes(this SkinnedMeshRenderer skinnedMeshRenderer)
        {
            if (skinnedMeshRenderer == null || skinnedMeshRenderer.sharedMesh == null)
                return Enumerable.Empty<(int Index, string Name)>();
            return BlendshapeIterator(skinnedMeshRenderer.sharedMesh);
        }

        private static IEnumerable<(int Index, string Name)> BlendshapeIterator(Mesh mesh)
        {
            int count = mesh.blendShapeCount;
            for(int i = 0; i < count; i++)
            {
                yield return (i, mesh.GetBlendShapeName(i));
            }
        }

        public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> enumerable, int count)
        {
            if (count <= 0)
                return enumerable.Skip(0);
            return SkipLastIterator(enumerable, count);
        }

        private static IEnumerable<T> SkipLastIterator<T>(IEnumerable<T> enumerable, int count)
        {
            var queue = SharedQueue<T>.Queue;
            queue.Clear();

            using (var enumerator = enumerable.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (queue.Count == count)
                    {
                        do
                        {
                            yield return queue.Dequeue();
                            queue.Enqueue(enumerator.Current);
                        }
                        while (enumerator.MoveNext());
                        break;
                    }
                    else
                    {
                        queue.Enqueue(enumerator.Current);
                    }
                }
            }
        }

        public static class SharedQueue<T>
        {
            public static readonly Queue<T> Queue = new Queue<T>();
        }
    }
}
