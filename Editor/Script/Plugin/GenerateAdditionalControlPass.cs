using nadena.dev.ndmf;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
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

            foreach (var x in context.AvatarRootObject.GetComponentsInChildren<IAdditionalControlProvider>())
            {
                container.CurrentRootObject = x.GameObject;

                x.GetAdditionalControls(container);
            }

            var gameObjects = new List<GameObject>();

            foreach(var item in container.Items)
            {
                Debug.Log($"{string.Join(",", item.Key.Select(x => x.Path))}");
                var conditions = item.Key;
                var tree = treeRoot.AddAndGate();

                var objects = item.Key.Select(x => context.AvatarRootObject.Find(x.Path));
                var name = string.Join(",", objects.Select(x => x.name));
                var off = new AnimationClip() { name = $"{name} OFF" };
                var on = new AnimationClip() { name = $"{name} ON" };

                AssetDatabase.AddObjectToAsset(off, AssetContainer);
                AssetDatabase.AddObjectToAsset(on, AssetContainer);

                foreach (var control in item)
                {
                    switch (control.Type)
                    {
                        case AdditionalControl.ControlType.AnimationClip:
                            SetAnimationClipAnimaiton(control, off, on);
                            break;
                        case AdditionalControl.ControlType.Blendshape:
                            SetBlendshapeAnimation(control, context.AvatarRootObject, off, on);
                            break;
                    }
                }

                tree.OFF = off;
                tree.ON = on;

                tree.Parameters = objects.Select(x => x != null && Session.ParameterDictionary.TryGetValue(x, out var param) ? param : "").ToArray();
            }

            return true;
        }

        private void SetAnimationClipAnimaiton(AdditionalControl control, AnimationClip off, AnimationClip on)
        {

        }

        private void SetBlendshapeAnimation(AdditionalControl control, GameObject avatarRootObject, AnimationClip off, AnimationClip on)
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


        }
    }
}
