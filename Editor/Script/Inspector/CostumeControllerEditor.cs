using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace gomoru.su.clothfire
{
    [CustomEditor(typeof(CostumeController))]
    internal sealed class CostumeControllerEditor : Editor
    {
        private ReorderableList _itemList;
        private ReorderableList _additionalControlList;
        private SerializedProperty _itemsProperty;

        private SerializedProperty _additionalControls;

        public void OnEnable()
        {
            foreach (var target in targets)
            {
                if (target is CostumeController ccg)
                    ccg.TryRefleshItems();
            }

            _itemsProperty = serializedObject.FindProperty(nameof(CostumeController.Items));
            _itemList = new ReorderableList(serializedObject, _itemsProperty)
            {
                displayAdd = false,
                displayRemove = false,
                draggable = true,
                footerHeight = 0,
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
            var obj = (target as CostumeController).gameObject;
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
            GUIUtils.ToggleLeft(activeCheckRect, isActiveByDefault, GUIContent.none);
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
                checkBoxRect.width /= 4;
                GUIUtils.ToggleLeft(checkBoxRect, isInclude, "Include".ToGUIContent());

                EditorGUI.BeginDisabledGroup(!isInclude.boolValue);

                checkBoxRect.x += checkBoxRect.width;
                GUIUtils.ToggleLeft(checkBoxRect, isActiveByDefault, "Active".ToGUIContent());

                var settings = item.FindPropertyRelative(nameof(ClothItem.ParameterSettings));
                var save = settings.FindPropertyRelative(nameof(ParameterSettings.IsSave));
                var localOnly = settings.FindPropertyRelative(nameof(ParameterSettings.IsLocal));

                checkBoxRect.x += checkBoxRect.width;
                GUIUtils.ToggleLeft(checkBoxRect, save, "Save".ToGUIContent());
                checkBoxRect.x += checkBoxRect.width;
                GUIUtils.ToggleLeft(checkBoxRect, localOnly, "Local Only".ToGUIContent());

                var listRect = lineRect;
                listRect.y += EditorGUIUtility.singleLineHeight;
                _additionalControls = item.FindPropertyRelative(nameof(ClothItem.AdditionalControls));
                listRect.x += 12;
                listRect.width -= 12;
                if (_additionalControls.isExpanded = EditorGUI.Foldout(listRect, _additionalControls.isExpanded, "Additional Controls", true))
                {
                    listRect.y += EditorGUIUtility.singleLineHeight;

                    _additionalControlList.serializedProperty = _additionalControls;
                    _additionalControlList.DoList(listRect);
                }

                EditorGUI.EndDisabledGroup();

                //EditorGUI.indentLevel--;
            }
        }

        [MenuItem("CONTEXT/CostumeController/Sort Items")]
        public static void SortItems(MenuCommand command)
        {
            var generator = command.context as CostumeController;
            var obj = generator.gameObject.GetRootObject();
            generator.Items.Sort((x, y) => obj.Find(x.Path).transform.GetSiblingIndex() - obj.Find(y.Path).transform.GetSiblingIndex());
            generator.MarkDirty();
        }
    }
}