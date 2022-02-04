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

        public int downloadTimeout = 10;

        protected AudioMetadataBase metadata;
        protected Dictionary<AudioType, AudioProperties> audioData;
        protected AudioType currentAudioType;
        protected AudioClip currentAudioClip;

        public override void Init(PublicAssetMetadataBase baseMetadata)
        {
            metadata = AudioMetadataBase.Parse(baseMetadata.jsonString);
            metadata.jsonString = baseMetadata.jsonString;

            audioData = new Dictionary<AudioType, AudioProperties>();
            foreach (var audioProperty in metadata.assetProperties)
            {
                // Filter out non-unity engine assets and unsupported content types
                AudioType audioType = ConvertAudioTypeFromString(audioProperty.contentType);
                if (audioType != AudioType.UNKNOWN)
                {
                    // Note: Overwrite duplicates. Does not throw an exception
                    audioData[audioType] = audioProperty;
                }
            }

            currentAudioType = AudioType.UNKNOWN;
        }
        
        public async Task<AudioClip> LoadAndSetAudioClipFromAudioType(AudioType type)
        {
            if (!audioData.ContainsKey(type) || type == AudioType.UNKNOWN)
            {
                Debug.LogError("[AudioAssetBase] No audio AudioType supported");
                return null;
            }

            if (currentAudioType == type)
            {
                return currentAudioClip;
            }

            AudioProperties data = null;
            foreach (var aData in audioData.Values)
            {
                if (ConvertAudioTypeFromString(aData.contentType) == type) {
                    data = aData;
                    break;
                }
            }
            
            if (data == null)
            {
                Debug.LogError("[AudioAssetBase] AudioClip metadata uri is not found");
                return null;
            }
            AudioClip audioClip = await Downloader.DownloadAudioClip(data.uri, type, downloadTimeout);

            currentAudioClip = audioClip;
            currentAudioType = type;
            return currentAudioClip;
        }

        public AudioClip GetCurrentAudioClip()
        {
            return currentAudioClip;
        }

        public AudioProperties GetCurrentAudioProperties()
        {
            return audioData[currentAudioType];
        }

        public AudioType GetCurrentAudioType()
        {
            return currentAudioType;
        }

        public List<AudioType> GetAvailableAudioTypes()
        {
            List<AudioType> types = new List<AudioType>();
            foreach(var data in audioData)
            {
                types.Add(data.Key);
            }
            return types;
        }

        public bool IsAudioTypeSupported(AudioType type)
        {
            foreach(var data in audioData)
            {
                if (type == data.Key)
                {
                    return true;
                }
            }
            return false;
        }
        
        public int GetDuration(AudioType type)
        {
            return audioData[type].duration;
        }

        private bool VerifyAudioClipProperties(AudioClip audioClip, AudioProperties properties)
        {
            // Debug.Log($"Channels: Actual: {audioClip.channels}, Property: {properties.channelCount}");
            // Debug.Log($"Length: Actual: {Mathf.RoundToInt(audioClip.length * 1000)}, Property: {properties.duration}");
            // Debug.Log($"Frequency: Actual: {audioClip.frequency}, Property: {properties.sampleRate}");

            if (Mathf.RoundToInt(audioClip.length * 1000) != properties.duration )
            {
                return false;
            }
            return true;
        }
        
        private AudioType ConvertAudioTypeFromString(string contentType)
        {
            switch(contentType)
            {
                case "audio/wav":
                {
                    return AudioType.WAV;
                }
                case "audio/mp3":
                {
                    return AudioType.MPEG;
                }
                default:
                {
                    return AudioType.UNKNOWN;
                }
            }
        }
    }
}
