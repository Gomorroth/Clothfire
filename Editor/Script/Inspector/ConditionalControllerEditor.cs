using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace gomoru.su.clothfire
{
    [CustomEditor(typeof(ConditionalController))]
    [CanEditMultipleObjects]
    internal sealed class ConditionalControllerEditor : Editor
    {
        private ReorderableList _conditionList;
        private ReorderableList _controlList;

        private GameObject[] _targets;
        private GUIContent[] _conditions;
        private static readonly GUIContent[] ONOFF = new[] { new GUIContent("OFF"), new GUIContent("ON") };

        internal void OnEnable()
        {
            ConstructConditions((target as Component).gameObject.GetRootObject());

            _conditionList = new ReorderableList(serializedObject, serializedObject.FindProperty(nameof(ConditionalController.Conditions)))
            {
                drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Conditions"),
                drawElementCallback = (rect, index, active, focused) =>
                {
                    var prop = _conditionList.serializedProperty.GetArrayElementAtIndex(index);
                    var path = prop.FindPropertyRelative(nameof(ConditionalController.Condition.Object));
                    var state = prop.FindPropertyRelative(nameof(ConditionalController.Condition.State));

                    int idx = _targets.AsSpan(0, _conditions.Length).IndexOf(x => x == path.objectReferenceValue);

                    var rect1 = rect;
                    var rect2 = rect;
                    rect1.width *= 0.8f;
                    rect2.width -= rect1.width + 4;
                    rect2.x += rect1.width + 4;

                    EditorGUI.BeginChangeCheck();
                    EditorGUI.showMixedValue = path.hasMultipleDifferentValues;
                    idx = EditorGUI.Popup(rect1, idx, _conditions);
                    if (EditorGUI.EndChangeCheck())
                    {
                        path.objectReferenceValue = _targets[idx];
                    }

                    EditorGUI.BeginChangeCheck();
                    EditorGUI.showMixedValue = state.hasMultipleDifferentValues;
                    var flag = EditorGUI.Popup(rect2, state.boolValue ? 1 : 0, ONOFF) != 0;
                    if (EditorGUI.EndChangeCheck())
                    {
                        state.boolValue = flag;
                    }
                    EditorGUI.showMixedValue = false;
                },
            };
            _controlList = new ReorderableList(serializedObject, serializedObject.FindProperty(nameof(ConditionalController.Controls)))
            {
                drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Controls"),
                elementHeightCallback = index => AdditionalControlDrawer.Height,
                drawElementCallback = (rect, index, active, focused) =>
                {
                    EditorGUI.PropertyField(rect, _controlList.serializedProperty.GetArrayElementAtIndex(index));
                }
            };
        }

        internal void OnDestroy()
        {
            if (_targets is null)
                return;

            ArrayPool<GameObject>.Shared.Return(_targets);
            _targets = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            _conditionList.DoLayoutList();
            _controlList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        private void ConstructConditions(GameObject avatarRootObject)
        {
            var items = ControlTarget.GetControlTargets(avatarRootObject);
            var contents = new GUIContent[items.Length];
            var targets = ArrayPool<GameObject>.Shared.Rent(items.Length);
            for(int i = 0; i < items.Length; i++)
            {
                targets[i] = avatarRootObject.Find(items[i].Path);
                string name = items[i].ToParameterName(avatarRootObject);
                contents[i] = new GUIContent(name);
            }
            _conditions = contents;
            _targets = targets;
        }
    }
}