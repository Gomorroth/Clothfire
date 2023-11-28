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

        void IAdditionalControlProvider.GetAdditionalControls(AdditionalControlContainer destination)
        {
            var conditions = Conditions.ToArray();
            foreach(var controls in Controls)
            {
                destination.Add(conditions, controls);
            }
        }
    }
}
