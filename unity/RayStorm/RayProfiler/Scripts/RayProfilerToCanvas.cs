// RayProfiler
// https://github.com/Bugfire

using UnityEngine;
using UnityEngine.Assertions;

#if UNITY_5_5_OR_NEWER
using UnityEngine.Profiling;
#endif

namespace RayStorm
{
    #if UNITY_5_5_OR_NEWER
    [DefaultExecutionOrder (int.MaxValue - 2)]
    #endif
    sealed public class RayProfilerToCanvas : MonoBehaviour
    {
        #region Inspector Settings Values

        [SerializeField]
        RayProfiler _Profiler;

        [SerializeField]
        RayCanvas _Canvas;

        #endregion

        struct Log
        {
            public int WaitForEndOfFrame;
            public int FrameEnd;
            public bool GC;
        }

        Log[] _Log = new Log[60];
        int _curLog = 0;

        System.Diagnostics.Stopwatch _stopWatch = new System.Diagnostics.Stopwatch ();
        float _lastAvgFPS = -1;
        int _lastAvgFPSCounter = 0;

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
            if (targetFps <= 0) {
                targetFps = 60;
            }
            var totalTicks = 10000000 / targetFps;
            var ticksUnit = totalTicks / (float)(barWidth - lineWidth * 2);

            _Canvas.Clear ();

            _Canvas.FillRect (0, 0, _Canvas.Width, _Canvas.Height, new Color (0, 0, 0, 0.5f));
            _Canvas.FillRect (0, 0, barWidth, barHeight, Color.white);

            const int sy = lineWidth;
            const int ey = barHeight - lineWidth;
            _Canvas.FillRect (lineWidth, sy, barWidth - lineWidth, ey, Color.black);
            var pos = lineWidth;
            var ammount = 0;

            _curLog++;
            if (_curLog >= _Log.Length) {
                _curLog = 0;
            }

            // FixedUpdate
            ammount = (int)(ticks.FixedUpdate / ticksUnit);
            _Canvas.FillRect (pos, sy, pos + ammount, ey, Color.yellow);
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
            _Canvas.FillRect (pos, sy, pos + ammount, ey, Color.blue);
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
            _Log [_curLog].WaitForEndOfFrame = pos;

            // FrameEnd
            ammount = (int)(ticks.FrameEnd / ticksUnit);
            _Canvas.FillRect (pos, sy, pos + ammount, ey, new Color (1, 1, 1, 0.5f));
            pos += ammount;
            _Log [_curLog].FrameEnd = pos;

            _Log [_curLog].GC = ticks.GCCount != 0;

            for (var i = 0; i < _Log.Length; i++) {
                int index = _curLog - i;
                if (index < 0) {
                    index += _Log.Length;
                }
                _Canvas.FillRect (
                    0, barHeight + i * 2, _Log [index].WaitForEndOfFrame, barHeight + i * 2 + 2,
                    _Log [index].GC ? new Color (1f, 1, 0.5f, 1) : new Color (0.5f, 1, 0.5f, 0.7f));
                _Canvas.FillRect (
                    _Log [index].WaitForEndOfFrame, barHeight + i * 2, _Log [index].FrameEnd, barHeight + i * 2 + 2,
                    _Log [index].GC ? new Color (1f, 0.5f, 0.5f, 1) : new Color (1f, 1f, 1f, 0.5f));
            }

            //
            var fps = Time.unscaledDeltaTime != 0 ? 1 / Time.unscaledDeltaTime : 0;

            var restartStopWatch = false;
            _lastAvgFPSCounter++;
            if (_lastAvgFPS < 0) {
                _lastAvgFPS = fps;
                restartStopWatch = true;
            } else if (_stopWatch.ElapsedMilliseconds > 1000 && _lastAvgFPSCounter > 10) {
                _lastAvgFPS = _lastAvgFPSCounter * 1000.0f / _stopWatch.ElapsedMilliseconds;
                restartStopWatch = true;
            }
            if (restartStopWatch == true) {
                _lastAvgFPSCounter = 0;
                _stopWatch.Stop ();
                _stopWatch.Reset ();
                _stopWatch.Start ();
            }

            _Canvas.SetTextPosition (8, barHeight);
            _Canvas.AddText ("FPS:");
            _Canvas.AddText (fps, 3, 2);
            _Canvas.AddText (" AVG.FPS:");
            _Canvas.AddText (_lastAvgFPS, 3, 2);
            #if UNITY_5_6_OR_NEWER
            if (UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong () >= 1024) {
                _Canvas.AddText ("\nVM:   ");
                _Canvas.AddText ((int)Profiler.GetMonoUsedSizeLong () / 1024, 6);
                _Canvas.AddText ("/");
                _Canvas.AddText ((int)Profiler.GetMonoHeapSizeLong () / 1024, 6);
            }
            if (UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong () >= 1024) {
                _Canvas.AddText ("\nTotal:");
                _Canvas.AddText ((int)Profiler.GetTotalAllocatedMemoryLong () / 1024, 6);
                _Canvas.AddText ("/");
                _Canvas.AddText ((int)Profiler.GetTotalReservedMemoryLong () / 1024, 6);
            }
            #else
            if (Profiler.GetMonoUsedSize () >= 1024) {
                _Canvas.AddText ("\nVM:   ");
                _Canvas.AddText ((int)Profiler.GetMonoUsedSize () / 1024, 6);
                _Canvas.AddText ("/");
                _Canvas.AddText ((int)Profiler.GetMonoHeapSize () / 1024, 6);
            }
            if (Profiler.GetTotalAllocatedMemory () >= 1024) {
                _Canvas.AddText ("\nTotal:");
                _Canvas.AddText ((int)Profiler.GetTotalAllocatedMemory () / 1024, 6);
                _Canvas.AddText ("/");
                _Canvas.AddText ((int)Profiler.GetTotalReservedMemory () / 1024, 6);
            }
            #endif
        }

        #endregion
    }

}