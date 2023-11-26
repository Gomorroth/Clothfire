using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace gomoru.su.clothfire
{
    [AddComponentMenu("Clothfire/Preset")]
    public sealed class Preset : ClothfireBaseComponent
    {
        public List<PresetItem> Targets = new List<PresetItem>();
        public int HashCode;

        public bool TryRefleshItems()
        {
            var root = GetComponentInParent<VRCAvatarDescriptor>();
            if (root == null)   
                return false;
            var hash = GetHierarchyHashCode(root.gameObject);
            if (hash != HashCode)
            {
                HashCode = hash;
                RefleshItems();
                return true;
            }
            return false;
        }

        public void RefleshItems()
        {
            var root = GetComponentInParent<VRCAvatarDescriptor>();
            var list = ControlTarget.SharedList;
            list.Clear();
            Targets.RemoveAll(x => x.Target.Parent == null || gameObject.GetRootObject()?.Find(x.Target.Path) == null);
            foreach (var x in root.GetComponentsInChildren<IControlTargetProvider>())
            {
                x.GetControlTargets(list);
            }
            Targets.AddRange(list.Where(x => !Targets.Contains(y => x.Path == y.Target.Path)).Select(x => new PresetItem() { Target = x, Include = true, Active = gameObject.GetRootObject()?.Find(x.Path)?.activeInHierarchy ?? false }));
            this.MarkDirty();
        }

        public static int GetHierarchyHashCode(GameObject avatarRootObject)
        {
            var hash = new HashCode();
            var list = ControlTarget.SharedList;
            list.Clear();
            foreach(var x in avatarRootObject.GetComponentsInChildren<IControlTargetProvider>())
            {
                x.GetControlTargets(list);
            }
            foreach(var item in list.AsSpan())
            {
                hash = hash.Append(item.Path.GetHashCode());
            }
            return hash.GetHashCode();
        }

        [Serializable]
        public struct PresetItem
        {
            public ControlTarget Target;
            public bool Include;
            public bool Active;
        }
    }
}