// RayProfiler
// https://github.com/Bugfire

using UnityEngine;
using UnityEngine.Assertions;

namespace RayStorm
{
    sealed public class RayProfiler : MonoBehaviour
    {
        // See https://docs.unity3d.com/Manual/ExecutionOrder.html
        public enum Order : int
        {
            First,
            Last,
        };

        public enum Marker : int
        {
            Undefined = 0,

            // {
            FixedUpdate,
            // Physics
            // OnTrigerXXX
            // OnCollisionXXX
            WaitForFixedUpdate,
            // } * N

            // OnMouseXXX

            Update,
            // yield null
            // yield WaitForSeconds
            // yield WWW
            // yield StartCoroutine
            // Internal animation update

            LateUpdate,

            // OnWillRenderObject
            // OnPreCull
            // OnBecameVisible
            // OnBecameInvisible
            // OnPreRender
            // OnRenderObject
            // OnPostRender
            // OnRenderImage

            // [EDITOR OnDrawGizmos]

            // OnGUI * N

            WaitForEndOfFrame,
        };

        public class Ticks
        {
            // 1ticks = 100ns = 0.1us = 0.00001ms
            public long FixedUpdate;
            public long Physics;
            public long WaitForFixedUpdate;
            public long Update;
            public long YieldAndAnimation;
            public long LateUpdate;
            public long Render;
            public long WaitForEndOfFrame;
            public long FrameEnd;

            public int FixedUpdateCount;
            public int GCCount;

            public void Reset ()
            {
                FixedUpdate = 0;
                Physics = 0;
                WaitForFixedUpdate = 0;
                Update = 0;
                YieldAndAnimation = 0;
                LateUpdate = 0;
                Render = 0;
                WaitForEndOfFrame = 0;
                FrameEnd = 0;

                FixedUpdateCount = 0;
                _lastGCCount = System.GC.CollectionCount (0);
            }

            int _lastGCCount;

            public void Finish ()
            {
                GCCount = System.GC.CollectionCount (0) - _lastGCCount;
            }
        }

        Marker _lastMarker;
        System.Diagnostics.Stopwatch _stopwatch;
        Ticks[] _ticks = new Ticks[2] { new Ticks (), new Ticks () };
        Ticks _cur;
        Ticks _last;
        bool _start;

        public Ticks LastTicks { get { return _last; } }

        #region Unity Messages

        void Start ()
        {
            _lastMarker = Marker.Undefined;
            _stopwatch = new System.Diagnostics.Stopwatch ();
            _stopwatch.Start ();
            _ticks [0].Reset ();
            _ticks [1].Reset ();
            _cur = _ticks [0];
            _last = _ticks [1];
            _start = false;
        }

        void Update ()
        {
        }

        #endregion

        public void Mark (Marker marker, Order order)
        {
            _stopwatch.Stop ();
            var ticks = _stopwatch.ElapsedTicks;
            _stopwatch.Reset ();
            _stopwatch.Start ();

            if (_start == false) {
                // Start profiling after first WaitForEndOfFrame=>FixedUpdate
                if (_lastMarker == Marker.WaitForEndOfFrame && marker == Marker.FixedUpdate) {
                    _start = true;
                } else {
                    _lastMarker = marker;
                    return;
                }
            }

            switch (marker) {
            case Marker.FixedUpdate:
                if (_lastMarker == Marker.WaitForFixedUpdate) {
                    _cur.WaitForFixedUpdate += ticks;
                    _cur.FixedUpdateCount++;
                } else if (_lastMarker == Marker.FixedUpdate) {
                    _cur.FixedUpdate += ticks;
                } else if (_lastMarker == Marker.WaitForEndOfFrame) {
                    _cur.FrameEnd = ticks;
                    Finish ();
                    _cur.FixedUpdateCount++;
                } else {
                    //Debug.LogErrorFormat ("Internal error {0} => {1}", _lastMarker, marker);
                    _cur.Reset ();
                }
                break;
            case Marker.WaitForFixedUpdate:
                if (_lastMarker == Marker.WaitForFixedUpdate) {
                    _cur.WaitForFixedUpdate += ticks;
                } else if (_lastMarker == Marker.FixedUpdate) {
                    _cur.Physics += ticks;
                } else {
                    Debug.LogErrorFormat ("Internal error {0} => {1}", _lastMarker, marker);
                    _cur.Reset ();
                }
                break;
            case Marker.Update:
                if (_lastMarker == Marker.Update) {
                    _cur.Update += ticks;
                } else if (_lastMarker == Marker.WaitForEndOfFrame) {
                    // skipped FixedUpdate()
                    _cur.FrameEnd = ticks;
                    Finish ();
                } else if (_lastMarker == Marker.WaitForFixedUpdate) {
                    // OnMouse...
                    // TODO: ticks to something
                } else {
                    //Debug.LogErrorFormat ("Internal error {0} => {1}", _lastMarker, marker);
                    Reset ();
                }
                break;
            case Marker.LateUpdate:
                if (_lastMarker == Marker.LateUpdate) {
                    _cur.LateUpdate += ticks;
                } else if (_lastMarker == Marker.Update) {
                    _cur.YieldAndAnimation = ticks;
                } else {
                    //Debug.LogErrorFormat ("Internal error {0} => {1}", _lastMarker, marker);
                    Reset ();
                }
                break;
            case Marker.WaitForEndOfFrame:
                if (_lastMarker == Marker.WaitForEndOfFrame) {
                    _cur.WaitForEndOfFrame += ticks;
                } else if (_lastMarker == Marker.LateUpdate) {
                    _cur.Render = ticks;
                } else {
                    //Debug.LogErrorFormat ("Internal error {0} => {1}", _lastMarker, marker);
                    Reset ();
                }
                break;
            }
            //Debug.LogFormat ("{0} {1}", marker, order);

            _lastMarker = marker;
        }

        void Reset ()
        {
            _cur.Reset ();
        }

        void Finish ()
        {
            _cur.Finish ();

            var t = _last;
            _last = _cur;
            _cur = t;
            _cur.Reset ();
        }
    }

}