// RayProfiler
// https://github.com/Bugfire

using System.Collections;
using System.Collections.Generic;
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