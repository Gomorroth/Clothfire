using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace gomoru.su.clothfire
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Clothfire/Clothfire Costume Controller")]
    public sealed class CostumeController : ClothfireBaseComponent, IControlTargetProvider, IAdditionalControlProvider, IControlGroup, IHierarchyChangedCallback
    {
        public int HashCode;
        public List<ClothItem> Items = new List<ClothItem>();

        public string GroupName => gameObject.name;
        GameObject IControlGroup.GroupMaster => gameObject;
        GameObject IAdditionalControlProvider.GameObject => gameObject;
        GroupType IControlGroup.GroupType => GroupType.Control;

        public bool TryRefleshItems()
        {
            var hashCode = GetHierarchyHashCode(gameObject);
            if (HashCode != hashCode)
            {
                HashCode = hashCode;
                RefleshItems();
                return true;
            }
            return false;
        }

        public void RefleshItems()
        {
            Func<ClothItem, bool> condition = x => !(gameObject.Find(x.Path) is GameObject obj) || obj.CompareTag("EditorOnly");
            if (Items.Count(condition) != 0)
            {
                RuntimeUtils.RecordObject(this, "Reflesh Items");
                Items.RemoveAll(x => !(gameObject.Find(x.Path) is GameObject obj) || obj.CompareTag("EditorOnly"));
            }
            var components = gameObject.GetChildrenComponents<Renderer>(true);
            {
                foreach (var renderer in components.Span)
                {
                    if (renderer.gameObject.CompareTag("EditorOnly"))
                        continue;

                    if (renderer is MeshRenderer || renderer is SkinnedMeshRenderer)
                    {
                        var path = renderer.gameObject.GetRelativePath(gameObject);
                        if (Items.Contains(x => x.Path == path))
                            continue;

                        Items.Add(new ClothItem() { Path = path, IsInclude = true, IsActiveByDefault = renderer.gameObject.activeInHierarchy });
                    }
                }
            }
            components.Dispose();
            this.MarkDirty();
        }

        private static int GetHierarchyHashCode(GameObject root)
        {
            var hashCode = new HashCode();
            var components = root.GetChildrenComponents<Renderer>(true);
            {
                foreach (var renderer in components.Span)
                {
                    if (renderer is MeshRenderer || renderer is SkinnedMeshRenderer)
                    {
                        hashCode = hashCode.Append(renderer.gameObject.GetRelativePath(root));
                        hashCode = hashCode.Append(renderer.gameObject.CompareTag("EditorOnly"));
                    }
                }
            }
            components.Dispose();

            return hashCode.GetHashCode();
        }

        public void GetControlTargets(List<ControlTarget> destination)
        {
            foreach(var item in Items.AsSpan())
            {
                if (!item.IsInclude)
                    continue;

                var obj = transform.Find(item.Path);
                if (obj == null)
                    continue;
                destination.Add(new ControlTarget(this, obj.gameObject, item.IsActiveByDefault, item.ParameterSettings));
            }
        }

        public void OnHierarchyChanged()
        {
            TryRefleshItems();
        }

        void IAdditionalControlProvider.GetAdditionalControls(AdditionalControlContainer destination)
        {
            foreach(var item in Items.AsSpan())
            {
                var conditions = new[] { new AdditionalControlCondition(gameObject.Find(item.Path), true) };
                foreach(var controls in item.AdditionalControls)
                {
                    destination.Add(conditions, controls);
                }
            }
        }
    }
}