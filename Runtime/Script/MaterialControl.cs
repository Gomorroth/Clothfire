using System;
using UnityEngine;

namespace gomoru.su.clothfire
{
    [Serializable]
    public sealed class MaterialControl
    {
        public string Path;
        public int Index;
        public Material OFF;
        public Material ON;
        public bool IsChangeOFF = true;
        public bool IsChangeON = true;
    }
}