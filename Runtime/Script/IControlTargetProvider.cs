using System.Collections.Generic;

namespace gomoru.su.clothfire
{
    public interface IControlTargetProvider
    {
        void GetControlTargets(List<ControlTarget> destination);
    }

}