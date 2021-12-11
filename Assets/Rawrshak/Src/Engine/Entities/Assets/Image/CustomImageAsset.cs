using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rawrshak
{
    public class CustomImageAsset : ImageAssetBase
    {
        public override bool IsValidAsset()
        {
            // Custom text assets do not have a max length
            return true;
        }
    }
}
