using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using Rawrshak;

namespace Rawrshak 
{
    public class AssetBundleMenuConfig : ScriptableObject
    {
        public string assetBundleFolder;
        public SupportedBuildTargets buildTarget;

        public static AssetBundleMenuConfig CreateInstance()
        {
            var data = ScriptableObject.CreateInstance<AssetBundleMenuConfig>();
            
            data.buildTarget = SupportedBuildTargets.StandaloneWindows;
            data.assetBundleFolder = "AssetBundles/StandaloneWindows";

            return data;
        }
    }
}