using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace gomoru.su.clothfire
{
    [CustomEditor(typeof(ObjectToggle))]
    [CanEditMultipleObjects]
    internal sealed class ObjectToggleEditor : Editor
    {
        private ReorderableList _additionalControlList;

        public void OnEnable()
        {
            _additionalControlList = new ReorderableList(serializedObject, serializedObject.FindProperty(nameof(ObjectToggle.AdditionalControls)))
            {
                displayAdd = true,
                displayRemove = true,
                draggable = true,
                drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Additional Controls"),
                elementHeightCallback = idx => AdditionalControlDrawer.Height,
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => EditorGUI.PropertyField(rect, _additionalControlList.serializedProperty.GetArrayElementAtIndex(index)),
            };
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(ObjectToggle.IsActiveByDefault)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(ObjectToggle.ParameterSettings)).FindPropertyRelative(nameof(ParameterSettings.IsSave)), "Save Parameter".ToGUIContent());
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(ObjectToggle.ParameterSettings)).FindPropertyRelative(nameof(ParameterSettings.IsLocal)), "Local Only".ToGUIContent());
            GUIUtils.GroupField(serializedObject.FindProperty(nameof(ObjectToggle.Group)), () => (target as Component).gameObject.GetRootObject(), type: GroupType.Control);
            _additionalControlList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
