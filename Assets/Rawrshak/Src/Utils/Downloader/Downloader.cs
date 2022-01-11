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

        public static async Task<string> DownloadMetadata(string uri)
        {
            using (UnityWebRequest uwr = UnityWebRequest.Get(uri))
            {
                // Request and wait for the text json file to be downloaded
                await uwr.SendWebRequest();

                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    lastError = uwr.error;
                    Debug.LogError(uwr.error);
                    return String.Empty;
                }

                Debug.Log(uwr.downloadHandler.text);
                return uwr.downloadHandler.text;
            }
        }
        
        public static async Task<Texture2D> DownloadTexture(string uri)
        {
            using (UnityWebRequest uwr = UnityWebRequest.Get(uri))
            {
                uwr.downloadHandler = new DownloadHandlerTexture();

                // Request and wait for the text json file to be downloaded
                await uwr.SendWebRequest();

                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    lastError = uwr.error;
                    Debug.LogError(uwr.error);
                    return null;
                }

                return DownloadHandlerTexture.GetContent(uwr);
            }
        }

        public static async Task<AssetBundle> DownloadAssetBundle(string uri)
        {
            using (UnityWebRequest uwr = UnityWebRequest.Get(uri))
            {
                uwr.downloadHandler = new DownloadHandlerAssetBundle(uwr.url, 0);

                // Request and wait for the text json file to be downloaded
                await uwr.SendWebRequest();

                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    lastError = uwr.error;
                    Debug.LogError(uwr.error);
                    return null;
                }

                return DownloadHandlerAssetBundle.GetContent(uwr);
            }
        }
    }
}