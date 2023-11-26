using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace gomoru.su.clothfire
{
    [CustomPropertyDrawer(typeof(AdditionalControl))]
    internal sealed class AdditionalControlDrawer : PropertyDrawer
    {
        public static readonly AdditionalControlDrawer Default = new AdditionalControlDrawer();
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float lineCount = 4;
            float height = EditorGUIUtility.singleLineHeight * lineCount;
            height += 4 * (lineCount - 1);
            height += 8f;
            return height;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var type = property.FindPropertyRelative(nameof(AdditionalControl.Type));
            position.height = EditorGUIUtility.singleLineHeight;
            position.y += 4f;
            var isAbsolute = property.FindPropertyRelative(nameof(AdditionalControl.IsAbsolute));

            var absoluteCheckRect = position;
            absoluteCheckRect.width = EditorStyles.toggle.CalcSize(GUIUtils.GetTempContent("Absolute")).x;

            var enumRect = position;
            enumRect.width -= absoluteCheckRect.width;

            absoluteCheckRect.x += enumRect.width;
            enumRect.width -= 8f;

            EditorGUI.PropertyField(enumRect, type, GUIContent.none);
            isAbsolute.boolValue = EditorGUI.ToggleLeft(absoluteCheckRect, "Absolute", isAbsolute.boolValue);

            if ((AdditionalControl.ControlType)type.enumValueIndex == AdditionalControl.ControlType.Blendshape)
            {
                position.y += EditorGUIUtility.singleLineHeight + 4;
                var control = property.FindPropertyRelative(nameof(AdditionalControl.Blendshape));
                var path = control.FindPropertyRelative(nameof(BlendshapeControl.Path));

                var root = (property.serializedObject.targetObject as Component).transform;
                if (isAbsolute.boolValue)
                    root = nadena.dev.ndmf.runtime.RuntimeUtil.FindAvatarInParents(root.transform);

                var renderer = root.transform.Find(path.stringValue)?.GetComponent<Renderer>();

                if (!string.IsNullOrEmpty(path.stringValue) && renderer == null)
                {
                    EditorGUI.PropertyField(position, path, GUIContent.none);
                }
                else
                {
                    EditorGUI.BeginChangeCheck();
                    renderer = EditorGUI.ObjectField(position, GUIContent.none, renderer, typeof(Renderer), true) as Renderer;
                    if (EditorGUI.EndChangeCheck())
                    {
                        var objPath = renderer.gameObject.GetRelativePath(root.gameObject);
                        if (objPath == null) // Not found
                        {
                            if (!isAbsolute.boolValue)
                            {
                                var avatarRoot = nadena.dev.ndmf.runtime.RuntimeUtil.FindAvatarInParents(root.transform);
                                objPath = renderer.gameObject.GetRelativePath(avatarRoot.gameObject);
                                if (objPath != null) // Found in avatar
                                {
                                    isAbsolute.boolValue = true;
                                    path.stringValue = objPath;
                                }
                            }
                        }
                        else
                        {
                            path.stringValue = objPath;
                        }
                    }
                }

                position.y += EditorGUIUtility.singleLineHeight + 4;

                var left = position;
                var right = position;
                left.width *= 0.4f;
                right.width = position.width - left.width;
                right.x += left.width;
                left.width -= 8f;
                var buttonRect = left;
                buttonRect.x += buttonRect.width - EditorGUIUtility.singleLineHeight;
                buttonRect.width = EditorGUIUtility.singleLineHeight;
                EditorGUIUtility.AddCursorRect(buttonRect, MouseCursor.Arrow);
                buttonRect = GUIUtils.ObjectFieldButtonStyle.margin.Remove(buttonRect);
                var blendshape = control.FindPropertyRelative(nameof(BlendshapeControl.Blendshape));
                EditorGUI.BeginDisabledGroup(renderer == null);
                if (GUI.Button(buttonRect, GUIContent.none, GUIStyle.none))
                {
                    BlendshapeSelector.Show(buttonRect, (renderer as SkinnedMeshRenderer).sharedMesh, x => { blendshape.stringValue = x; blendshape.serializedObject.ApplyModifiedProperties(); });
                }
                EditorGUI.EndDisabledGroup();
                EditorGUI.PropertyField(left, blendshape, GUIContent.none);
                if (Event.current.type == EventType.Repaint)
                {
                    GUIUtils.ObjectFieldButtonStyle.Draw(buttonRect, GUIContent.none, 0, renderer != null, renderer != null && buttonRect.Contains(Event.current.mousePosition));
                }

                var r1 = right;
                var r2 = right;
                var r3 = right;
                r1.width = EditorStyles.label.CalcSize("OFF:".ToGUIContent()).x;
                r2.width = EditorStyles.toggle.CalcSize(GUIUtils.GetTempContent("")).x;
                r3.width -= r2.width + 8 + r1.width + 8;
                r2.x += r1.width + 8;
                r3.x += r1.width + 8 + r2.width + 8;
                EditorGUI.LabelField(r1, "OFF");
                var check = control.FindPropertyRelative(nameof(BlendshapeControl.IsChangeOFF));
                check.boolValue = EditorGUI.ToggleLeft(r2, GUIContent.none, check.boolValue);
                EditorGUI.BeginDisabledGroup(!check.boolValue);
                EditorGUI.PropertyField(r3, control.FindPropertyRelative(nameof(BlendshapeControl.OFF)), GUIContent.none);
                EditorGUI.EndDisabledGroup();

                r1.y += EditorGUIUtility.singleLineHeight + 4;
                r2.y += EditorGUIUtility.singleLineHeight + 4;
                r3.y += EditorGUIUtility.singleLineHeight + 4;
                EditorGUI.LabelField(r1, "ON");
                check = control.FindPropertyRelative(nameof(BlendshapeControl.IsChangeON));
                check.boolValue = EditorGUI.ToggleLeft(r2, GUIContent.none, check.boolValue);
                EditorGUI.BeginDisabledGroup(!check.boolValue);
                EditorGUI.PropertyField(r3, control.FindPropertyRelative(nameof(BlendshapeControl.ON)), GUIContent.none);
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                position.y += EditorGUIUtility.singleLineHeight + 4;
                var control = property.FindPropertyRelative(nameof(AdditionalControl.Animation));
                var isMotionTimed = control.FindPropertyRelative(nameof(AnimationControl.IsMotionTimed));
                isMotionTimed.boolValue = EditorGUI.ToggleLeft(position, "Using motion time", isMotionTimed.boolValue);

                var on = control.FindPropertyRelative(nameof(AnimationControl.ON));
                var off = control.FindPropertyRelative(nameof(AnimationControl.OFF));
                var changeOff = control.FindPropertyRelative(nameof(AnimationControl.IsChangeOFF));
                var changeOn = control.FindPropertyRelative(nameof(AnimationControl.IsChangeON));

                position.y += EditorGUIUtility.singleLineHeight + 4;

                if (isMotionTimed.boolValue)
                {
                    EditorGUI.PropertyField(position, on, GUIContent.none);
                }
                else
                {
                    var r1 = position;
                    var r2 = position;
                    var r3 = position;
                    r1.width = EditorStyles.label.CalcSize("OFF:".ToGUIContent()).x;
                    r2.width = EditorStyles.toggle.CalcSize(GUIUtils.GetTempContent("")).x;
                    r3.width -= r2.width + 8 + r1.width + 8;
                    r2.x += r1.width + 8;
                    r3.x += r1.width + 8 + r2.width + 8;
                    EditorGUI.LabelField(r1, "OFF");
                    changeOff.boolValue = EditorGUI.ToggleLeft(r2, GUIContent.none, changeOff.boolValue);
                    EditorGUI.BeginDisabledGroup(!changeOff.boolValue);
                    EditorGUI.PropertyField(r3, off, GUIContent.none);
                    EditorGUI.EndDisabledGroup();

                    r1.y += EditorGUIUtility.singleLineHeight + 4;
                    r2.y += EditorGUIUtility.singleLineHeight + 4;
                    r3.y += EditorGUIUtility.singleLineHeight + 4;
                    EditorGUI.LabelField(r1, "ON");
                    changeOn.boolValue = EditorGUI.ToggleLeft(r2, GUIContent.none, changeOn.boolValue);
                    EditorGUI.BeginDisabledGroup(!changeOn.boolValue);
                    EditorGUI.PropertyField(r3, on, GUIContent.none);
                    EditorGUI.EndDisabledGroup();
                }
            }
        }
    }
}
