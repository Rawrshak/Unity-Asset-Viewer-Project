using UnityEngine;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Rawrshak
{
    // This Content Metadata is based off of the Rawrshak Metadata Framework (https://github.com/Rawrshak/RawrshakAssetFrameworks/blob/draft-metadata-standards/Drafts/ContentContractMetadata.md)
    // If you would like to add additional developer properties, you can add them in the 'properties' in the metadata.
    // You need to define your content metadata class based on this one as seen in /Assets/Rawrshak/Sample/Metadata/ContentMetadataSample.cs
    [Serializable]
    public class ContentMetadataBase
    {
        public string name;
        public string description;
        public string image;
        public string game;
        public string creator;
        public string[] tags;

        public static ContentMetadataBase Parse(string jsonString)
        {
            return JsonUtility.FromJson<ContentMetadataBase>(jsonString);
        }
    }
}