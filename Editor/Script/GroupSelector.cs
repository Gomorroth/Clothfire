using System;
using System.Linq;
using UnityEngine;

namespace gomoru.su.clothfire
{
    internal sealed class GroupSelector : StringSelectorWindow<GroupSelector>
    {
        public static void Show(Rect position, GameObject avatarRootObject, Action<string> callback, GroupType type = GroupType.All)
        {
            if (avatarRootObject == null)
                return;

            var groups = avatarRootObject.GetComponentsInChildren<IControlGroup>().Where(x => type.HasFlag(x.GroupType)).Select(x => x.GroupName).Where(x => !string.IsNullOrEmpty(x)).Distinct().OrderBy(x => x).ToTemporaryMemory();

            Show(position, groups, callback, new GUIContent($"Select groups"));
        }

        protected override GUIContent GetGUIContent(ref string item) => item.ToGUIContent();
    }
}
