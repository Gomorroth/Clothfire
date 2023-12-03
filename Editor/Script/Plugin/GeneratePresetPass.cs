using nadena.dev.modular_avatar.core;
using nadena.dev.ndmf;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.Presets;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace gomoru.su.clothfire.ndmf
{
    internal sealed class GeneratePresetPass : ClothfireBasePass<GeneratePresetPass>
    {
        private const string PresetParameterName = "//Clothfire/Preset";

        private Preset[] _presets;

        protected override bool Run(BuildContext context)
        {
            if (Session.Configuration.GenerateGroupTogglePreset)
            {
                GenerateGroupTogglePreset(context.AvatarRootObject);
            }

            var presets = _presets = context.AvatarRootObject.GetComponentsInChildren<Preset>();
            if (presets?.Length == 0)
              return true;

            var controller = new AnimatorController();
            var mama = context.AvatarRootObject.AddComponent<ModularAvatarMergeAnimator>();
            mama.animator = controller;
            mama.layerType = VRCAvatarDescriptor.AnimLayerType.FX;
            mama.matchAvatarWriteDefaults = true;

            var layer = new AnimatorControllerLayer() { name = "Clothfire Preset", stateMachine = new AnimatorStateMachine().AddTo(AssetContainer) };
            controller.AddLayer(layer);
            var stateMachine = layer.stateMachine;
            var blank = new AnimationClip();

            var idle = stateMachine.AddState("Idle");
            idle.motion = blank;
            stateMachine.defaultState = idle;

            foreach(var (preset, i) in presets.Select(Tuple.Create<Preset, int>))
            {
                var name = preset.name;
                if (!string.IsNullOrEmpty(preset.Group))
                {
                    name = $"{preset.Group} {name}";
                }
                var state = stateMachine.AddState(name);
                state.motion = blank;
                var driver = state.AddStateMachineBehaviour<VRCAvatarParameterDriver>();
                foreach(var target in preset.Targets.AsSpan())
                {
                    if (!target.Include)
                        continue;

                    driver.parameters.Add(new VRC.SDKBase.VRC_AvatarParameterDriver.Parameter()
                    {
                        name = ControlTarget.GetParameterName(target.Target, target.Parent, context.AvatarRootObject),
                        type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set,
                        value = VRCParameterConversion.ToSingle(target.Active)
                    });
                }
                var tr = idle.AddTransition(state);
                tr.AddCondition(AnimatorConditionMode.Equals, i + 1, PresetParameterName);

                tr = state.AddTransition(idle);
                tr.AddCondition(AnimatorConditionMode.NotEqual, i + 1, PresetParameterName);
            }

            Session.Parameters.Add(new AvatarParameter() { Name = PresetParameterName, AnimatorParameterType = AnimatorControllerParameterType.Int, ExpressionParameterType = ParameterSyncType.Int, IsLocalOnly = true });

            return true;
        }

        private void GenerateGroupTogglePreset(GameObject avatarRootObject)
        {
            foreach(var group in ControlTarget.GetControlTargetsAsList(avatarRootObject).Where(x => x.Parent is IControlGroup group && !string.IsNullOrEmpty(group.GroupName)).GroupBy(x => (x.Parent as IControlGroup)?.GroupName ?? string.Empty).OrderBy(x => x.Key))
            {
                var on = new GameObject($"ON");
                var off  = new GameObject($"OFF");
                on.transform.parent = avatarRootObject.transform;
                off.transform.parent = avatarRootObject.transform;
                var on_preset = on.AddComponent<Preset>();
                var off_preset = off.AddComponent<Preset>();
                on_preset.Group = group.Key;
                off_preset.Group = group.Key;
                foreach (var x in group)
                {
                    var item = new Preset.PresetItem()
                    {
                        Active = true,
                        Include = true,
                        Target = avatarRootObject.Find(x.Path),
                        Parent = x.Parent,
                    };
                    on_preset.Targets.Add(item);
                    item.Active = false;
                    off_preset.Targets.Add(item);
                }
            }
        }

        protected override void OnCreateMenu(BuildContext context, GameObject menu)
        {
            if ((_presets?.Length ?? 0) == 0)
                return;

            var menuRoot = CreateSubMenu();
            menuRoot.name = "Preset";
            menuRoot.transform.parent = menu.transform;
            menuRoot.transform.SetSiblingIndex(0);

            var dict = new Dictionary<string, GameObject>();

            foreach (var (preset, i) in _presets.Select(Tuple.Create<Preset, int>))
            {
                var menuParent = menuRoot;
                if (!string.IsNullOrEmpty(preset.Group))
                {
                    menuParent = dict.GetOrAdd(preset.Group, x =>
                    {
                        var m = CreateSubMenu();
                        m.name = x;
                        m.transform.parent = menuRoot.transform;
                        return m;
                    });
                }
                var button = CreateMenuButton(PresetParameterName, i + 1);
                button.name = preset.name;
                button.transform.parent = menuParent.transform;
            }
        }
    }
}
