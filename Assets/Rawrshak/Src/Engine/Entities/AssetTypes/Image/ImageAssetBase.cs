using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Rawrshak
{
    public abstract class ImageAssetBase : AssetBase
    {
        // Todo: Add Image ContentType
        protected ImageMetadataBase metadata;
        private Texture2D currentTextureAsset;

        public override void Init(PublicAssetMetadataBase baseMetadata)
        {
            metadata = ImageMetadataBase.Parse(baseMetadata.jsonString);
            metadata.jsonString = baseMetadata.jsonString;
        }

        public async Task<Texture2D> LoadAndSetTexture2D(int width, int height)
        {
            if (metadata.assetProperties.Length == 0)
            {
                Debug.LogError("[ImageAssetBase] No image asset uri available");
                return null;
            }

            // Check if the currently loaded asset has the same resolution
            if (currentTextureAsset != null &&
                currentTextureAsset.width == width &&
                currentTextureAsset.height == height)
            {
                return currentTextureAsset;
            }

            // Find the resolution if it exists
            string uri = String.Empty;
            for (int i = 0; i < metadata.assetProperties.Length; ++i)
            {
                if (width == metadata.assetProperties[i].width && 
                    height == metadata.assetProperties[i].height)
                {
                    uri = metadata.assetProperties[i].uri;
                }
            }

            // resolution doesn't exists
            if (String.IsNullOrEmpty(uri))
            {
                Debug.LogError("[ImageAssetBase] Resolution is not found");
                return null;
            }
            
            // resolution found
            Texture2D downloadedTexture = await Downloader.DownloadTexture(uri);

            // verify that the downloaded texture has the correct resolution
            if (downloadedTexture.width != width || downloadedTexture.height != height)
            {
                Debug.LogError("[ImageAssetBase] Incorrect Metadata for downloaded texture2d object");
                return null;
            }

            currentTextureAsset = downloadedTexture;
            return currentTextureAsset;
        }

        public Texture2D GetCurrentTexture2D()
        {
            return currentTextureAsset;
        }

        public int GetCurrentWidth()
        {
            if (currentTextureAsset == null)
            {
                return 0;
            }
            return currentTextureAsset.width;
        }

        public int GetCurrentHeight()
        {
            if (currentTextureAsset == null)
            {
                return 0;
            }
            return currentTextureAsset.height;
        }
        
        public List<List<int>> GetAvailableResolutions()
        {
            List<List<int>> resolutions = new List<List<int>>();
            for (int i = 0; i < metadata.assetProperties.Length; ++i)
            {
                List<int> resolution = new List<int>();
                resolution.Add(metadata.assetProperties[i].width);
                resolution.Add(metadata.assetProperties[i].height);
                resolutions.Add(resolution);
            }
            return resolutions;
        }

        protected static bool VerifyAspectRatio(float height, float width, float aspectRatio)
        {
            return height * aspectRatio == width;
        }
    }
}
