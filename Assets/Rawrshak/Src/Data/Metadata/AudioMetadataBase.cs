using UnityEngine;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Rawrshak
{
    // This Audio Asset Metadata is based off of the Rawrshak Metadata Framework (https://github.com/Rawrshak/RawrshakAssetFrameworks/blob/draft-metadata-standards/Drafts/AudioAssetMetadata.md)
    // If you would like to add additional developer properties, you can add them in the 'properties' in the metadata.
    // You need to define your content metadata class based on this one as seen in /Assets/Rawrshak/Sample/Metadata/ContentMetadataSample.cs
    [Serializable]
    public class AudioMetadataBase : PublicAssetMetadataBase
    {
        public AudioProperties[] assetProperties;

        public static new AudioMetadataBase Parse(string jsonString)
        {
            return JsonUtility.FromJson<AudioMetadataBase>(jsonString);
        }
    }

    [Serializable]
    public class AudioProperties
    {
        public string name;
        public string uri;
        public string contentType;
        public int duration;
    }
}