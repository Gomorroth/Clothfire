
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace gomoru.su.clothfire
{
    [ExecuteInEditMode]
    [AddComponentMenu("Clothfire/Clothfire Object Toggle")]
    public sealed class ObjectToggle : ClothfireBaseComponent, IControlTargetProvider, IAdditionalControlProvider, IControlGroup
    {
        public string Group;
        public bool IsActiveByDefault;
        public ParameterSettings ParameterSettings = ParameterSettings.Default;
        public List<AdditionalControl> AdditionalControls = new List<AdditionalControl>();

        public string GroupName => Group;
        GameObject IControlGroup.GroupMaster => null;
        GameObject IAdditionalControlProvider.GameObject => gameObject;

        public GameObject TargetObject => gameObject;

        public void GetControlTargets(List<ControlTarget> destination)
        {
            destination.Add(new ControlTarget(this, TargetObject, TargetObject.activeInHierarchy, ParameterSettings));
        }

#if UNITY_EDITOR
        internal void Awake()
        {
            IsActiveByDefault = gameObject.activeInHierarchy;
        }
#endif

        internal void OnEnable()
        {
        }

        void IAdditionalControlProvider.GetAdditionalControls(AdditionalControlContainer destination)
        {
            var condition = new[] { new AdditionalControlCondition(gameObject, true) };
            foreach(var control in AdditionalControls)
            {
                destination.Add(condition, control);
            }
        }
    }
}