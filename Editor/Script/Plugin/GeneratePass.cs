using nadena.dev.modular_avatar.core;
using nadena.dev.ndmf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Avatars.ScriptableObjects;
using Object = UnityEngine.Object;

namespace gomoru.su.clothfire.ndmf
{
    internal sealed class Session
    {
        public bool Handled;

        public DirectBlendTree DirectBlendTree;
        public ControlTargetObject[] ControlTargets;
        public List<AvatarParameter> Parameters = new List<AvatarParameter>();
    }

    internal struct ControlTargetObject
    {
        public Object Parent;
        public string Path;
        public bool DefaultState;
        public GameObject Object;
        public string Name;
    }

    internal struct AvatarParameter
    {
        public string Name;
        public float DefaultValue;
        public AnimatorControllerParameterType AnimatorParameterType;
        public ParameterSyncType ExpressionParameterType;
        public bool IsLocalOnly;
        public bool IsSaved;

        public AnimatorControllerParameter ToAnimatorParameter()
        {
            return new AnimatorControllerParameter()
            {
                name = Name,
                type = AnimatorParameterType,
                defaultInt = Mathf.FloorToInt(DefaultValue),
                defaultFloat = DefaultValue,
                defaultBool = DefaultValue != default,
            };
        }

        public ParameterConfig ToExpressionParameter()
        {
            return new ParameterConfig()
            {
                nameOrPrefix = Name,
                defaultValue = DefaultValue,
                syncType = ExpressionParameterType,
                localOnly = IsLocalOnly,
                saved = IsSaved,
            };
        }
    }

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

        private static (AnimationClip OFF, AnimationClip ON) CreateToggleAnimation(string path, string name)
        {
            var off = new AnimationClip() { name = $"{name} OFF" };
            var on = new AnimationClip() { name = $"{name} ON" };

            off.SetCurve(path, typeof(GameObject), "m_IsActive", AnimationCurve.Constant(0, 0, 0));
            on.SetCurve(path, typeof(GameObject), "m_IsActive", AnimationCurve.Constant(0, 0, 1));

            return (off, on);
        }
    }

    internal sealed class InitializePass : ClothfireBasePass<InitializePass>
    {
        protected override bool Run(BuildContext context)
        {
            Session.DirectBlendTree = new DirectBlendTree(context.AssetContainer);
            var targets = ControlTarget.GetControlTargets(context.AvatarRootObject);
            var objects = Session.ControlTargets = new ControlTargetObject[targets.Length];
            
            for(int i = 0; i < objects.Length; i++)
            {
                var obj = context.AvatarRootObject.Find(targets[i].Path);
                string name = obj.name;
                if (targets[i].Parent is IControlGroup group && !string.IsNullOrEmpty(group.GroupName))
                {
                    name = $"{group.GroupName}/{name}";
                }
                var targetObj = Unsafe.As<ControlTarget, ControlTargetObject>(ref Unsafe.AsRef(targets[i]));
                targetObj.Name = name;
                targetObj.Object = obj;

                objects[i] = targetObj;
            }

            if (context.AvatarRootObject.GetComponentInChildren<ClothfireBaseComponent>() == null)
                return false;

            return true;
        }
    }

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
            var layer = Session.DirectBlendTree.ToAnimatorControllerLayer();
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
                var toggle = CreateMenuToggle(target.Name);
                toggle.name = target.Object.name;
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
