using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rawrshak
{
    public class TitleAsset : TextAssetBase
    {
        public override bool IsValidAsset()
        {
            if (metadata.assetProperties.description.Length > (int)MaxDescriptionLength.Title)
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
