using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace gomoru.su.clothfire
{
    internal sealed class BlendshapeSelector : EditorWindow
    {
        public Mesh TargetMesh { get; set; }

        private string[] _blendshapes;
        private Vector2 _scrollPosition;
        private string _searchText;

        public Action<string> Callback;

        public static void Show(Rect position, Mesh targetMesh, Action<string> callback)
        {
            if (targetMesh == null)
                return;
            var window = CreateInstance<BlendshapeSelector>();
            window.titleContent = new GUIContent($"Blendshape Selector: {targetMesh.name}");
            window.TargetMesh = targetMesh;
            window.Callback = callback;
            window.position = new Rect(GUIUtility.GUIToScreenPoint(position.center), window.position.size);
            window.ShowAuxWindow();
        }

        internal void OnGUI()
        {
            if (_blendshapes == null)
                _blendshapes = Enumerable.Range(0, TargetMesh.blendShapeCount).Select(i => TargetMesh.GetBlendShapeName(i)).ToArray();

            _searchText = EditorGUILayout.TextField(_searchText);

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            foreach (ref readonly var x in _blendshapes.AsSpan())
            {
                if (!string.IsNullOrEmpty(_searchText))
                {
                    if (x.IndexOf(_searchText) == -1)
                        continue;
                }
                if (GUILayout.Button(x.ToGUIContent()))
                {
                    Callback(x);
                    Close();
                }
            }
            EditorGUILayout.EndScrollView();
        }

        private struct Blendshape
        {
            public string Name;
            public int Index;
        }
    }
}
