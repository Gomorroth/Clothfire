namespace gomoru.su.clothfire.ndmf
{
    internal static class VRCParameterConversion
    {
        public static float ToSingle<T>(T value)
        {
            if (typeof(T) == typeof(float))
                return (float)(object)value;
            if (typeof(T) == typeof(int))
                return (int)(object)value;
            if (typeof(T) == typeof(bool))
                return (bool)(object)value ? 1 : 0;

            return default;
        }

        public static T FromSingle<T>(float value)
        {
            if (typeof(T) == typeof(float))
                return (T)(object)value;
            if (typeof(T) == typeof(int))
                return (T)(object)(int)value;
            if (typeof(T) == typeof(bool))
                return (T)(object)(value != 0);

            return default;
        }

    }
}
