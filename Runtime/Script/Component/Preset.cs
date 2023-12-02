using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using Object = UnityEngine.Object;

namespace gomoru.su.clothfire
{
    [AddComponentMenu("Clothfire/Preset")]
    public sealed class Preset : ClothfireBaseComponent, IHierarchyChangedCallback
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
            Targets.RemoveAll(x => x.Target == null);
            foreach (var x in root.GetComponentsInChildren<IControlTargetProvider>())
            {
                x.GetControlTargets(list);
            }
            Targets.AddRange(list.Where(x => !Targets.Contains(y => x.GetTargetObject(root.gameObject) == y.Target)).Select(x => new PresetItem() { Target = x.GetTargetObject(root.gameObject), Parent = x.Parent, Include = false, Active = gameObject.GetRootObject()?.Find(x.Path)?.activeInHierarchy ?? false }));
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

        public void OnHierarchyChanged()
        {
            TryRefleshItems();
        }

        [Serializable]
        public struct PresetItem
        {
            public GameObject Target;
            public Object Parent;
            public bool Include;
            public bool Active;
        }
    }
}