using UnityEngine;
using Object = UnityEngine.Object;

namespace gomoru.su.clothfire.ndmf
{
    internal struct ControlTargetObject
    {
        public Object Parent;
        public string Path;
        public bool DefaultState;
        public GameObject Object;
        public string Name;
    }
}
