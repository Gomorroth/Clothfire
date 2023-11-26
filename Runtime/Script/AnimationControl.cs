using System;
using UnityEngine;

namespace gomoru.su.clothfire
{
    [Serializable]
    public sealed class AnimationControl
    {
        public bool IsMotionTimed;

        public AnimationClip OFF;
        public bool IsChangeOFF = true;
        public AnimationClip ON;
        public bool IsChangeON = true;
    }
}