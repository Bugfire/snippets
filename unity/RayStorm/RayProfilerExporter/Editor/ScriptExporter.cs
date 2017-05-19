// RayUtils
// https://github.com/Bugfire

using UnityEditor;

namespace RayStorm
{
	
    static class ScriptExporter
    {

        #region menu item

        [MenuItem ("Plugins/Management/ExportRayProfiler")]
        static void ExportRayProfiler ()
        {
            string _AssetBase = "Assets/Plugins/RayStorm";
            string[] _MyAssets = new string[] {
                _AssetBase + "/RayCanvas",
                _AssetBase + "/RayProfiler",
                _AssetBase + "/RayUtils",
            };
            var version = UnityEngine.Application.unityVersion; // 5.4.5f1
            var s = version.Split ('.');
            string _MyPackage = _AssetBase + "/RayProfiler" + s [0] + "_" + s [1] + ".unitypackage";
            AssetDatabase.ExportPackage (_MyAssets, _MyPackage, ExportPackageOptions.Recurse | ExportPackageOptions.Interactive);
            UnityEngine.Debug.LogFormat ("Completed to write {0}.", _MyPackage);
        }

        #endregion

    }
}
