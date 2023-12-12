using UnityEditor;
using UnityEngine;

namespace gomoru.su.clothfire
{
    [CustomEditor(typeof(Configuration))]
    internal sealed class ConfigurationEditor : Editor
    {
        private Texture2D _infoIcon;
        private GUIStyle _style;

        internal void OnEnable()
        {
            _infoIcon = typeof(EditorGUIUtility).GetProperty("infoIcon", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).GetValue(null) as Texture2D;
        }

        public override void OnInspectorGUI()
        {
            if (_style == null)
            {
                _style = new GUIStyle(EditorStyles.helpBox)
                {
                    fontSize = 12
                };
            }
            EditorGUILayout.LabelField("The included MA Menu Intaller allows you to change the menu generation destination.".ToGUIContent(image: _infoIcon), _style);
            EditorGUILayout.LabelField("The name of the menu generated can be changed by renaming the attached MA Menu Item.".ToGUIContent(image: _infoIcon), _style);

            EditorGUILayout.Separator();

            serializedObject.Update();

            EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Configuration.GenerateGroupTogglePreset)));

            serializedObject.ApplyModifiedProperties();
        }

    }
}