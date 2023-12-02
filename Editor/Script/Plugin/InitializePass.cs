using nadena.dev.ndmf;
using System.Runtime.CompilerServices;
using UnityEditor.Animations;

namespace gomoru.su.clothfire.ndmf
{
    internal sealed class InitializePass : ClothfireBasePass<InitializePass>
    {
        protected override bool Run(BuildContext context)
        {
            Session.ControlTargets = ControlTarget.GetControlTargets(context.AvatarRootObject).ToArray();
            if (Session.ControlTargets.Length == 0)
                return false;

            Session.DirectBlendTree = new DirectBlendTree();
            Session.Configuration = context.AvatarRootObject.GetComponentInChildren<Configuration>();

            return true;
        }
    }
}
