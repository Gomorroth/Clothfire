using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
    [CanEditMultipleObjects]
    internal sealed class PresetEditor : Editor
    {
        private VRCAvatarDescriptor _avatar;
        private ReorderableList _presetList;

        private static bool _showToggleByGroup;
        private static bool _showSyncHierarchy;

        internal void OnEnable()
        {
            _avatar = (target as Preset)?.GetComponentInParent<VRCAvatarDescriptor>();
            if (_avatar == null)
                return;

            foreach(var target in targets)
            {
                var preset = target as Preset;

                preset.TryRefleshItems();

                if (string.IsNullOrEmpty(preset.PresetName))
                {
                    preset.PresetName = preset.name;
                }
            }

            _presetList = new ReorderableList(serializedObject, serializedObject.FindProperty(nameof(Preset.Targets)))
            {
                displayAdd = false,
                displayRemove = false,
                footerHeight = 0,
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
                    GUIUtils.ToggleLeft(checkRect, active, GUIContent.none);
                    if (parent is IControlGroup group && !string.IsNullOrEmpty(group.GroupName))
                    {
                        var rect = fieldRect;
                        rect.width *= 0.2f;
                        fieldRect.x += rect.width + 4;
                        fieldRect.width -= rect.width + 4;
                        DrawGroupMaster(rect, group);
                    }
                    EditorGUI.PropertyField(fieldRect, target, GUIContent.none);
                    EditorGUI.EndDisabledGroup();

                    if (prop.isExpanded = EditorGUI.Foldout(position, prop.isExpanded, GUIContent.none, true))
                    {
                        var checkBoxRect = position;
                        checkBoxRect.y += EditorGUIUtility.singleLineHeight;
                        checkBoxRect.width /= 2;
                        GUIUtils.ToggleLeft(checkBoxRect, incldue, "Include".ToGUIContent());

                        EditorGUI.BeginDisabledGroup(!incldue.boolValue);

                        checkBoxRect.x += checkBoxRect.width;
                        GUIUtils.ToggleLeft(checkBoxRect, active, "Active".ToGUIContent());
                        EditorGUI.EndDisabledGroup();
                    }

                    EditorGUI.EndProperty();
                }
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Preset.PresetName)), "Name".ToGUIContent());
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Preset.Group)));

            _presetList.DoLayoutList();

            EditorGUILayout.Space(4);

            if (serializedObject.targetObjects.Length <= 1)
            {

                EditorGUILayout.LabelField("Utilities", EditorStyles.boldLabel);

                ToggleByGroup();
                SyncHierarchy();

            }

            serializedObject.ApplyModifiedProperties();
        }

        private void ToggleByGroup()
        {
            if (_showToggleByGroup = EditorGUILayout.BeginFoldoutHeaderGroup(_showToggleByGroup, "Toggle by Group"))
            {
                var preset = target as Preset;
                foreach (var group in preset.Targets.GroupBy(x => (x.Parent as IControlGroup)?.GroupName ?? string.Empty).OrderBy(x => string.IsNullOrEmpty(x.Key)).ThenBy(x => x.Key))
                {
                    bool? include = NullIfDifference(group.Select(x => x.Include));
                    bool? active = NullIfDifference(group.Select(x => x.Active));

                    var rect = EditorGUILayout.GetControlRect();
                    var checkRect = rect;
                    checkRect.width = EditorStyles.toggle.CalcSize(GUIContent.none).x;
                    rect.width -= checkRect.width + 2;
                    rect.x += checkRect.width + 2;

                    EditorGUI.showMixedValue = !include.HasValue;
                    EditorGUI.BeginChangeCheck();
                    var included = EditorGUI.Toggle(checkRect, include ?? true);
                    bool includeChanged = EditorGUI.EndChangeCheck();

                    checkRect.x += checkRect.width + 2;
                    rect.width -= checkRect.width + 2;
                    rect.x += checkRect.width + 2;

                    EditorGUI.showMixedValue = !active.HasValue;
                    EditorGUI.BeginChangeCheck();
                    var actived = EditorGUI.Toggle(checkRect, active ?? true);
                    bool activeChanged = EditorGUI.EndChangeCheck();

                    if (group.FirstOrDefault().Parent is IControlGroup gr && !string.IsNullOrEmpty(gr.GroupName))
                        DrawGroupMaster(rect, gr);
                    else
                        GUI.Label(rect, "Other", EditorStyles.objectField);

                    if (activeChanged || includeChanged)
                    {
                        foreach (ref var x in preset.Targets.AsSpan())
                        {
                            if (x.Parent is IControlGroup g && g.GroupName == group.Key)
                            {
                                if (activeChanged)
                                    x.Active = actived;
                                if (includeChanged)
                                    x.Include = included;
                            }
                        }
                        EditorUtility.SetDirty(target);
                    }
                    EditorGUI.showMixedValue = false;
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void SyncHierarchy()
        {
            if (_showSyncHierarchy = EditorGUILayout.BeginFoldoutHeaderGroup(_showSyncHierarchy, "Sync Hierarchy"))
            {
                bool toHierarchy = GUILayout.Button("Apply to Hierarchy from Preset");
                bool toPreset = GUILayout.Button("Apply to Preset from Hierarchy");

                if (toHierarchy || toPreset)
                {
                    var preset = target as Preset;

                    foreach(ref var x in preset.Targets.AsSpan())
                    {
                        if (toHierarchy)
                        {
                            x.Target.SetActive(x.Active);
                        }
                        else
                        {
                            x.Active = x.Target.activeInHierarchy;
                        }
                    }
                }

            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private static T? NullIfDifference<T>(IEnumerable<T> enumerable) where T : struct
        {
            T? value = null;
            foreach (var item in enumerable)
            {
                if (!value.HasValue)
                {
                    value = item;
                }
                else if (!EqualityComparer<T>.Default.Equals(value.Value, item))
                {
                    return null;
                }
            }
            return value;
        }

        private static void DrawGroupMaster(Rect rect, IControlGroup group)
        {
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
    }
}
