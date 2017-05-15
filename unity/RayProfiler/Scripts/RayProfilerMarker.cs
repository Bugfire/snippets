// RayProfiler
// https://github.com/Bugfire

using System.Collections;
using System.Collections.Generic;
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

		readonly RayProfiler.Order Order;

		protected RayProfilerMarker (RayProfiler.Order order)
		{
			Order = order;
		}

		#region Unity Messages

		void Awake ()
		{
			Assert.IsNotNull (_Master, "_Master is not defined on " + RayDebugUtils.GetObjectName (this));

			StartCoroutine (MarkWaitForFixedUpdate ());
			StartCoroutine (MarkWaitForEndOfFrame ());
		}

		void FixedUpdate ()
		{
			_Master.Mark (RayProfiler.Marker.FixedUpdate, Order);
		}

		void Update ()
		{
			_Master.Mark (RayProfiler.Marker.Update, Order);
		}

		void LateUpdate ()
		{
			_Master.Mark (RayProfiler.Marker.LateUpdate, Order);
		}

		#endregion

		IEnumerator MarkWaitForFixedUpdate ()
		{
			var w = new WaitForFixedUpdate ();
			while (true) {
				yield return w;
				_Master.Mark (RayProfiler.Marker.WaitForFixedUpdate, Order);
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
				_Master.Mark (RayProfiler.Marker.WaitForEndOfFrame, Order);
			}
		}

	}

}