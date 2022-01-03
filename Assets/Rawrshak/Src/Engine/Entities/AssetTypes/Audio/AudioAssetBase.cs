using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Rawrshak
{
    public abstract class AudioAssetBase : AssetBase
    {
        public enum MaxDurationMs {
            SoundEffect = 1000,
            Shout = 2000,
            CharacterLine = 10000,
            BackgroundMusic = 300000
        };

        public enum ContentTypes {
            Invalid,
            Wav,
            MP3,
            Ogg,
            Aiff
        }

        // todo: add Compression

        private static string Engine = "unity";

        protected AudioMetadataBase metadata;
        protected Dictionary<ContentTypes, AudioProperties> audioData;
        protected ContentTypes currentContentType;
        protected AudioClip currentAudioClip;

        public override void Init(PublicAssetMetadataBase baseMetadata)
        {
            metadata = AudioMetadataBase.Parse(baseMetadata.jsonString);
            metadata.jsonString = baseMetadata.jsonString;

            audioData = new Dictionary<ContentTypes, AudioProperties>();
            foreach (var audioProperty in metadata.assetProperties)
            {
                // Filter out non-unity engine assets and unsupported content types
                ContentTypes contentType = ConvertFromString(audioProperty.contentType);
                if (audioProperty.engine == Engine && contentType != ContentTypes.Invalid)
                {
                    // Note: Overwrite duplicates. Does not throw an exception
                    audioData[contentType] = audioProperty;
                }
            }

            currentContentType = ContentTypes.Invalid;
        }
        
        public async Task<AudioClip> LoadAndSetAudioClipFromContentType(ContentTypes type)
        {
            if (!audioData.ContainsKey(type) || type == ContentTypes.Invalid)
            {
                Debug.LogError("No audio ContentType supported");
                return null;
            }

            if (currentContentType == type)
            {
                return currentAudioClip;
            }

            AudioProperties data = audioData[type];
            
            if (String.IsNullOrEmpty(data.uri))
            {
                Debug.LogError("AudioClip metadata uri is not found");
                return null;
            }

            AssetBundle assetBundle = await Downloader.DownloadAssetBundle(data.uri);
            if (assetBundle == null)
            {
                Debug.LogError("AssetBundle not found");
                return null;
            }

            // Debug.Log("****** Filename: " + data.name);
            AudioClip audioClip = assetBundle.LoadAsset<AudioClip>(data.name);

            if (audioClip == null)
            {
                Debug.LogError("AudioClip doesn't exist in AssetBundle");
                return null;
            }
            
            // Compare AudioClip data to audio properties metadata
            if (!VerifyAudioClipProperties(audioClip, data))
            {
                Debug.LogError("AudioClip does not have the correct audio properties");
                return null;
            }

            assetBundle.Unload(false);
            currentAudioClip = audioClip;
            currentContentType = type;
            return currentAudioClip;
        }

        public AudioClip GetCurrentAudioClip()
        {
            return currentAudioClip;
        }

        public AudioProperties GetCurrentAudioProperties()
        {
            return audioData[currentContentType];
        }

        public ContentTypes GetCurrentContentType()
        {
            return currentContentType;
        }

        public List<ContentTypes> GetAvailableContentTypes()
        {
            List<ContentTypes> types = new List<ContentTypes>();
            foreach(var data in audioData)
            {
                types.Add(data.Key);
            }
            return types;
        }
        
        public int GetDuration(ContentTypes type)
        {
            return audioData[type].duration;
        }

        public int GetChannelCount(ContentTypes type)
        {
            return audioData[type].channelCount;
        }

        public int GetSampleRate(ContentTypes type)
        {
            return audioData[type].sampleRate;
        }

        private bool VerifyAudioClipProperties(AudioClip audioClip, AudioProperties properties)
        {
            // Debug.Log($"Channels: Actual: {audioClip.channels}, Property: {properties.channelCount}");
            // Debug.Log($"Length: Actual: {Mathf.RoundToInt(audioClip.length * 1000)}, Property: {properties.duration}");
            // Debug.Log($"Frequency: Actual: {audioClip.frequency}, Property: {properties.sampleRate}");

            if (audioClip.channels != properties.channelCount ||
                Mathf.RoundToInt(audioClip.length * 1000) != properties.duration || 
                audioClip.frequency != properties.sampleRate)
            {
                return false;
            }
            return true;
        }
        
        private ContentTypes ConvertFromString(string contentType)
        {
            switch(contentType)
            {
                case "audio/wav":
                {
                    return ContentTypes.Wav;
                }
                case "audio/mp3":
                {
                    return ContentTypes.MP3;
                }
                case "audio/ogg":
                {
                    return ContentTypes.Ogg;
                }
                case "audio/x-aiff":
                {
                    return ContentTypes.Aiff;
                }
                default:
                {
                    return ContentTypes.Invalid;
                }
            }
        }
    }
}
