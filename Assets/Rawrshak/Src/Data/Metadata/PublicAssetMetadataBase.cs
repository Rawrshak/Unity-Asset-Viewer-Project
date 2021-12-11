using UnityEngine;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Rawrshak
{
    // This Public Asset Metadata is based off of the Rawrshak Metadata Framework (https://github.com/Rawrshak/RawrshakAssetFrameworks/blob/draft-metadata-standards/Drafts/PublicAssetMetadata.md)
    // If you would like to add additional developer properties, you can add them in the 'properties' in the metadata.
    // You need to define your content metadata class based on this one as seen in /Assets/Rawrshak/Sample/Metadata/ContentMetadataSample.cs
    [Serializable]
    public class PublicAssetMetadataBase
    {
        public string jsonString;
        public string name;
        public string description;
        public string image;
        public string[] tags;
        public string type;
        public string subtype;

        public static PublicAssetMetadataBase Parse(string jsonString)
        {
            PublicAssetMetadataBase metadata = JsonUtility.FromJson<PublicAssetMetadataBase>(jsonString);
            metadata.jsonString = jsonString;
            return metadata;
        }
    }
}