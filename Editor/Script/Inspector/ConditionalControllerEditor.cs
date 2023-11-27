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
    internal sealed class ConditionalControllerEditor : Editor
    {
        private ReorderableList _conditionList;
        private ReorderableList _controlList;

        private string[] _targets;
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
                    var path = prop.FindPropertyRelative(nameof(ConditionalController.Condition.Path));
                    var state = prop.FindPropertyRelative(nameof(ConditionalController.Condition.State));

                    int idx = _targets.AsSpan(0, _conditions.Length).IndexOf(x => x == path.stringValue);

                    var rect1 = rect;
                    var rect2 = rect;
                    rect1.width *= 0.8f;
                    rect2.width -= rect1.width + 4;
                    rect2.x += rect1.width + 4;

                    EditorGUI.BeginChangeCheck();
                    idx = EditorGUI.Popup(rect1, idx, _conditions);
                    if (EditorGUI.EndChangeCheck())
                    {
                        path.stringValue = _targets[idx];
                    }

                    state.boolValue = EditorGUI.Popup(rect2, state.boolValue ? 1 : 0, ONOFF) != 0;
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

            ArrayPool<string>.Shared.Return(_targets);
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
            var targets = ArrayPool<string>.Shared.Rent(items.Length);
            for(int i = 0; i < items.Length; i++)
            {
                var target = avatarRootObject.Find(items[i].Path);
                targets[i] = items[i].Path;
                string name = items[i].Parent is IControlGroup ? $"{items[i].Parent.name}/{target.name}" : $"{target.name}";
                contents[i] = new GUIContent(name);
            }
            _conditions = contents;
            _targets = targets;
        }
    }
}