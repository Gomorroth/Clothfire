using System;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace gomoru.su.clothfire
{
    internal abstract class SelectorWindow<T, TSelf> : EditorWindow where TSelf : SelectorWindow<T, TSelf>
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
            var window = CreateInstance<TSelf>();
            if (title == null)
                title = GUIContent.none;
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

    internal abstract class StringSelectorWindow<TSelf> : SelectorWindow<string, TSelf> where TSelf : StringSelectorWindow<TSelf>
    {
        private Regex _regex;
        private string _cachedQuery;

        protected override bool OnFiltering(string query, ref string item)
        {
            if (_cachedQuery != query)
            {
                _cachedQuery = query;
                try
                {
                    _regex = new Regex(query, RegexOptions.IgnoreCase);
                }
                catch
                {
                    _regex = null;
                }
            }
            if (_regex is null)
                return false;
            return _regex.Match(item).Success;
        }
    }
}
