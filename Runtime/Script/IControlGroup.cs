using UnityEngine;

namespace gomoru.su.clothfire
{
    public interface IControlGroup
    {
        string GroupName { get; }
        GameObject GroupMaster { get; }
    }
}