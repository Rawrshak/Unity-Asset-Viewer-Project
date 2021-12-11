using UnityEngine;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Rawrshak
{
    // This Text Asset Metadata is based off of the Rawrshak Metadata Framework (https://github.com/Rawrshak/RawrshakAssetFrameworks/blob/draft-metadata-standards/Drafts/ImageAssetMetadata.md)
    // If you would like to add additional developer properties, you can add them in the 'properties' in the metadata.
    // You need to define your content metadata class based on this one as seen in /Assets/Rawrshak/Sample/Metadata/ContentMetadataSample.cs
    [Serializable]
    public class TextMetadataBase : PublicAssetMetadataBase
    {
        public TextProperties assetProperties;

        public static new TextMetadataBase Parse(string jsonString)
        {
            return JsonUtility.FromJson<TextMetadataBase>(jsonString);
        }
    }

    [Serializable]
    public class TextProperties
    {
        public string title;
        public string description;
    }
}