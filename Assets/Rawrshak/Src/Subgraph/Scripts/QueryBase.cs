using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Numerics;
using UnityEngine;
using UnityEngine.Networking;
using GraphQlClient.Core;

namespace Rawrshak
{
    // public abstract class QueryBase : SingletonScriptableObject<QueryBase>
    public abstract class QueryBase
    {
        protected static string LoadQuery(string queryLocation) {
            TextAsset metadataTextAsset=(TextAsset)Resources.Load(queryLocation);
            return metadataTextAsset.text;
        }

        protected static async Task<string> PostAsync(string uri, string queryWithArgs) {
            // Post query
            using (UnityWebRequest request = await HttpHandler.PostAsync(uri, queryWithArgs, null))
            {
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(request.error);
                    return String.Empty;
                }
                
                Debug.Log(HttpHandler.FormatJson(request.downloadHandler.text));
                return request.downloadHandler.text;
            }
        }
    }
}
