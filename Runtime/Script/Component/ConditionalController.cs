using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace gomoru.su.clothfire
{
    [AddComponentMenu("Clothfire/Conditional Controller")]
    public sealed class ConditionalController : ClothfireBaseComponent
    {
        public List<Condition> Conditions;
        public List<AdditionalControl> Controls;

        [Serializable]
        public struct Condition
        {
            public string Path;
            public bool State;
        }
    }
}
