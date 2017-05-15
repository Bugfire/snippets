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

		const int LOG_MAX = 60;
		int[] _wofLog = new int[LOG_MAX];
		int[] _fendLog = new int[LOG_MAX];
		int _curLog = 0;

		#region Unity Messages

		void Awake ()
		{
			Assert.IsNotNull (_Profiler, "_Profiler is not defined on " + RayDebugUtils.GetObjectName (this));
			Assert.IsNotNull (_Canvas, "_Canvas is not defined on " + RayDebugUtils.GetObjectName (this));
		}

		void Update ()
		{
			const int barWidth = 320;
			const int barHeight = 16;
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

			_curLog++;
			if (_curLog >= LOG_MAX) {
				_curLog = 0;
			}

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
			_wofLog [_curLog] = pos;

			// FrameEnd
			ammount = (int)(ticks.FrameEnd / ticksUnit);
			_Canvas.FillRect (pos, sy, pos + ammount, ey, new Color (1, 1, 1, 0.5f));
			pos += ammount;
			_fendLog [_curLog] = pos;

			for (var i = 0; i < LOG_MAX; i++) {
				int index = _curLog - i;
				if (index < 0) {
					index += LOG_MAX;
				}
				_Canvas.FillRect (0, barHeight + i * 2, _wofLog [index], barHeight + i * 2 + 2, new Color (0.5f, 1, 0.5f, 0.7f));
				_Canvas.FillRect (_wofLog [index], barHeight + i * 2, _fendLog [index], barHeight + i * 2 + 2, new Color (0.5f, 0.5f, 0.5f, 0.7f));
			}

			//
			var fps = Time.unscaledDeltaTime != 0 ? 1 / Time.unscaledDeltaTime : 0;
			_Canvas.SetTextPosition (8, barHeight);
			_Canvas.AddText ("FPS:");
			_Canvas.AddText (fps, 3, 2);
		}

		#endregion
	}

}