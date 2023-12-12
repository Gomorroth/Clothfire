using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace gomoru.su.clothfire
{
    [InitializeOnLoad]
    internal static class ClothfireCore
    {
        static ClothfireCore()
        {
            EditorApplication.hierarchyChanged += default(object).OnHierarchyChangedCallback;
        }

        private static void OnHierarchyChangedCallback(this object _)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            foreach (var component in Object.FindObjectsOfType<Component>().Where(x => x is IHierarchyChangedCallback))
            {
                (component as IHierarchyChangedCallback).OnHierarchyChanged();
            }
        }
    }
}
