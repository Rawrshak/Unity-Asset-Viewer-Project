using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rawrshak
{
    public class CharacterLineAsset : AudioAssetBase
    {
        public override bool IsValidAsset()
        {
            foreach(var audio in audioData)
            {
                if (audio.Value.duration > (int)MaxDurationMs.CharacterLine)
                {
                    return false;
                }   
            }
            return true;
        }
    }
}