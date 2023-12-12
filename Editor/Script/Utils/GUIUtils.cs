using UnityEditor;
using UnityEngine;
using static VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu;

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

        public static void GroupField(SerializedProperty property, GameObject avatarRootObject, GUIContent label = null)
        {
            var rect = EditorGUILayout.GetControlRect(label != null, EditorGUIUtility.singleLineHeight);
            GroupField(rect, property, avatarRootObject, label);
        }

        public static void GroupField(Rect position, SerializedProperty property, GameObject avatarRootObject, GUIContent label = null)
        {
            var left = position;
            var buttonRect = left;
            buttonRect.x += buttonRect.width - EditorGUIUtility.singleLineHeight;
            buttonRect.width = EditorGUIUtility.singleLineHeight;
            EditorGUIUtility.AddCursorRect(buttonRect, MouseCursor.Arrow);
            buttonRect = GUIUtils.ObjectFieldButtonStyle.margin.Remove(buttonRect);
            if (GUI.Button(buttonRect, GUIContent.none, GUIStyle.none))
            {
                GroupSelector.Show(buttonRect, avatarRootObject, x => { property.stringValue = x; property.serializedObject.ApplyModifiedProperties(); });
            }
            EditorGUI.PropertyField(left, property, GUIContent.none);
            if (Event.current.type == EventType.Repaint)
            {
                ObjectFieldButtonStyle.Draw(buttonRect, GUIContent.none, 0, true, buttonRect.Contains(Event.current.mousePosition));
            }
        }
    }
}