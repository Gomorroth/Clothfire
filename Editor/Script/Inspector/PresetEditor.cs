using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using Object = UnityEngine.Object;

namespace gomoru.su.clothfire
{
    [CustomEditor(typeof(Preset))]
    internal sealed class PresetEditor : Editor
    {
        private VRCAvatarDescriptor _avatar;
        private ReorderableList _presetList;

        internal void OnEnable()
        {
            var preset = target as Preset;
            _avatar = preset?.GetComponentInParent<VRCAvatarDescriptor>();
            if (preset == null || _avatar == null)
                return;

            preset.TryRefleshItems();

            _presetList = new ReorderableList(serializedObject, serializedObject.FindProperty(nameof(Preset.Targets)))
            {
                displayAdd = false,
                displayRemove = false,
                drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Items"),
                elementHeightCallback = (index) => EditorGUIUtility.singleLineHeight * (_presetList.serializedProperty.GetArrayElementAtIndex(index).isExpanded ? 2 : 1) + 4,
                drawElementCallback = (position, index, _1, _2) =>
                {
                    var prop = _presetList.serializedProperty.GetArrayElementAtIndex(index);
                    _ = EditorGUI.BeginProperty(position, GUIContent.none, prop);
                    position.height = EditorGUIUtility.singleLineHeight;
                    position.y += 2;
                    position.x += 12;
                    position.width -= 12;
                    var checkRect = position;
                    var fieldRect = position;

                    checkRect.width = EditorStyles.toggle.CalcSize(GUIContent.none).x;
                    fieldRect.x += checkRect.width + 2;
                    fieldRect.width -= checkRect.width + 2;

                    var incldue = prop.FindPropertyRelative(nameof(Preset.PresetItem.Include));
                    var active = prop.FindPropertyRelative(nameof(Preset.PresetItem.Active));
                    var target = prop.FindPropertyRelative(nameof(Preset.PresetItem.Target));
                    var parent = prop.FindPropertyRelative(nameof(Preset.PresetItem.Parent)).objectReferenceValue;
                    var obj = target.objectReferenceValue as GameObject;

                    EditorGUI.BeginDisabledGroup(!incldue.boolValue);
                    active.boolValue = EditorGUI.ToggleLeft(checkRect, GUIContent.none, active.boolValue);
                    if (parent is IControlGroup group && !string.IsNullOrEmpty(group.GroupName))
                    {
                        var rect = fieldRect;
                        rect.width *= 0.2f;
                        fieldRect.x += rect.width + 4;
                        fieldRect.width -= rect.width + 4;

                        if (group.GroupMaster is GameObject master)
                        {
                            if (GUI.Button(rect, GUIContent.none, EditorStyles.objectField))
                            {
                                EditorGUIUtility.PingObject(master);
                            }
                            if (Event.current.type == EventType.Repaint)
                            {
                                var r = rect;
                                r.height -= 6f;
                                r.y += 4f;
                                r.x += 2;
                                EditorStyles.label.Draw(r, group.GroupName.ToGUIContent(image: AssetPreview.GetMiniTypeThumbnail(typeof(GameObject))), 0);
                            }
                        }
                        else
                        {
                            GUI.Label(rect, group.GroupName, EditorStyles.objectField);
                        }
                    }
                    EditorGUI.ObjectField(fieldRect, obj, typeof(GameObject), true);
                    EditorGUI.EndDisabledGroup();

                    if (prop.isExpanded = EditorGUI.Foldout(position, prop.isExpanded, GUIContent.none, true))
                    {
                        var checkBoxRect = position;
                        checkBoxRect.y += EditorGUIUtility.singleLineHeight;
                        checkBoxRect.width /= 2;
                        incldue.boolValue = EditorGUI.ToggleLeft(checkBoxRect, "Include", incldue.boolValue);

                        EditorGUI.BeginDisabledGroup(!incldue.boolValue);

                        checkBoxRect.x += checkBoxRect.width;
                        active.boolValue = EditorGUI.ToggleLeft(checkBoxRect, "Active", active.boolValue);
                        EditorGUI.EndDisabledGroup();
                    }

                    EditorGUI.EndProperty();
                }
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            _presetList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
