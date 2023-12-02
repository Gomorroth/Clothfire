using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace gomoru.su.clothfire
{
    internal static partial class ContextMenu
    {
        private const string AddConfigurationPath = "GameObject/Clothfire/Add Configuration";

        private const string ConfigurationPrefabGUID = "5f0ebe914061cda4090c6b140b3be6a6";

        private const int Priority = 42;

        [MenuItem(AddConfigurationPath, false, Priority)]
        public static void AddConfiguration()
        {
            var prefab = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(ConfigurationPrefabGUID))) as GameObject;
            prefab.transform.parent = Selection.activeGameObject.transform;
        }

        [MenuItem(AddConfigurationPath, true, Priority)]
        public static bool ValidateAddConfiguration()
        {
            return Selection.activeGameObject != null && 
                Selection.activeGameObject.GetComponent<VRCAvatarDescriptor>() != null &&
                Selection.activeGameObject.GetComponentInChildren<Configuration>() == null;
        }
    }
}
