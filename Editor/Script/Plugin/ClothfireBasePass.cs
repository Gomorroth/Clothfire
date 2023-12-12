using nadena.dev.modular_avatar.core;
using nadena.dev.ndmf;
using System;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;
using Object = UnityEngine.Object;

namespace gomoru.su.clothfire.ndmf
{
    internal interface IClothfirePass
    {
        void OnCreateMenu(BuildContext context, GameObject menu);
        bool Run(BuildContext context);
    }
    internal abstract class ClothfireBasePass<T> : Pass<T>, IClothfirePass where T : Pass<T>, new()
    {
        public override string DisplayName => $"Clothfire{typeof(T).Name}";


        protected Session Session { get; private set; }
        protected Object AssetContainer { get; private set; }

        protected ClothfireBasePass()
        {

        }

        protected override void Execute(BuildContext context)
        {
            Session = context.GetState<Session>();
            AssetContainer = context.AssetContainer;
            if (Session.Handled)
                return;

            try
            {
                if (!Run(context))
                {
                    Session.Handled = true;
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Session.Handled = true;
            }
        }

        protected abstract bool Run(BuildContext context);

        protected virtual void OnCreateMenu(BuildContext context, GameObject menu) { }

        bool IClothfirePass.Run(BuildContext context) => Run(context);
        void IClothfirePass.OnCreateMenu(BuildContext context, GameObject menu) => OnCreateMenu(context, menu);

        protected static GameObject CreateSubMenu()
        {
            var obj = CreateMenuItem(out var menuItem);
            menuItem.Control.type = VRCExpressionsMenu.Control.ControlType.SubMenu;
            menuItem.MenuSource = SubmenuSource.Children;
            return obj;
        }

        protected static GameObject CreateMenuButton<TValue>(string parameterName, TValue value)
        {
            var obj = CreateMenuItem(out var menuItem);
            menuItem.Control.type = VRCExpressionsMenu.Control.ControlType.Button;
            menuItem.Control.parameter = new VRCExpressionsMenu.Control.Parameter() { name = parameterName };
            menuItem.Control.value = VRCParameterConversion.ToSingle(value);
            return obj;
        }

        protected static GameObject CreateMenuToggle(string parameterName)
        {
            var obj = CreateMenuItem(out var menuItem);
            menuItem.Control.type = VRCExpressionsMenu.Control.ControlType.Toggle;
            menuItem.Control.parameter = new VRCExpressionsMenu.Control.Parameter() { name = parameterName };
            return obj;
        }

        protected static GameObject CreateMenuItem(out ModularAvatarMenuItem menuItem)
        {
            var obj = new GameObject();
            menuItem = obj.AddComponent<ModularAvatarMenuItem>();
            return obj;
        }
    }
}
