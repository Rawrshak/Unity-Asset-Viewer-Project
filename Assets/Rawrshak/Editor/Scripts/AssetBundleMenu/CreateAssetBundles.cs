using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using Rawrshak;

public class CreateAssetBundles
{
    public static void BuildAllAssetBundles(SupportedBuildTargets buildTarget, string assetBundleDirectory)
    {
        string directory = "Assets/" + assetBundleDirectory;
        if(!Directory.Exists(directory))
        {
            Debug.Log("Directory is being created: " + directory);
            Directory.CreateDirectory(directory);
        }
        BuildPipeline.BuildAssetBundles(directory,
                                        BuildAssetBundleOptions.None, 
                                        ConvertToBuildTarget(buildTarget));
        AssetDatabase.Refresh();
    }

    static BuildTarget ConvertToBuildTarget(SupportedBuildTargets buildTarget)
    {
        switch (buildTarget) {
            case SupportedBuildTargets.StandaloneWindows64:
                return BuildTarget.StandaloneWindows64;
            case SupportedBuildTargets.Android:
                return BuildTarget.Android;
            case SupportedBuildTargets.WebGL:
                return BuildTarget.WebGL;
            case SupportedBuildTargets.iOS:
                return BuildTarget.iOS;
            case SupportedBuildTargets.StandaloneWindows:
            default:
                return BuildTarget.StandaloneWindows;
        }
    }
}