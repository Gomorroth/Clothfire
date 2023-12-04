using UnityEditor;
using UnityEngine;

namespace gomoru.su.clothfire
{
    internal static class GUIUtils
    {
        private static readonly GUIContent _tempContent = new GUIContent();
        public static readonly GUIStyle ObjectFieldButtonStyle = typeof(EditorStyles).GetProperty("objectFieldButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).GetMethod.Invoke(null, null) as GUIStyle;

        public static GUIContent GetTempContent(string label, string toolTip = null, Texture image = null)
        {
            var content = _tempContent;
            content.text = label;
            content.tooltip = toolTip;
            content.image = image;
            return content;
        }

        public static GUIContent ToGUIContent(this string text, string toolTip = null, Texture image = null) => GetTempContent(text, toolTip, image);

        public static bool ToggleLeft(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            var showMixedValue = EditorGUI.showMixedValue;
            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
            var value = EditorGUI.ToggleLeft(position, label, property.boolValue);
            EditorGUI.showMixedValue = showMixedValue;
            if (EditorGUI.EndChangeCheck())
            {
                property.boolValue = value;
                return true;
            }
            return false;
        }
    }
}