using UnityEngine;

namespace gomoru.su.clothfire
{
    partial class Utils
    {
        public static TComponent GetOrAddComponent<TComponent>(this GameObject gameObject) where TComponent : Component
        {
            if (!(gameObject.GetComponent<TComponent>() is TComponent component))
            {
                component = gameObject.AddComponent<TComponent>();
            }
            return component;
        }
    }
}
