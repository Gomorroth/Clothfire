﻿using System;
using System.Linq;
using UnityEngine;

namespace gomoru.su.clothfire
{
    internal sealed class GroupSelector : StringSelectorWindow<GroupSelector>
    {
        public static void Show(Rect position, GameObject avatarRootObject, Action<string> callback)
        {
            if (avatarRootObject == null)
                return;

            var groups = avatarRootObject.GetComponentsInChildren<IControlGroup>().Select(x => x.GroupName).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToTemporaryMemory();

            Show(position, groups, callback, new GUIContent($"Select groups"));
        }

        protected override GUIContent GetGUIContent(ref string item) => item.ToGUIContent();
    }
}
