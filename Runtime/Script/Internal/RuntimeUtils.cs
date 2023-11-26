using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace gomoru.su.clothfire
{
    internal static class RuntimeUtils
    {
        public static TemporaryBuffer<T> GetChildrenComponents<T>(this GameObject gameObject, bool includeInactive = false) where T : Component
        {
            var components = gameObject.GetComponentsInChildren<T>(includeInactive);
            var buffer = ArrayPool<T>.Shared.Rent(components.Length);
            int count = 0;

            _ = buffer.Length;

            foreach( var component in components )
            {
                if (component.transform.parent == gameObject.transform)
                {
                    buffer[count++] = component;
                }
            }

            return new TemporaryBuffer<T>(buffer, count);
        }

        public static void MarkDirty(this Object obj)
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(obj);
#endif
        }

        public static bool TryGetGUIDAndLocalId(this Object obj, out string guid, out long localId)
        {
#if UNITY_EDITOR
            if (obj == null)
            {
                (guid, localId) = (null, 0);
                return false;
            }
            return UnityEditor.AssetDatabase.TryGetGUIDAndLocalFileIdentifier(obj, out guid, out localId);
#else
            (guid, localId) = (null, 0);
            return false;
#endif
        }

        private static string[] _relativePathBuffer = new string[256];

        public static string GetRelativePath(this GameObject gameObject, GameObject relativeTo)
        {
            if (relativeTo == gameObject) return "";

            var buffer = _relativePathBuffer;
            int i = buffer.Length - 1;

            while (gameObject != relativeTo && gameObject != null)
            {
                if (i < 0)
                {
                    var newBuffer = new string[buffer.Length * 2];
                    buffer.CopyTo(newBuffer.AsSpan(buffer.Length));
                    i += buffer.Length;
                    _relativePathBuffer = buffer = newBuffer;
                }
                buffer[i--] = gameObject.name;
                gameObject = gameObject.transform.parent?.gameObject;
            }

            if (gameObject == null && relativeTo != null) return null;

            i++;
            return string.Join("/", buffer, i, buffer.Length - i);
        }

        public static GameObject GetRootObject(this GameObject gameObject)
        {
            if (gameObject == null) 
                return null;

            var tr = gameObject.transform;
            while(tr.parent != null)
            {
                tr = tr.parent;
            }
            return tr.gameObject;
        }

        public static GameObject Find(this GameObject obj, string path)
        {
            return obj?.transform.Find(path)?.gameObject;
        }

        public static bool Contains<T>(this List<T> list, Predicate<T> condition)
        {
            foreach (var item in list.AsSpan())
            {
                if (condition(item))
                    return true;
            }
            return false;
        }

        public static Span<T> AsSpan<T>(this List<T> list)
        {
            var dummy = Unsafe.As<DummyList<T>>(list);
            return dummy.Array.AsSpan(0, list.Count);
        }

        private sealed class DummyList<T>
        {
            public T[] Array;
            public int Count;
        }
    }

    public readonly ref struct TemporaryBuffer<T>
    {
        private readonly T[] _array;
        public readonly Span<T> Span;

        public TemporaryBuffer(T[] array, int count)
        {
            _array = array;
            Span = array.AsSpan(0, count);
        }

        public void Dispose()
        {
            ArrayPool<T>.Shared.Return(_array);
        }
    }
}
