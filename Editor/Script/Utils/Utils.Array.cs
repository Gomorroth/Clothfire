namespace gomoru.su.clothfire
{
    partial class Utils
    {
        public static T[] Single<T>() => SingleArrayCache<T>.Single;

        private static class SingleArrayCache<T>
        {
            public static readonly T[] Single = new T[1];
        }
    }
}
