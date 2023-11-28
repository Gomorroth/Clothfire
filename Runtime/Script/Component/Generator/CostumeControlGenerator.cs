using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace gomoru.su.clothfire
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Clothfire/Generator/Costume Control Generator")]
    public sealed class CostumeControlGenerator : ClothfireBaseComponent, IControlTargetProvider, IAdditionalControlProvider, IControlGroup, IHierarchyChangedCallback
    {
        public int HashCode;
        public List<ClothItem> Items = new List<ClothItem>();

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
            var components = gameObject.GetChildrenComponents<Renderer>(true);
            {
                foreach (var renderer in components.Span)
                {
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
            Items.RemoveAll(x => gameObject.transform.Find(x.Path) == null);
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
                        hashCode = hashCode.Append(renderer.gameObject.GetRelativePath(root));
                }
            }
            components.Dispose();

            return hashCode.GetHashCode();
        }

        public void GetControlTargets(List<ControlTarget> destination)
        {
            foreach(var item in Items.AsSpan())
            {
                destination.Add(new ControlTarget(this, transform.Find(item.Path).gameObject));
            }
        }

        public void OnHierarchyChanged()
        {
            TryRefleshItems();
        }

        void IAdditionalControlProvider.GetAdditionalControls(List<(SingleOrArray<Condition> Conditions, SingleOrArray<AdditionalControl> Controls)> destination)
        {
            foreach(var item in Items.AsSpan())
            {
                var condition = new Condition(item.Path);
                destination.Add((condition, item.AdditionalControls));
            }
        }
    }
}