// RayProfiler
// https://github.com/Bugfire

using UnityEngine;

namespace RayStorm
{
    /*
	 * If no DefaultExecutionOrder is available, following altanative ways exists
	 * 1. Setup by using GUI : Edit > ProjectSettings > ScriptExecution Order
	 * 2. Setup by modifying meta file : executionOrder
	 */
    #if UNITY_5_5_OR_NEWER
    [DefaultExecutionOrder (int.MinValue)]
    #endif
    sealed public class RayProfilerFirstComponent : RayProfilerMarker
    {
        RayProfilerFirstComponent ()
            : base (RayProfiler.Order.First)
        {
        }

    }

}