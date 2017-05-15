// RayProfiler
// https://github.com/Bugfire

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace RayStorm
{

	[DefaultExecutionOrder (int.MaxValue - 2)]
	sealed public class RayProfilerToCanvas : MonoBehaviour
	{
		#region Inspector Settings Values

		[SerializeField]
		RayProfiler _Profiler;

		[SerializeField]
		RayCanvas _Canvas;

		#endregion

		#region Unity Messages

		void Awake ()
		{
			Assert.IsNotNull (_Profiler, "_Profiler is not defined on " + RayDebugUtils.GetObjectName (this));
			Assert.IsNotNull (_Canvas, "_Canvas is not defined on " + RayDebugUtils.GetObjectName (this));
		}

		void Update ()
		{
			const int barWidth = 320;
			const int barHeight = 32;
			const int lineWidth = 2;
			var ticks = _Profiler.LastTicks;

			var targetFps = (int)Application.targetFrameRate;
			if (targetFps == 0) {
				targetFps = 60;
			}
			var totalTicks = 10000000 / targetFps;
			var ticksUnit = totalTicks / (float)(barWidth - lineWidth * 2);

			_Canvas.Clear ();
			_Canvas.FillRect (0, 0, barWidth, barHeight, Color.white);

			const int sy = lineWidth;
			const int ey = barHeight - lineWidth;
			_Canvas.FillRect (lineWidth, sy, barWidth - lineWidth, ey, Color.black);
			var pos = lineWidth;
			var ammount = 0;

			// FixedUpdate
			ammount = (int)(ticks.FixedUpdate / ticksUnit);
			_Canvas.FillRect (pos, sy, pos + ammount, ey, Color.blue);
			pos += ammount;

			// Physics
			ammount = (int)(ticks.Physics / ticksUnit);
			_Canvas.FillRect (pos, sy, pos + ammount, ey, Color.green);
			pos += ammount;

			// WaitForFixedUpdate
			ammount = (int)(ticks.WaitForFixedUpdate / ticksUnit);
			_Canvas.FillRect (pos, sy, pos + ammount, ey, Color.cyan);
			pos += ammount;

			// Update
			ammount = (int)(ticks.Update / ticksUnit);
			_Canvas.FillRect (pos, sy, pos + ammount, ey, Color.yellow);
			pos += ammount;

			// YieldAndAnimation
			ammount = (int)(ticks.YieldAndAnimation / ticksUnit);
			_Canvas.FillRect (pos, sy, pos + ammount, ey, Color.magenta);
			pos += ammount;

			// LateUpdate
			ammount = (int)(ticks.LateUpdate / ticksUnit);
			_Canvas.FillRect (pos, sy, pos + ammount, ey, Color.grey);
			pos += ammount;

			// Render
			ammount = (int)(ticks.Render / ticksUnit);
			_Canvas.FillRect (pos, sy, pos + ammount, ey, Color.red);
			pos += ammount;

			// WaitForEndOfFrame
			ammount = (int)(ticks.WaitForEndOfFrame / ticksUnit);
			_Canvas.FillRect (pos, sy, pos + ammount, ey, Color.white);
			pos += ammount;

			// FrameEnd
			ammount = (int)(ticks.FrameEnd / ticksUnit);
			_Canvas.FillRect (pos, sy, pos + ammount, ey, new Color (1, 1, 1, 0.5f));
			pos += ammount;

			//
			var fps = Time.deltaTime != 0 ? 1 / Time.deltaTime : 0;
			_Canvas.SetTextPosition (0, barHeight);
			_Canvas.AddText ("FPS:");
			_Canvas.AddText (fps, 3, 2);
			_Canvas.AddText (" GC: ");
			_Canvas.AddText (ticks.GCCount);
		}

		#endregion
	}

}