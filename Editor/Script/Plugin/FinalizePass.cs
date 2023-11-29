using nadena.dev.modular_avatar.core;
using nadena.dev.ndmf;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;
using Object = UnityEngine.Object;

namespace gomoru.su.clothfire.ndmf
{
    internal sealed class FinalizePass : ClothfireBasePass<FinalizePass>
    {
        protected override bool Run(BuildContext context)
        {
            GenerateMergeAnimator(context);
            GenerateMenu(context);

            foreach(var x in context.AvatarRootObject.GetComponentsInChildren<ClothfireBaseComponent>())
            {
                Object.DestroyImmediate(x);
            }

            return true;
        }

        private void GenerateMergeAnimator(BuildContext context)
        {
            var controller = new AnimatorController() { name = "Clothfire" };
            var layer = Session.DirectBlendTree.ToAnimatorControllerLayer(context.AssetContainer);
            controller.AddLayer(layer);
            controller.AddParameter(new AnimatorControllerParameter() { name = "1", type = AnimatorControllerParameterType.Float, defaultFloat = 1 });
            AssetDatabase.AddObjectToAsset(controller, context.AssetContainer);

            var mama = context.AvatarRootObject.AddComponent<ModularAvatarMergeAnimator>();
            mama.animator = controller;
            mama.matchAvatarWriteDefaults = false;
            mama.pathMode = MergeAnimatorPathMode.Absolute;

            var map = context.AvatarRootObject.AddComponent<ModularAvatarParameters>();

            foreach (var parameter in Session.Parameters.AsSpan())
            {
                controller.AddParameter(parameter.ToAnimatorParameter());
                map.parameters.Add(parameter.ToExpressionParameter());

            }
        }

        private void GenerateMenu(BuildContext context)
        {
            var menuRoot = CreateSubMenu();
            menuRoot.name = "Clothfire";
            menuRoot.transform.parent = context.AvatarRootTransform;
            menuRoot.AddComponent<ModularAvatarMenuInstaller>();

            var groups = new Dictionary<string, GameObject>();
            
            foreach(var target in Session.ControlTargets.AsSpan())
            {
                var parent = menuRoot;
                if (target.Parent is IControlGroup group && !string.IsNullOrEmpty(group.GroupName))
                {
                    if (!groups.TryGetValue(group.GroupName, out parent))
                    {
                        parent = CreateSubMenu();
                        parent.transform.parent = menuRoot.transform;
                        parent.name = group.GroupName;
                        groups.Add(group.GroupName, parent);
                    }
                }
                var toggle = CreateMenuToggle(target.ToParameterName(context.AvatarRootObject));
                toggle.name = target.Path.AsSpan(target.Path.LastIndexOf("/") is int idx && idx == -1 ? 0 : (idx + 1)).ToString();
                toggle.transform.parent = parent.transform;
            }
        }

        private static GameObject CreateSubMenu()
        {
            var obj = CreateMenuItem(out var menuItem);
            menuItem.Control.type = VRCExpressionsMenu.Control.ControlType.SubMenu;
            menuItem.MenuSource = SubmenuSource.Children;
            return obj;
        }

        private static GameObject CreateMenuToggle(string parameterName)
        {
            var obj = CreateMenuItem(out var menuItem);
            menuItem.Control.type = VRCExpressionsMenu.Control.ControlType.Toggle;
            menuItem.Control.parameter = new VRCExpressionsMenu.Control.Parameter() { name = parameterName };
            return obj;
        }

        private static GameObject CreateMenuItem(out ModularAvatarMenuItem menuItem)
        {
            var obj = new GameObject();
            menuItem = obj.AddComponent<ModularAvatarMenuItem>();
            return obj;
        }
    }
}
