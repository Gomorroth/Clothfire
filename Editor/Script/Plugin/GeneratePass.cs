using nadena.dev.modular_avatar.core;
using nadena.dev.ndmf;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace gomoru.su.clothfire.ndmf
{

    internal sealed class GeneratePass : ClothfireBasePass<GeneratePass>
    {
        protected override bool Run(BuildContext context)
        {
            var treeRoot = Session.DirectBlendTree.AddDirectBlendTree();
            treeRoot.Name = "Controls";

            Dictionary<string, DirectBlendTree> controlGroup = new Dictionary<string, DirectBlendTree>();

            foreach (var target in Session.ControlTargets)
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
                var obj = target.GetTargetObject(context.AvatarRootObject);
                var name = target.ToParameterName(context.AvatarRootObject);
                Session.Parameters.Add(CreateParameter(name, target.DefaultState, target.ParameterSettings));
                Session.ParameterDictionary.Add(obj, name);
                var toggle = treeParent.AddToggle();
                toggle.ParameterName = name;
                toggle.Name = obj.name;
                (toggle.OFF, toggle.ON) = CreateToggleAnimation(target.Path, name);
            }

            return true;
        }

        protected override void OnCreateMenu(BuildContext context, GameObject menu)
        {
            var groups = new Dictionary<string, GameObject>();

            foreach (var target in Session.ControlTargets.AsSpan())
            {
                var parent = menu.gameObject;
                if (target.Parent is IControlGroup group && !string.IsNullOrEmpty(group.GroupName))
                {
                    if (!groups.TryGetValue(group.GroupName, out parent))
                    {
                        parent = CreateSubMenu();
                        parent.transform.parent = menu.transform;
                        parent.name = group.GroupName;
                        groups.Add(group.GroupName, parent);
                    }
                }
                var toggle = CreateMenuToggle(target.ToParameterName(context.AvatarRootObject));
                toggle.name = target.Path.AsSpan(target.Path.LastIndexOf("/") is int idx && idx == -1 ? 0 : (idx + 1)).ToString();
                toggle.transform.parent = parent.transform;
            }
        }

        private static AvatarParameter CreateParameter(string name, bool defaultValue, ParameterSettings parameterSettings)
        {
            return new AvatarParameter() { Name = name, AnimatorParameterType = AnimatorControllerParameterType.Float, ExpressionParameterType = ParameterSyncType.Bool, DefaultValue = defaultValue ? 1 : 0, IsLocalOnly = parameterSettings.IsLocal, IsSaved = parameterSettings.IsSave };
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
