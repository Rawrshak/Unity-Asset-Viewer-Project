using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rawrshak
{
    public abstract class AssetBase : ScriptableObject
    {
        public abstract void Init(PublicAssetMetadataBase baseMetadata);
        public abstract bool IsValidAsset();
    }
}