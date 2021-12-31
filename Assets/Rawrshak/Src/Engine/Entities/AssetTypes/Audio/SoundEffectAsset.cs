using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rawrshak
{
    public class SoundEffectAsset : AudioAssetBase
    {
        public override bool IsValidAsset()
        {
            foreach(var audio in audioData)
            {
                if (audio.Value.duration > (int)MaxDurationMs.SoundEffect)
                {
                    return false;
                }   
            }
            return true;
        }
    }
}