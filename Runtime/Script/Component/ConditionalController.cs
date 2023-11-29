using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace gomoru.su.clothfire
{
    [AddComponentMenu("Clothfire/Conditional Controller")]
    public sealed class ConditionalController : ClothfireBaseComponent, IAdditionalControlProvider
    {
        public List<Condition> Conditions;
        public List<AdditionalControl> Controls;

        GameObject IAdditionalControlProvider.GameObject => gameObject;

        void IAdditionalControlProvider.GetAdditionalControls(AdditionalControlContainer destination)
        {
            var conditions = Conditions;
            foreach(var controls in Controls)
            {
                destination.Add(conditions, controls);
            }
        }

        [Serializable]
        public struct Condition
        {
            public string Path;
            public bool State;

            public Condition(string parameterName, bool state = true)
            {
                Path = parameterName;
                State = state;
            }

            public override int GetHashCode() => new HashCode().Append(Path).Append(State).GetHashCode();
        }
    }
}
