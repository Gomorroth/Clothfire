using System;
using UnityEngine;

namespace gomoru.su.clothfire
{
    internal sealed class BlendshapeSelector : StringSelectorWindow<BlendshapeSelector>
    {
        public static void Show(Rect position, Mesh targetMesh, Action<string> callback)
        {
            if (targetMesh == null)
                return;

            var items = TemporaryMemory<string>.Allocate(targetMesh.blendShapeCount);
            var span = items.Span;
            for(int i = 0; i < span.Length; i++)
            {
                span[i] = targetMesh.GetBlendShapeName(i);
            }

            Show(position, items, callback, new GUIContent($"Select blendshape: {targetMesh.name}"));
        }

        protected override GUIContent GetGUIContent(ref string item) => item.ToGUIContent();
    }
}
