﻿using System;
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

        public static void GroupField(SerializedProperty property, Func<GameObject> avatarRootObject, GUIContent label = null, GroupType type = GroupType.All)
        {
            var rect = EditorGUILayout.GetControlRect(label != null, EditorGUIUtility.singleLineHeight);
            GroupField(rect, property, avatarRootObject, label, type);
        }

        public static void GroupField(Rect position, SerializedProperty property, Func<GameObject> avatarRootObject, GUIContent label = null, GroupType type = GroupType.All)
        {
            if (label == null)
                label = property.displayName.ToGUIContent();

            label = EditorGUI.BeginProperty(position, label, property);

            var field = position;
            if (label != GUIContent.none)
            {
                var labelRect = position;
                labelRect.width = EditorGUIUtility.labelWidth;
                field.x += labelRect.width + 2f;
                field.width -= labelRect.width + 2f;
                EditorGUI.LabelField(labelRect, label);
            }

            var buttonRect = field;
            buttonRect.x += buttonRect.width - EditorGUIUtility.singleLineHeight;
            buttonRect.width = EditorGUIUtility.singleLineHeight;
            EditorGUIUtility.AddCursorRect(buttonRect, MouseCursor.Arrow);
            buttonRect = ObjectFieldButtonStyle.margin.Remove(buttonRect);
            if (GUI.Button(buttonRect, GUIContent.none, GUIStyle.none))
            {
                GroupSelector.Show(buttonRect, avatarRootObject(), x => { property.stringValue = x; property.serializedObject.ApplyModifiedProperties(); }, type);
            }
            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
            EditorGUI.PropertyField(field, property, GUIContent.none);
            EditorGUI.showMixedValue = false;
            if (Event.current.type == EventType.Repaint)
            {
                ObjectFieldButtonStyle.Draw(buttonRect, GUIContent.none, 0, true, buttonRect.Contains(Event.current.mousePosition));
            }

            EditorGUI.EndProperty();
        }
    }
}