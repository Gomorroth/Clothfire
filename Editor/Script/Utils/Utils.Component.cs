using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
