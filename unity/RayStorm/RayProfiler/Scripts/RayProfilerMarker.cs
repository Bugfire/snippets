// RayProfiler
// https://github.com/Bugfire

using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace RayStorm
{
    public class RayProfilerMarker : MonoBehaviour
    {
        #region Inspector Settings Values

        [SerializeField]
        RayProfiler _Master;

        #endregion

        readonly RayProfiler.Order _Order;

        Coroutine _MarkWaitForFixedUpdate;
        Coroutine _MarkWaitForEndOfFrame;

        protected RayProfilerMarker (RayProfiler.Order order)
        {
            _Order = order;
        }

        #region Unity Messages

        void Awake ()
        {
            Assert.IsNotNull (_Master, "_Master is not defined on " + RayDebugUtils.GetObjectName (this));
        }

        void OnEnable ()
        {
            _MarkWaitForFixedUpdate = StartCoroutine (MarkWaitForFixedUpdate ());
            _MarkWaitForEndOfFrame = StartCoroutine (MarkWaitForEndOfFrame ());
        }

        void OnDisable ()
        {
            if (_MarkWaitForFixedUpdate != null) {
                StopCoroutine (_MarkWaitForFixedUpdate);
                _MarkWaitForFixedUpdate = null;
            }
            if (_MarkWaitForEndOfFrame != null) {
                StopCoroutine (_MarkWaitForEndOfFrame);
                _MarkWaitForEndOfFrame = null;
            }
        }

        void FixedUpdate ()
        {
            _Master.Mark (RayProfiler.Marker.FixedUpdate, _Order);
        }

        void Update ()
        {
            _Master.Mark (RayProfiler.Marker.Update, _Order);
        }

        void LateUpdate ()
        {
            _Master.Mark (RayProfiler.Marker.LateUpdate, _Order);
        }

        #endregion

        IEnumerator MarkWaitForFixedUpdate ()
        {
            var w = new WaitForFixedUpdate ();
            while (true) {
                yield return w;
                _Master.Mark (RayProfiler.Marker.WaitForFixedUpdate, _Order);
            }
        }

        IEnumerator MarkWaitForEndOfFrame ()
        {
            var w = new WaitForEndOfFrame ();
            while (true) {
                yield return w;
                #if UNITY_EDITOR
                if (UnityEditor.EditorApplication.isPaused) {
                    continue;
                }
                #endif
                _Master.Mark (RayProfiler.Marker.WaitForEndOfFrame, _Order);
            }
        }

    }

}