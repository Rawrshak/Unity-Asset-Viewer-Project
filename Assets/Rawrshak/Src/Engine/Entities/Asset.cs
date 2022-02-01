using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using UnityEngine;

namespace Rawrshak
{
    public class Asset : ScriptableObject
    {
        // This is a string because Asset received from the GraphQL may be from other contracts that do
        // not belong to this developer.
        public string contractAddress;
        public string tokenId;
        public string assetName;
        public AssetType type;
        public AssetSubtype subtype;
        public string currentSupply;
        public string maxSupply;
        public string latestPublicUriVersion;
        public string latestHiddenUriVersion;
        public string latestPublicUri;
        public List<string> tags;
        public PublicAssetMetadataBase baseMetadata;
        public string imageUri;
        public Texture2D imageTexture;
        public AssetBase assetComponent;
        
        private Network network;

        void Start()
        {
            network = Network.Instance;
        }

        public async Task<bool> Load()
        {
            GetAssetInfo.ReturnData data = await GetAssetInfo.Fetch(contractAddress, tokenId);

            if (String.IsNullOrEmpty(data.data.asset.id))
            {
                Debug.LogError("Invalid Rawrshak Asset Load. Asset doesn't exist.");
                return false;
            }

            assetName = data.data.asset.name;
            type = ParseAssetType(data.data.asset.type);
            subtype = ParseAssetSubtype(data.data.asset.subtype);
            imageUri = data.data.asset.imageUri;
            currentSupply = data.data.asset.currentSupply;
            maxSupply = data.data.asset.maxSupply;
            latestPublicUriVersion = data.data.asset.latestPublicUriVersion;
            latestHiddenUriVersion = data.data.asset.latestHiddenUriVersion;

            // Todo: The metadata is currently stored on IPFS. The subgraph doesn't support Arweave yet. Update this to pull data from Arweave.
            latestPublicUri = String.Format(Constants.IPFS_QUERY_FORMAT, data.data.asset.latestPublicUri);

            tags = new List<string>();
            foreach(GetAssetInfo.TagData tag in data.data.asset.tags)
            {
                tags.Add(tag.id);
            }

            // Download Metadata
            return await LoadMetadata(latestPublicUri);
        }

        public async Task<bool> LoadImage()
        {
            if (!String.IsNullOrEmpty(imageUri))
            {
                imageTexture = await Downloader.DownloadTexture(imageUri);
                return true;
            }
            return false;
        }

        public void CreateAssetComponent()
        {
            // If the asset component already exists, No-op.
            if (assetComponent) {
                return;
            }

            assetComponent = CreateAssetBase(type, subtype);

            // Initializes the component. If it the asset isn't valid, the component is removed.
            if (assetComponent)
            {
                Debug.Log("Asset Component Created!");
                assetComponent.Init(baseMetadata);
                if (!assetComponent.IsValidAsset())
                {
                    Debug.Log("Asset Component Deleted!");
                    Destroy(assetComponent);
                    assetComponent = null;
                }
            }
        }

        public bool IsTextAsset()
        {
            return type == AssetType.Text;
        }

        public bool IsAudioAsset()
        {
            return type == AssetType.Audio;
        }

        public bool IsImageAsset()
        {
            return type == AssetType.Image;
        }

        public bool IsStatic3dObjectAsset()
        {
            return type == AssetType.Static3dObject;
        }

        // Internal Functions
        private static AssetBase CreateAssetBase(AssetType assetType, AssetSubtype assetSubtype)
        {
            switch (assetSubtype)
            {
                case AssetSubtype.Custom:
                {
                    switch (assetType)
                    {
                        case AssetType.Text:
                        {
                            return ScriptableObject.CreateInstance<CustomTextAsset>();
                        }
                        case AssetType.Image:
                        {
                            return ScriptableObject.CreateInstance<CustomImageAsset>();
                        }
                        case AssetType.Audio:
                        {
                            return ScriptableObject.CreateInstance<CustomAudioAsset>();
                        }
                        default:
                        {
                            Debug.LogError("Invalid asset component to add.");
                            return null;
                        }
                    }
                }
                case AssetSubtype.Title:
                {
                    return ScriptableObject.CreateInstance<TitleAsset>();
                }
                case AssetSubtype.Lore:
                {
                    return ScriptableObject.CreateInstance<LoreAsset>();
                }
                case AssetSubtype.Square:
                {
                    return ScriptableObject.CreateInstance<SquareAsset>();
                }
                case AssetSubtype.HorizontalBanner:
                {
                    return ScriptableObject.CreateInstance<HorizontalBannerAsset>();
                }
                case AssetSubtype.VerticalBanner:
                {
                    return ScriptableObject.CreateInstance<VerticalBannerAsset>();
                }
                case AssetSubtype.SoundEffect:
                {
                    return ScriptableObject.CreateInstance<SoundEffectAsset>();
                }
                case AssetSubtype.Shout:
                {
                    return ScriptableObject.CreateInstance<ShoutAsset>();
                }
                case AssetSubtype.CharacterLine:
                {
                    return ScriptableObject.CreateInstance<CharacterLineAsset>();
                }
                case AssetSubtype.BackgroundMusic:
                {
                    return ScriptableObject.CreateInstance<BackgroundMusicAsset>();
                }
                case AssetSubtype.Trophy:
                {
                    return ScriptableObject.CreateInstance<TrophyAsset>();
                }
                case AssetSubtype.Decoration:
                {
                    return ScriptableObject.CreateInstance<DecorationAsset>();
                }
                default:
                {
                    Debug.LogError("Invalid asset component to add.");
                    return null;
                }
            }
        }
        
        private async Task<bool> LoadMetadata(string uri)
        {
            if (String.IsNullOrEmpty(uri))
            {
                Debug.LogError("Invalid Rawrshak Asset Load. Metadata URI doesn't exist");
                return false;
            }

            string metadataJson = await Downloader.DownloadMetadata(uri);

            if (String.IsNullOrEmpty(metadataJson)) {
                Debug.LogError("Invalid Rawrshak Asset Load. Metadata doesn't exist");
                return false;
            }

            baseMetadata = PublicAssetMetadataBase.Parse(metadataJson);
            return true;
        }


        public static AssetType ParseAssetType(string type)
        {
            switch (type)
            {
                case "text":
                {
                    return AssetType.Text;
                }
                case "image":
                {
                    return AssetType.Image;
                }
                case "audio":
                {
                    return AssetType.Audio;
                }
                case "static3dobject":
                {
                    return AssetType.Static3dObject;
                }
                default:
                {
                    return AssetType.Invalid;
                }
            }
        }

        public static AssetSubtype ParseAssetSubtype(string subtype)
        {
            switch (subtype)
            {
                case "custom":
                {
                    return AssetSubtype.Custom;
                }
                case "title":
                {
                    return AssetSubtype.Title;
                }
                case "lore":
                {
                    return AssetSubtype.Lore;
                }
                case "square":
                {
                    return AssetSubtype.Square;
                }
                case "horizontal-banner":
                {
                    return AssetSubtype.HorizontalBanner;
                }
                case "vertical-banner":
                {
                    return AssetSubtype.VerticalBanner;
                }
                case "sound-effect":
                {
                    return AssetSubtype.SoundEffect;
                }
                case "shout":
                {
                    return AssetSubtype.Shout;
                }
                case "character-line":
                {
                    return AssetSubtype.CharacterLine;
                }
                case "background-music":
                {
                    return AssetSubtype.BackgroundMusic;
                }
                case "trophy":
                {
                    return AssetSubtype.Trophy;
                }
                case "decoration":
                {
                    return AssetSubtype.Decoration;
                }
                default:
                {
                    return AssetSubtype.Invalid;
                }
            }
        }
    }
}