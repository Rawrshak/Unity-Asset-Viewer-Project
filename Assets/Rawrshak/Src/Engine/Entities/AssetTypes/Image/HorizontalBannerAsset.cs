using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rawrshak
{
    public class HorizontalBannerAsset : ImageAssetBase
    {
        public static float HorizontalBannerAspectRatio = 2.0f;
        public override bool IsValidAsset()
        {
            foreach(var img in metadata.assetProperties)
            {
                if (!VerifyAspectRatio((float)img.height, (float)img.width, HorizontalBannerAspectRatio))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
