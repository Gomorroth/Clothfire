using nadena.dev.ndmf;
using System.Runtime.CompilerServices;
using UnityEditor.Animations;

namespace gomoru.su.clothfire.ndmf
{
    internal sealed class InitializePass : ClothfireBasePass<InitializePass>
    {
        protected override bool Run(BuildContext context)
        {
            Session.DirectBlendTree = new DirectBlendTree();
            var targets = ControlTarget.GetControlTargets(context.AvatarRootObject);
            var objects = Session.ControlTargets = new ControlTargetObject[targets.Length];
            
            for(int i = 0; i < objects.Length; i++)
            {
                var obj = context.AvatarRootObject.Find(targets[i].Path);
                string name = obj.name;
                if (targets[i].Parent is IControlGroup group && !string.IsNullOrEmpty(group.GroupName))
                {
                    name = $"{group.GroupName}/{name}";
                }
                var targetObj = Unsafe.As<ControlTarget, ControlTargetObject>(ref Unsafe.AsRef(targets[i]));
                targetObj.Name = name;
                targetObj.Object = obj;

                Session.ParameterDictionary.Add(targets[i].Path, name);

                objects[i] = targetObj;
            }

            if (context.AvatarRootObject.GetComponentInChildren<ClothfireBaseComponent>() == null)
                return false;

            return true;
        }
    }
}
