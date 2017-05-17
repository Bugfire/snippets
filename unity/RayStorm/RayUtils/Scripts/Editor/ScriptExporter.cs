﻿// RayUtils
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
            string _MyPackage = _AssetBase + "/RayProfiler.unitypackage";
            string[] _MyAssets = new string[] {
                _AssetBase + "/RayCanvas",
                _AssetBase + "/RayProfiler",
                _AssetBase + "/RayUtils",
            };
            AssetDatabase.ExportPackage (_MyAssets, _MyPackage, ExportPackageOptions.Recurse);
            UnityEngine.Debug.LogFormat ("Completed to write {0}.", _MyPackage);
        }

        #endregion

    }
}
