using System;
using UnityEngine;

namespace gomoru.su.clothfire
{
    [Serializable]
    public sealed class AdditionalControl
    {
        public enum ControlType
        {
            Blendshape,
            AnimationClip
        }

        public ControlType Type;
        public bool IsAbsolute = false;
        public BlendshapeControl Blendshape;
        public AnimationControl Animation;

        [NonSerialized]
        public GameObject Root;
    }
}