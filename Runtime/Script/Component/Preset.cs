using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using Object = UnityEngine.Object;

namespace gomoru.su.clothfire
{
    [AddComponentMenu("Clothfire/Clothfire Preset")]
    public sealed class Preset : ClothfireBaseComponent, IHierarchyChangedCallback
    {
        public string PresetName;
        public string Group;
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
            Func<PresetItem, bool> removeCondition = x => x.Target == null || x.Target.CompareTag("EditorOnly");
            if (Targets.Count(removeCondition) != 0)
            {
                RuntimeUtils.RecordObject(this, "Reflesh Items");
                Targets.RemoveAll(new Predicate<PresetItem>(removeCondition));
            }
            var root = GetComponentInParent<VRCAvatarDescriptor>();
            var list = ControlTarget.SharedList;
            list.Clear();
            foreach (var x in root.GetComponentsInChildren<IControlTargetProvider>())
            {
                if ((x as Component).CompareTag("EditorOnly"))
                    continue;

                x.GetControlTargets(list);
            }
            Targets.AddRange(list.Where(x => !Targets.Contains(y => x.GetTargetObject(root.gameObject) == y.Target)).Select(x => new PresetItem() { Target = x.GetTargetObject(root.gameObject), Parent = x.Parent, Include = false, Active = gameObject.GetRootObject()?.Find(x.Path)?.activeInHierarchy ?? false }));
            this.MarkDirty();
        }

        public static int GetHierarchyHashCode(GameObject avatarRootObject)
        {
            var hash = new HashCode();
            foreach(var item in ControlTarget.GetControlTargets(avatarRootObject))
            {
                hash = hash.Append(item.Path.GetHashCode());
                hash = hash.Append(avatarRootObject.Find(item.Path)?.CompareTag("EditorOnly"));
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