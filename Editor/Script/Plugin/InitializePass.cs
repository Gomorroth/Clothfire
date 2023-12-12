using nadena.dev.ndmf;

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
            if (Session.Configuration == null)
            {
                Session.Configuration = Configuration.AddConfiguration(context.AvatarRootObject);
            }

            return true;
        }
    }
}
