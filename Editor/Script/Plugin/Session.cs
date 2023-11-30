using nadena.dev.modular_avatar.core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace gomoru.su.clothfire.ndmf
{
    internal sealed class Session
    {
        public bool Handled;

        public DirectBlendTree DirectBlendTree;
        public ControlTarget[] ControlTargets;
        public List<AvatarParameter> Parameters = new List<AvatarParameter>();
        public Dictionary<GameObject, string> ParameterDictionary = new Dictionary<GameObject, string>();
    }
}
