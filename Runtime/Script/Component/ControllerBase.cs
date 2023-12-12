using System;
using UnityEngine;

namespace gomoru.su.clothfire
{
    public abstract class ControllerBase : ClothfireBaseComponent, IControlGroup, IAdditionalControlProvider
    {
        public string MenuName;
        public string Group;

        string IControlGroup.GroupName => Group;

        GameObject IControlGroup.GroupMaster => gameObject;

        GroupType IControlGroup.GroupType => GroupType.Control;

        GameObject IAdditionalControlProvider.GameObject => gameObject;

        void IAdditionalControlProvider.GetAdditionalControls(AdditionalControlContainer destination)
        {
            throw new NotImplementedException();
        }
    }
}
