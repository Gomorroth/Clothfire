﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace gomoru.su.clothfire
{
    [AddComponentMenu("Clothfire/Clothfire Conditional Controller")]
    public sealed class ConditionalController : ClothfireBaseComponent, IAdditionalControlProvider
    {
        public List<Condition> Conditions;
        public List<AdditionalControl> Controls;

        GameObject IAdditionalControlProvider.GameObject => gameObject;

        void IAdditionalControlProvider.GetAdditionalControls(AdditionalControlContainer destination)
        {
            var conditions = Conditions.Select(x => new AdditionalControlCondition(x.Object, x.State));
            foreach(var controls in Controls)
            {
                destination.Add(conditions, controls);
            }
        }

        [Serializable]
        public struct Condition
        {
            public GameObject Object;
            public bool State;

            public Condition(GameObject obj, bool state = true)
            {
                Object = obj;
                State = state;
            }

            public override int GetHashCode() => new HashCode().Append(Object).Append(State).GetHashCode();
        }
    }
}
