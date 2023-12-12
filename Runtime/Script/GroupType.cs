using System;
using System.Collections.Generic;
using System.Text;

namespace gomoru.su.clothfire
{
    [Flags]
    public enum GroupType : int
    {
        None = 0,
        Control = 1,
        Preset = 2,
        All = -1,
    }
}
