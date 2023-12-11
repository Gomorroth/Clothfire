using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.Bindings;
using Object = UnityEngine.Object;

namespace gomoru.su.clothfire
{
    internal static class AssetUtils
    {
        public static T AddTo<T>(this T asset, Object destination) where T : Object
        {
            AssetDatabase.AddObjectToAsset(asset, destination);
            return asset;
        }
    }
}