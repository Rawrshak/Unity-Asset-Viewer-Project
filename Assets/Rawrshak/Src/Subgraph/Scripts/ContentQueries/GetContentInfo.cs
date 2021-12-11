using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using GraphQlClient.Core;

namespace Rawrshak
{
    public class GetContentInfo : QueryBase
    {
        public static async Task<ReturnData> Fetch(string address) {
            // Load query if this is the first Fetch
            string query = LoadQuery(Constants.GET_CONTENT_INFO_QUERY_STRING_LOCATION);

            // Load the query parameters
            string queryWithArgs = String.Format(query, address.ToLower());

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
            public ContentData content;
        }
        
        [Serializable]
        public class ContentData 
        {
            public string id;
            public string name;
            public string game;
            public string creator;
            public string creatorAddress;
            public OwnerData owner;
            public string contractAddress;
            public string contractUri;
            public int royaltyRate;
            public TagData[] tags;
        }

        [Serializable]
        public class OwnerData 
        {
            public string id;
        }

        [Serializable]
        public class TagData 
        {
            public string id;
        }
    }
}
