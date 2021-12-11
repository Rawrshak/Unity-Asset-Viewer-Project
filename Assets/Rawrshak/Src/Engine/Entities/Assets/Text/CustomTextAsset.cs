using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rawrshak
{
    public class CustomTextAsset : TextAssetBase
    {
        public override bool IsValidAsset()
        {
            if (metadata.assetProperties.title.Length > MaxTitleLength)
            {
                return false;
            }

            // Custom text assets do not have a max length
            return true;
        }
    }
}
