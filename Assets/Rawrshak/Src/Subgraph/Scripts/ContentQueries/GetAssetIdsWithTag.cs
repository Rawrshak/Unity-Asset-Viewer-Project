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
    public class GetAssetIdsWithTag : QueryBase
    {
        public static async Task<ReturnData> Fetch(string tag, int first, string lastId) {
            // Load query if this is the first Fetch
            string query = LoadQuery(Constants.GET_ASSET_IDS_WITH_TAG_QUERY_STRING_LOCATION);

            // Load the query parameters
            string queryWithArgs = String.Format(query, first, lastId, tag);

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
            public TagData[] tags;
        }

        [Serializable]
        public class TagData 
        {
            public string id;
            public AssetData[] assets;
        }

        [Serializable]
        public class AssetData 
        {
            public string id;
            public string tokenId;
            public ContentIdData parentContract;
        }

        [Serializable]
        public class ContentIdData 
        {
            public string id;
        }
    }
}
