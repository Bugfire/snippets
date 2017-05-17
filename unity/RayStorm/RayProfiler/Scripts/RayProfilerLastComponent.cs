// RayProfiler
// https://github.com/Bugfire

using UnityEngine;

namespace RayStorm
{
    /* See RayProfilerFirstComponent.cs about DefaultExecutionOrder. */
    #if UNITY_5_5_OR_NEWER
    [DefaultExecutionOrder (int.MaxValue)]
    #endif
    sealed public class RayProfilerLastComponent : RayProfilerMarker
    {
        RayProfilerLastComponent ()
            : base (RayProfiler.Order.Last)
        {
        }
    }

}