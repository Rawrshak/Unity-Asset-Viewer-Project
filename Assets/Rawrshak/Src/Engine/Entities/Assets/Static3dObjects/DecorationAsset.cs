using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rawrshak
{
    public class DecorationAsset : Static3dObjectAssetBase
    {
        public override bool IsValidAsset()
        {
            // Todo: calculate that it fits within the bounds of the 'shape'
            return true;
        }
    }
}