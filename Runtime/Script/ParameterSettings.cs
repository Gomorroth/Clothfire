using System;

namespace gomoru.su.clothfire
{
    [Serializable]
    public struct ParameterSettings
    {
        public static ParameterSettings Default => new ParameterSettings(isSave: true, isLocal: false);

        public bool IsSave;
        public bool IsLocal;

        public ParameterSettings(bool isSave = true, bool isLocal = false)
        {
            IsSave = isSave;
            IsLocal = isLocal;
        }
    }
}