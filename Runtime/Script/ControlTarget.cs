using System;
using System.Collections.Generic;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using Object = UnityEngine.Object;

namespace gomoru.su.clothfire
{
    [Serializable]
    public struct ControlTarget
    {
        internal static readonly List<ControlTarget> SharedList = new List<ControlTarget>();

        public static ReadOnlySpan<ControlTarget> GetControlTargets(GameObject root)
        {
            var list = SharedList;
            list.Clear();
            foreach(var x in root.GetComponentsInChildren<IControlTargetProvider>())
            {
                x.GetControlTargets(list);
            }
            return list.AsSpan();
        }

        public Object Parent;
        public string Path;

        public ControlTarget(Object parent, GameObject obj)
        {
            Parent = parent;
            Path = obj.GetRelativePath(obj.GetComponentInParent<VRCAvatarDescriptor>().gameObject);
        }
    }
}