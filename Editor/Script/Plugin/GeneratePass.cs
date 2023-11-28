using nadena.dev.modular_avatar.core;
using nadena.dev.ndmf;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace gomoru.su.clothfire.ndmf
{

    internal sealed class GeneratePass : ClothfireBasePass<GeneratePass>
    {
        protected override bool Run(BuildContext context)
        {
            var treeRoot = Session.DirectBlendTree.AddDirectBlendTree();
            treeRoot.Name = "Controls";

            Dictionary<string, DirectBlendTree> controlGroup = new Dictionary<string, DirectBlendTree>();

            foreach(var target in Session.ControlTargets)
            {
                var treeParent = treeRoot;
                if (target.Parent is IControlGroup group)
                {
                    if (!controlGroup.TryGetValue(group.GroupName, out treeParent))
                    {
                        treeParent = treeRoot.AddDirectBlendTree();
                        treeParent.Name = group.GroupName;
                        controlGroup[group.GroupName] = treeParent;
                    }
                }
                var obj = target.Object;
                var name = target.Name;
                Session.Parameters.Add(CreateParameter(name, target.DefaultState));
                var toggle = treeParent.AddToggle();
                toggle.ParameterName = name;
                toggle.Name = obj.name;
                (toggle.OFF, toggle.ON) = CreateToggleAnimation(target.Path, name);
            }

            return true;
        }

        private static AvatarParameter CreateParameter(string name, bool defaultValue)
        {
            return new AvatarParameter() { Name = name, AnimatorParameterType = AnimatorControllerParameterType.Float, ExpressionParameterType = ParameterSyncType.Bool, DefaultValue = defaultValue ? 1 : 0 }; 
        }

        private (AnimationClip OFF, AnimationClip ON) CreateToggleAnimation(string path, string name)
        {
            var off = new AnimationClip() { name = $"{name} OFF" };
            var on = new AnimationClip() { name = $"{name} ON" };

            AssetDatabase.AddObjectToAsset(off, AssetContainer);
            AssetDatabase.AddObjectToAsset(on, AssetContainer);

            off.SetCurve(path, typeof(GameObject), "m_IsActive", AnimationCurve.Constant(0, 0, 0));
            on.SetCurve(path, typeof(GameObject), "m_IsActive", AnimationCurve.Constant(0, 0, 1));

            return (off, on);
        }
    }
}
