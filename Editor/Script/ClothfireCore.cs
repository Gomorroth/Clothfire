using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

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

            foreach(var component in GameObject.FindObjectsOfType<CostumeControlGenerator>())
            {
                component.TryRefleshItems();
            }
        }
    }
}
