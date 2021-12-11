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
    public class GetAssetInfo : QueryBase
    {
        public static async Task<ReturnData> Fetch(string contractAddress, string tokenId) {
            // Load query if this is the first Fetch
            string query = LoadQuery(Constants.GET_ASSET_INFO_QUERY_STRING_LOCATION);
            
            // Load the query parameters
            string queryWithArgs = String.Format(query, contractAddress.ToLower(), tokenId);

            // Post query
            string returnData = await PostAsync(Subgraph.Instance.contentsSubgraphUri, queryWithArgs);

            // Parse data
            return JsonUtility.FromJson<ReturnData>(returnData);
        }

        [Serializable]
        public class ReturnData
        {
            public DataObject data;
        }

        [Serializable]
        public class DataObject 
        {
            public AssetData asset;
        }

        [Serializable]
        public class AssetData 
        {
            public string id;
            public string tokenId;
            public string name;
            public string type;
            public string subtype;
            public string imageUri;
            public string currentSupply;
            public string maxSupply;
            public string latestPublicUriVersion;
            public string latestHiddenUriVersion;
            public string latestPublicUri;
            public TagData[] tags;
        }

        [Serializable]
        public class TagData 
        {
            public string id;
        }
    }
}
