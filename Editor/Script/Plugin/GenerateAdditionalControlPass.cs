using nadena.dev.ndmf;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

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
                            succss &= SetAnimationClipAnimaiton(control, off, on);
                            break;
                        case AdditionalControl.ControlType.Blendshape:
                            succss &= SetBlendshapeAnimation(control, context.AvatarRootObject, off, on);
                            break;
                    }

                    if (!succss)
                        break;
                }

                tree.OFF = off;
                tree.ON = on;

                treeRoot.Add(tree);
            }

            return true;
        }

        private bool SetAnimationClipAnimaiton(AdditionalControl control, AnimationClip off, AnimationClip on)
        {
            return true;
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
                Debug.LogError(string.Join(",", Conditions.Select(x => x.Object?.name ?? "Null")));
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
