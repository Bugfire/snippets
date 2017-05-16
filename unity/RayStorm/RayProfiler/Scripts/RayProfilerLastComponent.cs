// RayProfiler
// https://github.com/Bugfire

using UnityEngine;

namespace RayStorm
{
    /* See RayProfilerFirstComponent.cs about DefaultExecutionOrder. */
    [DefaultExecutionOrder (int.MaxValue)]
    sealed public class RayProfilerLastComponent : RayProfilerMarker
    {
        RayProfilerLastComponent ()
            : base (RayProfiler.Order.Last)
        {
        }
    }

}