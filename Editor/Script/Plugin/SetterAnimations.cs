using UnityEngine;

namespace gomoru.su.clothfire.ndmf
{
    public readonly struct SetterAnimations
    {
        public readonly AnimationClip Zero;
        public readonly AnimationClip One;

        public SetterAnimations(string parameterName, Object assetContainer)
        {
            Zero = new AnimationClip().AddTo(assetContainer);
            One = new AnimationClip().AddTo(assetContainer);

            Zero.SetCurve("", typeof(Animator), parameterName, AnimationCurve.Constant(0, 0, 0));
            One.SetCurve("", typeof(Animator), parameterName, AnimationCurve.Constant(0, 0, 1));
        }

        public void Deconstruct(out AnimationClip zero, out AnimationClip one) => (zero, one) = (Zero, One);
    }
}
