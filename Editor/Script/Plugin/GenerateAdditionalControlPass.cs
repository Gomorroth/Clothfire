using nadena.dev.ndmf;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace gomoru.su.clothfire.ndmf
{
    internal sealed class GenerateAdditionalControlPass : ClothfireBasePass<GenerateAdditionalControlPass>
    {
        protected override bool Run(BuildContext context)
        {
            var treeRoot = Session.DirectBlendTree.AddDirectBlendTree();
            treeRoot.Name = "Additional Controls";

            var container = new AdditionalControlContainer();
            container.AvatarRootObject = context.AvatarRootObject;

            foreach (var x in context.AvatarRootObject.GetComponentsInChildren<IAdditionalControlProvider>())
            {
                container.CurrentRootObject = x.GameObject;

                x.GetAdditionalControls(container);
            }

            foreach(var item in container.Items)
            {
                var tree = new AdditionalControlBlendTree() { Conditions = item.Key, Session = Session };

                var off = new AnimationClip() { name = $"OFF" };
                var on = new AnimationClip() { name = $"ON" };

                AssetDatabase.AddObjectToAsset(off, AssetContainer);
                AssetDatabase.AddObjectToAsset(on, AssetContainer);

                bool succss = true;

                foreach (var control in item)
                {
                    switch (control.Type)
                    {
                        case AdditionalControl.ControlType.AnimationClip:
                            succss &= SetAnimationClipAnimaiton(control, context.AvatarRootObject, off, on);
                            break;
                        case AdditionalControl.ControlType.Blendshape:
                            succss &= SetBlendshapeAnimation(control, context.AvatarRootObject, off, on);
                            break;
                    }

                    if (!succss)
                        break;
                }

                if (!succss)
                    break;

                tree.OFF = off;
                tree.ON = on;

                treeRoot.Add(tree);
            }

            return true;
        }

        private bool SetAnimationClipAnimaiton(AdditionalControl control, GameObject avatarRootObject, AnimationClip off, AnimationClip on)
        {
            var animation = control.Animation;

            var s_on = animation.ON;
            var s_off = animation.OFF;

            if (s_on == null && s_off == null)
                return false;

            if (s_on == null)
                s_on = GenerateDefaultAnimation(s_off, control.IsAbsolute ? avatarRootObject : control.Root);
            else if (s_off == null)
                s_off = GenerateDefaultAnimation(s_on, control.IsAbsolute ? avatarRootObject : control.Root);

            if (!control.IsAbsolute)
            {
                s_on = ResolveAnimationPath(s_on, control.Root, avatarRootObject);
                s_off = ResolveAnimationPath(s_off, control.Root, avatarRootObject);
            }


            EditorUtility.CopySerializedIfDifferent(s_on, off);
            EditorUtility.CopySerializedIfDifferent(s_off, on);
            
            return true;
        }

        private static AnimationClip ResolveAnimationPath(AnimationClip animation, GameObject currentRootObject, GameObject avatarRootObject)
        {
            if (animation == null)
                return null;
            var name = $"{animation.name} (Remapped)";
            animation = Object.Instantiate(animation);
            animation.name = $"{name}";

            var bindings = AnimationUtility.GetCurveBindings(animation)
                .Select(x => (Binding: x, RemappedBinding: RemapBinding(x, currentRootObject, avatarRootObject), Editor:AnimationUtility.GetEditorCurve(animation, x), Object: AnimationUtility.GetObjectReferenceCurve(animation, x)))
                ;

            foreach(var x in bindings)
            {
                if (x.Editor != null)
                {
                    AnimationUtility.SetEditorCurve(animation, x.Binding, null);
                    AnimationUtility.SetEditorCurve(animation, x.RemappedBinding, x.Editor);
                }
                if (x.Object != null)
                {
                    AnimationUtility.SetObjectReferenceCurve(animation, x.Binding, null);
                    AnimationUtility.SetObjectReferenceCurve(animation, x.RemappedBinding, x.Object);
                }
            }
                
            return animation;
        }

        private static EditorCurveBinding RemapBinding(EditorCurveBinding binding, GameObject currentRootObject, GameObject avatarRootObject)
        {
            var obj = currentRootObject.Find(binding.path);
            if (obj == null)
                return binding;
            binding.path = obj.GetRelativePath(avatarRootObject);
            return binding;
        }

        private static AnimationClip GenerateDefaultAnimation(AnimationClip source, GameObject rootObject)
        {
            var destination = new AnimationClip();
            AnimationUtility.SetAnimationClipSettings(destination, AnimationUtility.GetAnimationClipSettings(source));
            destination.name = $"{source.name} (Default)";

            foreach(var binding in AnimationUtility.GetCurveBindings(source))
            {
                var obj = rootObject.Find(binding.path);
                if (obj == null) continue;

                if (AnimationUtility.GetEditorCurve(source, binding) is AnimationCurve editorCurve
                    && AnimationUtility.GetFloatValue(rootObject, binding, out var value))
                {
                    var keys = editorCurve.keys;
                    foreach(ref var key in keys.AsSpan())
                    {
                        key.value = value;
                    }
                    editorCurve.keys = keys;
                    AnimationUtility.SetEditorCurve(destination, binding, editorCurve);
                }
                if (AnimationUtility.GetObjectReferenceCurve(source, binding) is ObjectReferenceKeyframe[] objectKeys
                    && AnimationUtility.GetObjectReferenceValue(rootObject, binding, out var objValue))
                {
                    foreach(ref var x in objectKeys.AsSpan())
                    {
                        x.value = objValue;
                    }
                    AnimationUtility.SetObjectReferenceCurve(destination, binding, objectKeys);
                }

            }

            return destination;
        }

        private bool SetBlendshapeAnimation(AdditionalControl control, GameObject avatarRootObject, AnimationClip off, AnimationClip on)
        {
            var blendshape = control.Blendshape;
            GameObject target;
            string path;
            if (control.IsAbsolute)
            {
                target = avatarRootObject.Find(blendshape.Path);
                path = blendshape.Path;
            }
            else
            {
                target = control.Root.Find(blendshape.Path);
                path = target.GetRelativePath(avatarRootObject);
            }

            var renderer = target?.GetComponent<SkinnedMeshRenderer>();
            var blendshapes = renderer?.EnumerateBlendshapes().ToDictionary(x => x.Name, x => x.Index);
            if (renderer == null || !blendshapes.TryGetValue(blendshape.Blendshape, out var shapeIdx))
                return false;

            var offValue = blendshape.IsChangeOFF ? blendshape.OFF : renderer.GetBlendShapeWeight(shapeIdx);
            var onValue = blendshape.IsChangeON ? blendshape.ON : renderer.GetBlendShapeWeight(shapeIdx);

            off.SetCurve(path, typeof(SkinnedMeshRenderer), $"blendShape.{blendshape.Blendshape}", AnimationCurve.Constant(0, 0, offValue));
            on.SetCurve(path, typeof(SkinnedMeshRenderer), $"blendShape.{blendshape.Blendshape}", AnimationCurve.Constant(0, 0, onValue));

            return true;

        }

        private sealed class AdditionalControlBlendTree : DirectBlendTreeItemBase
        {
            public Motion ON { get; set; }
            public Motion OFF { get; set; }

            public IEnumerable<AdditionalControlCondition> Conditions { get; set; }

            public Session Session { get; set; }

            protected override void Apply(BlendTree destination, Object assetContainer)
            {
                var blendTree = new BlendTree().AddTo(assetContainer);
                var root = blendTree;
                foreach(var condition in Conditions.SkipLast(1))
                {
                    blendTree.blendParameter = Session.ParameterDictionary[condition.Object];
                    BlendTree child;
                    if (condition.State)
                    {
                        blendTree.AddChild(OFF, 0);
                        child = blendTree.CreateBlendTreeChild(1);
                    }
                    else
                    {
                        child = blendTree.CreateBlendTreeChild(0);
                        blendTree.AddChild(OFF, 1);
                    }
                    blendTree = child;
                }

                {
                    var condition = Conditions.LastOrDefault();

                    blendTree.blendParameter = Session.ParameterDictionary[condition.Object];
                    blendTree.AddChild(condition.State ? OFF : ON, 0);
                    blendTree.AddChild(condition.State ? ON : OFF, 1);
                }

                destination.AddChild(root);
            }
        }
    }
}
