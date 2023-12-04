using System;
using UnityEngine;

namespace gomoru.su.clothfire
{
    [Serializable]
    public sealed class MaterialControl
    {
        public Renderer Target;
        public Material OFF;
        public Material ON;
        public bool IsChangeOFF = true;
        public bool IsChangeON = true;
    }
}