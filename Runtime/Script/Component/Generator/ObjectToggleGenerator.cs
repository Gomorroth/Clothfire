
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace gomoru.su.clothfire
{
    [ExecuteInEditMode]
    [AddComponentMenu("Clothfire/Generator/Object Toggle Generator")]
    public sealed class ObjectToggleGenerator : ClothfireBaseComponent, IControlTargetProvider, IAdditionalControlProvider, IControlGroup
    {
        public string Group;
        public bool IsActiveByDefault;
        public List<AdditionalControl> AdditionalControls = new List<AdditionalControl>();

        public string GroupName => Group;

        public GameObject TargetObject => gameObject;

        public void GetControlTargets(List<ControlTarget> destination)
        {
            destination.Add(new ControlTarget(this, TargetObject, TargetObject.activeInHierarchy));
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
            var path = gameObject.GetRelativePath(gameObject.GetRootObject());
            var condition = new[] { new Condition(path) };
            foreach(var control in AdditionalControls)
            {
                destination.Add(condition, control);
            }
        }
    }
}