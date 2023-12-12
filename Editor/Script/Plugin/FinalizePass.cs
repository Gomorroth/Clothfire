using nadena.dev.modular_avatar.core;
using nadena.dev.ndmf;
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

            var map = context.AvatarRootObject.AddComponent<ModularAvatarParameters>();

            foreach (var parameter in Session.Parameters.AsSpan())
            {
                var param = parameter.ToExpressionParameter();
                if (!param.nameOrPrefix.StartsWith("//Clothfire/"))
                    param.remapTo = $"//Clothfire/Parameter/{param.nameOrPrefix}";
                map.parameters.Add(param);
            }

            foreach (var x in context.AvatarRootObject.GetComponentsInChildren<ClothfireBaseComponent>())
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
            AssetDatabase.AddObjectToAsset(controller, context.AssetContainer);
            Session.Parameters.Add(new AvatarParameter() { Name = "1", AnimatorParameterType = AnimatorControllerParameterType.Float, ExpressionParameterType = ParameterSyncType.NotSynced, DefaultValue = 1 });

            var mama = context.AvatarRootObject.AddComponent<ModularAvatarMergeAnimator>();
            mama.animator = controller;
            mama.matchAvatarWriteDefaults = false;
            mama.pathMode = MergeAnimatorPathMode.Absolute;

            foreach (var parameter in Session.Parameters.AsSpan())
            {
                controller.AddParameter(parameter.ToAnimatorParameter());

            }
        }

        private void GenerateMenu(BuildContext context)
        {
            GameObject menuRoot;
            if (Session.Configuration == null)
            {
                menuRoot = CreateSubMenu();
                menuRoot.name = "Clothfire";
                menuRoot.transform.parent = context.AvatarRootTransform;
                menuRoot.AddComponent<ModularAvatarMenuInstaller>();
            }
            else
            {
                menuRoot = Session.Configuration.gameObject;
                var menuItem = menuRoot.GetOrAddComponent<ModularAvatarMenuItem>();
                menuItem.Control.type = VRCExpressionsMenu.Control.ControlType.SubMenu;
                menuItem.MenuSource = SubmenuSource.Children;
                var menuInstaller = menuRoot.GetOrAddComponent<ModularAvatarMenuInstaller>();
                menuInstaller.menuToAppend = null;
            }

            foreach (var pass in PluginDefinition.Passes)
            {
                pass.OnCreateMenu(context, menuRoot);
            }
        }
    }
}
