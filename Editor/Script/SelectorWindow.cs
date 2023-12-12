using System;
using UnityEditor;
using UnityEngine;

namespace gomoru.su.clothfire
{
    internal abstract class SelectorWindow<T> : EditorWindow
    {
        public TemporaryMemory<T> Items { get; set; }
        public Action<T> Callback { get; set; }

        private string _searchText;
        private Vector2 _scrollPosition;

        internal void OnGUI()
        {
            var items = Items;
            if (items.IsEmpty)
                return;

            _searchText = EditorGUILayout.TextField(_searchText);

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            foreach (ref var x in items.Span)
            {
                if (!string.IsNullOrEmpty(_searchText))
                {
                    if (!OnFiltering(_searchText, ref x))
                        continue;
                }
                if (GUILayout.Button(GetGUIContent(ref x)))
                {
                    OnSelected(ref x);
                    Close();
                }
            }
            EditorGUILayout.EndScrollView();
        }

        protected static void Show(Rect position, TemporaryMemory<T> items, Action<T> callback, GUIContent title = null)
        {
            var window = CreateInstance<SelectorWindow<T>>();
            window.titleContent = title;
            window.Items = items;
            window.Callback = callback;
            window.position = new Rect(GUIUtility.GUIToScreenPoint(position.center), window.position.size);
            window.ShowAuxWindow();
        }

        protected virtual void OnSelected(ref T item)
        {
            Callback?.Invoke(item);
        }

        protected virtual bool OnFiltering(string query, ref T item) => true;

        protected abstract GUIContent GetGUIContent(ref T item);
    }
}
