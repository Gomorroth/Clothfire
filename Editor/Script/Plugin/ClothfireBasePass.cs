using System;
using nadena.dev.ndmf;
using UnityEngine;
using Object = UnityEngine.Object;

namespace gomoru.su.clothfire.ndmf
{
    internal abstract class ClothfireBasePass<T> : Pass<T> where T : Pass<T>, new()
    {
        public override string DisplayName => $"Clothfire{typeof(T).Name}";

        protected Session Session { get; private set; }
        protected Object AssetContainer { get; private set; }

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
            catch(Exception e)
            {
                Debug.LogException(e);
                Session.Handled = true;
            }
        }

        protected abstract bool Run(BuildContext context);
    }
}
