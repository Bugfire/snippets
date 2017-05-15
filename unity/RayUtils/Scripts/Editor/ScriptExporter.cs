// RayUtils
// https://github.com/Bugfire

using UnityEditor;

namespace RayStorm
{
	
	static class ScriptExporter
	{

		#region menu item

		[MenuItem ("Plugin/Management/ExportRayProfiler")]
		static void ExportRayProfiler ()
		{
			string _MyPackage = "Assets/Plugin/RayProfiler.unitypackage";
			string[] _MyAssets = new string[] {
				"Assets/Plugin/RayCanvas",
				"Assets/Plugin/RayProfiler",
				"Assets/Plugin/RayUtils",
			};
			AssetDatabase.ExportPackage (_MyAssets, _MyPackage, ExportPackageOptions.Recurse);
		}

		#endregion

	}
}
