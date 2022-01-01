using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using WalletConnectSharp.Core.Models.Ethereum;
using WalletConnectSharp.Unity;

namespace Rawrshak
{
    // The Wallet Script queries the subgraph for account information and an account's list of assets. It has
    // several methods of filtering the subgraph query. The Wallet doesn't keep track of these assets though. 
    // The user should manage that. 
    public class Wallet : MonoBehaviour
    {
        public string walletAddress;
        public BigInteger mintCount = 0;
        public BigInteger burnCount = 0;
        public BigInteger uniqueAssetCount = 0;

        void Start()
        {
            
        }

        void Update()
        {
            
        }

        public async Task LoadAccountInfo(string address)
        {
            GetAccountInfo.ReturnData responseData = await GetAccountInfo.Fetch(address);

            if (responseData.data.account.address == null)
            {
                Debug.Log("Error: No Assets");
                return;
            }

            walletAddress = responseData.data.account.address;
            mintCount = BigInteger.Parse(responseData.data.account.mintCount);
            burnCount = BigInteger.Parse(responseData.data.account.burnCount);
            uniqueAssetCount = BigInteger.Parse(responseData.data.account.uniqueAssetCount);
        }

        public async Task<List<KeyValuePair<Asset, int>>> GetAllAssetsInWallet(int amount, string lastId)
        {
            List<KeyValuePair<Asset, int>> assets = new List<KeyValuePair<Asset, int>>();

            GetAssetsInWallet.ReturnData responseData = await GetAssetsInWallet.Fetch(walletAddress, amount, lastId);

            if (responseData.data.account.assetBalances == null)
            {
                Debug.Log("Error: No Assets");
                return assets;
            }

            foreach (var balanceData in responseData.data.account.assetBalances)
            {
                Asset asset = ScriptableObject.CreateInstance<Asset>();
                asset.contractAddress = balanceData.asset.parentContract.id;
                asset.tokenId = balanceData.asset.tokenId;
                
                assets.Add(new KeyValuePair<Asset, int>(asset, balanceData.amount));
            }

            return assets;
        }

        public async Task<List<KeyValuePair<Asset, int>>> GetAssetsInContract(string contractAddress, int amount, string lastId)
        {
            List<KeyValuePair<Asset, int>> assets = new List<KeyValuePair<Asset, int>>();

            GetWalletAssetsInContent.ReturnData responseData = await GetWalletAssetsInContent.Fetch(walletAddress, contractAddress, amount, lastId);

            if (responseData.data.account.assetBalances == null)
            {
                Debug.Log("Error: No Assets");
                return assets;
            }

            foreach (var balanceData in responseData.data.account.assetBalances)
            {
                Asset asset = ScriptableObject.CreateInstance<Asset>();
                asset.contractAddress = balanceData.asset.parentContract.id;
                asset.tokenId = balanceData.asset.tokenId;
                
                assets.Add(new KeyValuePair<Asset, int>(asset, balanceData.amount));
            }

            return assets;
        }

        public async Task<List<KeyValuePair<Asset, int>>> GetAssetsOfType(string type, int amount, string lastId)
        {
            List<KeyValuePair<Asset, int>> assets = new List<KeyValuePair<Asset, int>>();
            GetWalletAssetsOfType.ReturnData responseData = await GetWalletAssetsOfType.Fetch(walletAddress, type, amount, lastId);

            if (responseData.data.account.assetBalances == null)
            {
                Debug.Log("Error: No Assets");
                return assets;
            }

            foreach (var balanceData in responseData.data.account.assetBalances)
            {
                Asset asset = ScriptableObject.CreateInstance<Asset>();
                asset.contractAddress = balanceData.asset.parentContract.id;
                asset.tokenId = balanceData.asset.tokenId;
                
                assets.Add(new KeyValuePair<Asset, int>(asset, balanceData.amount));
            }

            return assets;
        }

        public async Task<List<KeyValuePair<Asset, int>>> GetAssetsOfSubtype(string subtype, int amount, string lastId)
        {
            List<KeyValuePair<Asset, int>> assets = new List<KeyValuePair<Asset, int>>();

            GetWalletAssetsOfSubtype.ReturnData responseData = await GetWalletAssetsOfSubtype.Fetch(walletAddress, subtype, amount, lastId);

            if (responseData.data.account.assetBalances == null)
            {
                Debug.Log("Error: No Assets");
                return assets;
            }

            foreach (var balanceData in responseData.data.account.assetBalances)
            {
                Asset asset = ScriptableObject.CreateInstance<Asset>();
                asset.contractAddress = balanceData.asset.parentContract.id;
                asset.tokenId = balanceData.asset.tokenId;
                
                assets.Add(new KeyValuePair<Asset, int>(asset, balanceData.amount));
            }

            return assets;
        }
    }
}
