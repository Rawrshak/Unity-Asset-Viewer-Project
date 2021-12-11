using UnityEngine;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Rawrshak
{
    // This Static 3D Object Asset Metadata is based off of the Rawrshak Metadata Framework (https://github.com/Rawrshak/RawrshakAssetFrameworks/blob/draft-metadata-standards/Drafts/AudioAssetMetadata.md)
    // If you would like to add additional developer properties, you can add them in the 'properties' in the metadata.
    // You need to define your content metadata class based on this one as seen in /Assets/Rawrshak/Sample/Metadata/ContentMetadataSample.cs
    [Serializable]
    public class Static3dObjectMetadataBase : PublicAssetMetadataBase
    {
        public PrefabProperties[] assetProperties;

        public static new Static3dObjectMetadataBase Parse(string jsonString)
        {
            return JsonUtility.FromJson<Static3dObjectMetadataBase>(jsonString);
        }
    }

    [Serializable]
    public class PrefabProperties
    {
        public string name;
        public string uri;
        public string engine;
        public string renderPipeline;
        public string platform;
        public string fidelity;
        public string shape;
    }
}