// RayUtils
// https://github.com/Bugfire

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RayStorm
{
	
	static public class RayDebugUtils
	{
		static public string GetFullPathName (Transform t)
		{
			if (t.parent != null) {
				return GetFullPathName (t.parent) + "/" + t.gameObject.name;
			} else {
				return t.gameObject.scene.name + ":/" + t.gameObject.name;
			}
		}

		static public string GetObjectName (MonoBehaviour m)
		{
			return string.Format ("{0}({1})", GetFullPathName (m.transform), m.GetType ());
		}

	}

}
