using UnityEditor;
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