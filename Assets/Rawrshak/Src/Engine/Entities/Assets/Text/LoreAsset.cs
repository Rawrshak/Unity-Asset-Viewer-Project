using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rawrshak
{
    public class LoreAsset : TextAssetBase
    {
        public override bool IsValidAsset()
        {
            if (metadata.assetProperties.description.Length > (int)MaxDescriptionLength.Lore)
            {
                return false;
            }

            if (metadata.assetProperties.title.Length > MaxTitleLength)
            {
                return false;
            }

            return true;
        }
    }
}
