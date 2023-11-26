using System;

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
    }
}