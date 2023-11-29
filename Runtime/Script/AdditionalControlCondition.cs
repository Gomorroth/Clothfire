using UnityEngine;

namespace gomoru.su.clothfire
{
    internal struct AdditionalControlCondition
    {
        public GameObject Object;
        public bool State;

        public AdditionalControlCondition(GameObject obj, bool state)
        {
            Object = obj;
            State = state;
        }
    }
}
