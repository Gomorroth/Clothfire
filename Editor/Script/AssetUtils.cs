using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.Bindings;
using Object = UnityEngine.Object;

namespace gomoru.su.clothfire
{
    internal static class AssetUtils
    {
        public static GUID GetGUID(this Object obj)
        {
            if (GetGUIDAndLocalIdentifierInFile(obj.GetInstanceID(), out var guid, out long _))
            {
                return guid;
            }
            return default;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [FreeFunction("AssetDatabase::GetGUIDAndLocalIdentifierInFile")]
        private static extern bool GetGUIDAndLocalIdentifierInFile(int instanceID, out GUID outGuid, out long outLocalId);
    }
}
namespace UnityEngine.Bindings
{

    [AttributeUsage(AttributeTargets.Method)]
    internal class FreeFunctionAttribute : Attribute
    {
        public FreeFunctionAttribute()
        {
        }

        public FreeFunctionAttribute(string name) { }
        

        public FreeFunctionAttribute(string name, bool isThreadSafe) { }
    }
}