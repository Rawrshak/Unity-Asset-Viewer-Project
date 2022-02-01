using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

namespace Rawrshak
{
    public class Downloader
    {
        public static string lastError;

        public static async Task<string> DownloadMetadata(string uri, int timeout = 10)
        {
            using (UnityWebRequest uwr = UnityWebRequest.Get(uri))
            {
                // Set a timeout if the we can't find the file
                uwr.timeout = timeout;

                // Request and wait for the text json file to be downloaded
                await uwr.SendWebRequest();

                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    lastError = uwr.error;
                    Debug.LogError("[Downloader] Uri: " + uri + ", Error Message: " + lastError);
                    return String.Empty;
                }

                Debug.Log(uwr.downloadHandler.text);
                return uwr.downloadHandler.text;
            }
        }
        
        public static async Task<Texture2D> DownloadTexture(string uri, int timeout = 10)
        {
            using (UnityWebRequest uwr = UnityWebRequest.Get(uri))
            {
                // Set a timeout if the we can't find the file
                uwr.timeout = timeout;
                uwr.downloadHandler = new DownloadHandlerTexture();

                // Request and wait for the text json file to be downloaded
                await uwr.SendWebRequest();

                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    lastError = uwr.error;
                    Debug.LogError("[Downloader] Uri: " + uri + ", Error Message: " + lastError);
                    return null;
                }

                return DownloadHandlerTexture.GetContent(uwr);
            }
        }
        
        public static async Task<AudioClip> DownloadAudioClip(string uri, AudioType type, int timeout = 10)
        {
            using (UnityWebRequest uwr = UnityWebRequest.Get(uri))
            {
                uwr.timeout = timeout;
                uwr.downloadHandler = new DownloadHandlerAudioClip(uri, type);

                // Request and wait for the text json file to be downloaded
                await uwr.SendWebRequest();

                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    lastError = uwr.error;
                    Debug.LogError("[Downloader] Uri: " + uri + ", Error Message: " + lastError);
                    return null;
                }

                return DownloadHandlerAudioClip.GetContent(uwr);
            }
        }

        public static async Task<AssetBundle> DownloadAssetBundle(string uri, int timeout = 10)
        {
            using (UnityWebRequest uwr = UnityWebRequest.Get(uri))
            {
                // Set a timeout if the we can't find the file
                uwr.timeout = timeout;
                uwr.downloadHandler = new DownloadHandlerAssetBundle(uwr.url, 0);

                // Request and wait for the text json file to be downloaded
                await uwr.SendWebRequest();

                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    lastError = uwr.error;
                    Debug.LogError("[Downloader] Uri: " + uri + ", Error Message: " + lastError);
                    return null;
                }

                return DownloadHandlerAssetBundle.GetContent(uwr);
            }
        }
    }
}