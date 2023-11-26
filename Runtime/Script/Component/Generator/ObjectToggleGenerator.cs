﻿
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace gomoru.su.clothfire
{
    [ExecuteInEditMode]
    [AddComponentMenu("Clothfire/Generator/Object Toggle Generator")]
    public sealed class ObjectToggleGenerator : ClothfireBaseComponent, IControlTargetProvider
    {
        public string Group;
        public bool IsActiveByDefault;
        public List<AdditionalControl> AdditionalControls = new List<AdditionalControl>();

        public GameObject TargetObject => gameObject;

        public void GetControlTargets(List<ControlTarget> destination)
        {
            destination.Add(new ControlTarget(this, TargetObject));
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
    }
}