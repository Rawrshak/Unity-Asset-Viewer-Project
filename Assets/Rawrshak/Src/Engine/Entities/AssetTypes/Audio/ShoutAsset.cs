using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rawrshak
{
    public class ShoutAsset : AudioAssetBase
    {
        public override bool IsValidAsset()
        {
            foreach(var audio in audioData)
            {
                if (audio.Value.duration > (int)MaxDurationMs.Shout)
                {
                    return false;
                }   
            }
            return true;
        }
    }
}