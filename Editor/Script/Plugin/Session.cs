using System.Collections.Generic;
using UnityEngine;

namespace gomoru.su.clothfire.ndmf
{
    internal sealed class Session
    {
        public bool Handled;

        public DirectBlendTree DirectBlendTree;
        public ControlTargetObject[] ControlTargets;
        public List<AvatarParameter> Parameters = new List<AvatarParameter>();
        public Dictionary<string, string> ParameterDictionary = new Dictionary<string, string>();
    }
}
