using System;

namespace gomoru.su.clothfire
{
    [Serializable]
    public struct Condition
    {
        public string Path;
        public bool State;

        public Condition(string path, bool state = true)
        {
            Path = path;
            State = state;
        }

        public override int GetHashCode() => new HashCode().Append(Path).Append(State).GetHashCode();
    }
}
