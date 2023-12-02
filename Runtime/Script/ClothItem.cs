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
        public ParameterSettings ParameterSettings;

        public List<AdditionalControl> AdditionalControls;
    }
}