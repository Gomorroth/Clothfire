using UnityEngine;

namespace gomoru.su.clothfire
{
    [DisallowMultipleComponent]
    [AddComponentMenu("")]
    public sealed class Configuration : ClothfireBaseComponent
    {
        public bool GenerateGroupTogglePreset = false;

#if UNITY_EDITOR

        private const string ConfigurationPrefabGUID = "5f0ebe914061cda4090c6b140b3be6a6";

        internal static Configuration AddConfiguration(GameObject target)
        {
            var prefab = UnityEditor.PrefabUtility.InstantiatePrefab(UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(UnityEditor.AssetDatabase.GUIDToAssetPath(ConfigurationPrefabGUID))) as GameObject;
            prefab.transform.parent = target.transform;
            
            var config = prefab.GetComponent<Configuration>();
            if (config == null)
            {
                config = prefab.AddComponent<Configuration>();
            }
            return config;
        }
#endif
    }
}
