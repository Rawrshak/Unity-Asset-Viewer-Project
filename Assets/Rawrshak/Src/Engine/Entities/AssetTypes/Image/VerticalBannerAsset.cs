using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rawrshak
{
    public class VerticalBannerAsset : ImageAssetBase
    {
        public static float VerticalBannerAspectRatio = 0.5f;
        public override bool IsValidAsset()
        {
            foreach(var img in metadata.assetProperties)
            {
                if (!VerifyAspectRatio((float)img.height, (float)img.width, VerticalBannerAspectRatio))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
