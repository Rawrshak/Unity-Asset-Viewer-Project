using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using GraphQlClient.Core;

namespace Rawrshak
{
    public class GetWalletAssetsFromContract : QueryBase
    {
        public static async Task<ReturnData> Fetch(string walletAddress, string contractAddress, int first, string lastId) {
            // Load query if this is the first Fetch
            string query = LoadQuery(Constants.GET_WALLET_ASSETS_FROM_CONTRACT_QUERY_STRING_LOCATION);
            
            // Load the query parameters
            string queryWithArgs = String.Format(query, walletAddress.ToLower(), contractAddress.ToLower(), first, lastId);

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
            public AccountData account;
        }
    
        [Serializable]
        public class AccountData
        {
            public AssetBalanceData[] assetBalances;
        }
        
        [Serializable]
        public class AssetBalanceData 
        {
            public string id;
            public int amount;
            public AssetData asset;
        }

        [Serializable]
        public class AssetData 
        {
            public string id;
            public string tokenId;
            public ContentData parentContract;
        }

        [Serializable]
        public class ContentData 
        {
            public string id;
        }
    }
}
