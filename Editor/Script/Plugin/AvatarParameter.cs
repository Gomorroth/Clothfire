using nadena.dev.modular_avatar.core;
using UnityEngine;

namespace gomoru.su.clothfire.ndmf
{
    internal struct AvatarParameter
    {
        public string Name;
        public float DefaultValue;
        public AnimatorControllerParameterType AnimatorParameterType;
        public ParameterSyncType ExpressionParameterType;
        public bool IsLocalOnly;
        public bool IsSaved;

        public AnimatorControllerParameter ToAnimatorParameter()
        {
            return new AnimatorControllerParameter()
            {
                name = Name,
                type = AnimatorParameterType,
                defaultInt = Mathf.FloorToInt(DefaultValue),
                defaultFloat = DefaultValue,
                defaultBool = DefaultValue != default,
            };
        }

        public ParameterConfig ToExpressionParameter()
        {
            return new ParameterConfig()
            {
                nameOrPrefix = Name,
                defaultValue = DefaultValue,
                syncType = ExpressionParameterType,
                localOnly = IsLocalOnly,
                saved = IsSaved,
            };
        }
    }
}
