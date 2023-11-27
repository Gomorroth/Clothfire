using System.Collections.Generic;
using nadena.dev.ndmf.util;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace gomoru.su.clothfire
{
    [CustomEditor(typeof(CostumeControlGenerator))]
    internal sealed class CostumeControlGeneratorEditor : Editor
    {
        private ReorderableList _itemList;
        private ReorderableList _additionalControlList;
        private SerializedProperty _itemsProperty;

        private SerializedProperty _additionalControls;

        private Dictionary<int, ReorderableList> _additionalControlLists;

        public void OnEnable()
        {
            foreach(var target in targets)
            {
                if (target is CostumeControlGenerator ccg)
                    ccg.TryRefleshItems();
            }

            _additionalControlLists = new Dictionary<int, ReorderableList>();

            _itemsProperty = serializedObject.FindProperty(nameof(CostumeControlGenerator.Items));
            _itemList = new ReorderableList(serializedObject, _itemsProperty)
            {
                displayAdd = false,
                displayRemove = false,
                draggable = true,
                elementHeightCallback = GetHeight,
                drawElementCallback = DrawItem,
                drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Items"),
            };
            _additionalControlList = new ReorderableList(serializedObject, null)
            {
                displayAdd = true,
                displayRemove = true,
                draggable = true,
                headerHeight = 0.5f,
                elementHeightCallback = idx => AdditionalControlDrawer.Height,
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => EditorGUI.PropertyField(rect, _additionalControlList.serializedProperty.GetArrayElementAtIndex(index)),
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            _itemList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        public float GetHeight(int index)
        {
            var item = _itemsProperty.GetArrayElementAtIndex(index);
            float height = EditorGUIUtility.singleLineHeight;
            if (item.isExpanded)
            {
                height += EditorGUIUtility.singleLineHeight * 2;
                var list = item.FindPropertyRelative(nameof(ClothItem.AdditionalControls));
                if (list.isExpanded)
                {
                    height += EditorGUIUtility.singleLineHeight;
                    var count = list.arraySize;
                    if (count == 0)
                        height += EditorGUIUtility.singleLineHeight;

                    for (int i = 0; i < count; i++)
                    {
                        height += AdditionalControlDrawer.Height;
                    }
                    height += EditorGUIUtility.singleLineHeight;
                }

            }
            height += 4; // Margin
            return height;
        }

        public void DrawItem(Rect rect, int index, bool isActive, bool isFocused)
        {
            var item = _itemsProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            var path = item.FindPropertyRelative(nameof(ClothItem.Path)).stringValue;
            var isInclude = item.FindPropertyRelative(nameof(ClothItem.IsInclude));
            var isActiveByDefault = item.FindPropertyRelative(nameof(ClothItem.IsActiveByDefault));
            var obj = (target as CostumeControlGenerator).gameObject;
            var renderer = obj.transform.Find(path)?.GetComponent<Renderer>();
            if (renderer == null)
            {
                EditorGUI.ObjectField(rect, null, typeof(Renderer), false);
                return;
            }

            var itemRect = rect;
            itemRect.height = EditorGUIUtility.singleLineHeight;
            itemRect.x += 12;
            itemRect.width -= 12;

            var activeCheckRect = itemRect;
            activeCheckRect.width = EditorStyles.toggle.CalcSize(GUIContent.none).x;

            var rendererRect = itemRect;
            rendererRect.x += activeCheckRect.width + 2;
            rendererRect.width -= activeCheckRect.width + 2;

            EditorGUI.BeginDisabledGroup(!isInclude.boolValue);
            isActiveByDefault.boolValue = EditorGUI.ToggleLeft(activeCheckRect, GUIContent.none, isActiveByDefault.boolValue);
            EditorGUI.ObjectField(rendererRect, renderer.gameObject, typeof(GameObject), false);
            EditorGUI.EndDisabledGroup();

            if (item.isExpanded = EditorGUI.Foldout(itemRect, item.isExpanded, GUIContent.none, true))
            {
                //EditorGUI.indentLevel++;
                var lineRect = rect;
                lineRect.height = EditorGUIUtility.singleLineHeight;
                lineRect.y += EditorGUIUtility.singleLineHeight;
                lineRect.x += 12;
                lineRect.width -= 12;

                var checkBoxRect = lineRect;
                checkBoxRect.width /= 2;
                isInclude.boolValue = EditorGUI.ToggleLeft(checkBoxRect, "Include", isInclude.boolValue);

                EditorGUI.BeginDisabledGroup(!isInclude.boolValue);

                checkBoxRect.x += checkBoxRect.width;
                isActiveByDefault.boolValue = EditorGUI.ToggleLeft(checkBoxRect, "Active", isActiveByDefault.boolValue);

                var listRect = lineRect;
                listRect.y += EditorGUIUtility.singleLineHeight;
                _additionalControls = item.FindPropertyRelative(nameof(ClothItem.AdditionalControls));
                listRect.x += 12;
                listRect.width -= 12;
                if (_additionalControls.isExpanded = EditorGUI.Foldout(listRect, _additionalControls.isExpanded, "Additional Controls", true))
                {
                    listRect.y += EditorGUIUtility.singleLineHeight;

                    if (!_additionalControlLists.TryGetValue(index, out var list))
                    {
                        list = new ReorderableList(serializedObject, _additionalControls)
                        {
                            displayAdd = true,
                            displayRemove = true,
                            draggable = true,
                            headerHeight = 0.5f,
                            elementHeightCallback = idx => AdditionalControlDrawer.Height,
                            drawElementCallback = (Rect rect2, int index2, bool isActive2, bool isFocused2) => EditorGUI.PropertyField(rect2, _additionalControls.GetArrayElementAtIndex(index2)),
                        };
                        _additionalControlLists.Add(index, list);
                    }

                    list.DoList(listRect);
                }

                EditorGUI.EndDisabledGroup();

                //EditorGUI.indentLevel--;
            }
        }

        [MenuItem("CONTEXT/CostumeControlGenerator/Sort Items")]
        public static void SortItems(MenuCommand command)
        {
            var generator = command.context as CostumeControlGenerator;
            var obj = generator.gameObject.GetRootObject();
            generator.Items.Sort((x, y) => obj.Find(x.Path).transform.GetSiblingIndex() - obj.Find(y.Path).transform.GetSiblingIndex());
            generator.MarkDirty();
        }
    }
}