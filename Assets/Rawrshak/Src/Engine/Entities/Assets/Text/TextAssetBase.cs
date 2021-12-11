using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rawrshak
{
    public abstract class TextAssetBase : AssetBase
    {
        public static int MaxTitleLength = 40;
        public enum MaxDescriptionLength {
            Title = 500,
            Lore = 5000
        };

        protected TextMetadataBase metadata;

        public override void Init(PublicAssetMetadataBase baseMetadata)
        {
            metadata = TextMetadataBase.Parse(baseMetadata.jsonString);
            metadata.jsonString = baseMetadata.jsonString;
        }

        public string GetTitle()
        {
            return metadata.assetProperties.title;
        }
        
        public string GetDescription()
        {
            return metadata.assetProperties.description;
        }
    }
}
