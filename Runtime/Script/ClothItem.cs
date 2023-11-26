using System;
using System.Collections.Generic;
using UnityEngine;

namespace gomoru.su.clothfire
{
    [Serializable]
    public sealed class ClothItem
    {
        public string Path;
        public bool IsInclude;
        public bool IsActiveByDefault;

        public List<AdditionalControl> AdditionalControls;
    }
}