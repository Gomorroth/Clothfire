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
            return GetControlTargetsAsList(root).AsSpan();
        }

        internal static List<ControlTarget> GetControlTargetsAsList(GameObject root)
        {
            var list = SharedList;
            list.Clear();
            foreach (var x in root.GetComponentsInChildren<IControlTargetProvider>(true))
            {
                var y = x as MonoBehaviour;
                if (y.CompareTag("EditorOnly") || !y.enabled)
                    continue;

                x.GetControlTargets(list);
            }
            return list;
        }

        public Object Parent;
        public string Path;
        public bool DefaultState;
        public ParameterSettings ParameterSettings;

        public ControlTarget(Object parent, GameObject obj, bool defaultState, ParameterSettings parameterSettings)
        {
            Parent = parent;
            Path = obj.GetRelativePath(obj.GetComponentInParent<VRCAvatarDescriptor>().gameObject);
            DefaultState = defaultState;
            ParameterSettings = parameterSettings;
        }

        public string ToParameterName(GameObject avatarRootObject)
        {
            var obj = avatarRootObject.Find(Path);
            if (obj == null)
            {
                return null;
            }
            return GetParameterName(obj, Parent, avatarRootObject);
        }

        internal static string GetParameterName(GameObject obj, Object parent, GameObject avatarRootObject)
        {
            var name = obj.name;
            if (parent is IControlGroup group && !string.IsNullOrEmpty(group.GroupName))
            {
                name = $"{group.GroupName}/{name}";
            }
            return name;
        }

        public GameObject GetTargetObject(GameObject avatarRootObject) => avatarRootObject.Find(Path);
    }
}