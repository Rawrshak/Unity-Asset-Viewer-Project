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

        public enum CompressionType {
            Raw,
            PCM,
            ADPCM,
            Compressed
        }

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
                ContentTypes contentType = ConvertContentTypeFromString(audioProperty.contentType);
                if (audioProperty.engine == Engine && contentType != ContentTypes.Invalid)
                {
                    // Note: Overwrite duplicates. Does not throw an exception
                    audioData[contentType] = audioProperty;
                }
            }

            currentContentType = ContentTypes.Invalid;
        }
        
        public async Task<AudioClip> LoadAndSetAudioClipFromContentType(ContentTypes type, CompressionType compressionType)
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

            AudioProperties data = null;
            foreach (var aData in audioData.Values)
            {
                if (ConvertContentTypeFromString(aData.contentType) == type && ConvertCompressionFromString(aData.compression) == compressionType) {
                    data = aData;
                    break;
                }
            }
            
            if (data == null)
            {
                Debug.LogError("AudioClip metadata uri is not found");
                return null;
            }

            AudioClip audioClip;
            if (ConvertCompressionFromString(data.compression) == CompressionType.Raw)
            {
                switch(type)
                {
                    case ContentTypes.Wav:
                    {
                        audioClip = await Downloader.DownloadAudioClip(data.uri, AudioType.WAV);
                        break;
                    }
                    case ContentTypes.MP3:
                    {
                        audioClip = await Downloader.DownloadAudioClip(data.uri, AudioType.MPEG);
                        break;
                    }
                    case ContentTypes.Ogg:
                    {
                        audioClip = await Downloader.DownloadAudioClip(data.uri, AudioType.OGGVORBIS);
                        break;
                    }
                    case ContentTypes.Aiff:
                    {
                        audioClip = await Downloader.DownloadAudioClip(data.uri, AudioType.AIFF);
                        break;
                    }
                    default:
                    {
                        Debug.LogError("Audio Clip Type is not supported.");
                        return null;
                    }
                }
            }
            else
            {
                AssetBundle assetBundle = await Downloader.DownloadAssetBundle(data.uri);
                if (assetBundle == null)
                {
                    Debug.LogError("AssetBundle not found");
                    return null;
                }

                // Debug.Log("****** Filename: " + data.name);
                audioClip = assetBundle.LoadAsset<AudioClip>(data.name);

                if (audioClip == null)
                {
                    Debug.LogError("AudioClip doesn't exist in AssetBundle");
                    assetBundle.Unload(true);
                    return null;
                }
                
                // Compare AudioClip data to audio properties metadata
                if (!VerifyAudioClipProperties(audioClip, data))
                {
                    Debug.LogError("AudioClip does not have the correct audio properties");
                    assetBundle.Unload(true);
                    return null;
                }
                assetBundle.Unload(false);
            }

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

        public bool IsContentTypeSupported(ContentTypes type)
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

        public bool IsCompressionTypeSupported(CompressionType type)
        {
            foreach(var data in audioData)
            {
                if (type == ConvertCompressionFromString(data.Value.compression))
                {
                    return true;
                }
            }
            return false;
        }

        public List<CompressionType> GetAvailableCompressionTypes()
        {
            List<CompressionType> types = new List<CompressionType>();
            foreach(var data in audioData)
            {
                types.Add(ConvertCompressionFromString(data.Value.compression));
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
        
        private ContentTypes ConvertContentTypeFromString(string contentType)
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
        
        private CompressionType ConvertCompressionFromString(string compressionType)
        {
            switch(compressionType)
            {
                case "pcm":
                {
                    return CompressionType.PCM;
                }
                case "adpcm":
                {
                    return CompressionType.ADPCM;
                }
                case "compressed":
                {
                    return CompressionType.Compressed;
                }
                default:
                {
                    return CompressionType.Raw;
                }
            }
        }
    }
}
