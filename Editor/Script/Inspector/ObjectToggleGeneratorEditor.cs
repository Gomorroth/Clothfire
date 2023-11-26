using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace gomoru.su.clothfire
{
    [CustomEditor(typeof(ObjectToggleGenerator))]
    internal sealed class ObjectToggleGeneratorEditor : Editor
    {
        private ReorderableList _additionalControlList;

        public void OnEnable()
        {
            _additionalControlList = new ReorderableList(serializedObject, serializedObject.FindProperty(nameof(ObjectToggleGenerator.AdditionalControls)))
            {
                displayAdd = true,
                displayRemove = true,
                draggable = true,
                drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Additional Controls"),
                elementHeightCallback = idx => AdditionalControlDrawer.Default.GetPropertyHeight(null, null),
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => EditorGUI.PropertyField(rect, _additionalControlList.serializedProperty.GetArrayElementAtIndex(index)),
            };
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(ObjectToggleGenerator.IsActiveByDefault)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(ObjectToggleGenerator.Group)));
            _additionalControlList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
