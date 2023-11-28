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

            var controls = new List<(SingleOrArray<Condition> Conditions, SingleOrArray<AdditionalControl> Controls)>();

            foreach (var x in context.AvatarRootObject.GetComponentsInChildren<IAdditionalControlProvider>())
            {
                x.GetAdditionalControls(controls);
            }

            var gameObjects = new List<GameObject>();

            foreach(var control in controls.AsSpan())
            {
                var objects = control.Conditions.Values is null ? Enumerable.Repeat(context.AvatarRootObject.Find(control.Conditions.Value.Path), 1) : control.Conditions.Values.Select(x => context.AvatarRootObject.Find(x.Path));
                var tree = treeRoot.AddLogicANDGate();
                var name = control.Conditions.Values is null ? Path.GetFileName(control.Conditions.Value.Path) : string.Join(",", control.Conditions.Values.Select(y => Path.GetFileName(y.Path)));

                var off = new AnimationClip() { name = $"{name} OFF" };
                var on = new AnimationClip() { name = $"{name} ON" };

                AssetDatabase.AddObjectToAsset(off, AssetContainer);
                AssetDatabase.AddObjectToAsset(on, AssetContainer);

                tree.OFF = off;
                tree.ON = on;

                tree.Parameters = objects.Select(x => x != null && Session.ParameterDictionary.TryGetValue(x, out var param) ? param : "").ToArray();
            }

            return true;
        }
    }
}
