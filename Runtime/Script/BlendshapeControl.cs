using System;
using UnityEngine;

namespace gomoru.su.clothfire
{
    [Serializable]
    public sealed class BlendshapeControl
    {
        public string Path;
        public string Blendshape;

        [Range(0, 100f)]
        public float OFF;
        public bool IsChangeOFF = true;

        [Range(0, 100f)]
        public float ON;
        public bool IsChangeON = true;
    }
}