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
    // The RawrshakWallet Script queries the subgraph for account information and an account's list of assets. It has
    // several methods of filtering the subgraph query. The RawrshakWallet doesn't keep track of these assets though. 
    // The user should manage that. 
    public class RawrshakWallet : MonoBehaviour
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

            walletAddress = responseData.data.account.address;
            mintCount = BigInteger.Parse(responseData.data.account.mintCount);
            burnCount = BigInteger.Parse(responseData.data.account.burnCount);
            uniqueAssetCount = BigInteger.Parse(responseData.data.account.uniqueAssetCount);
        }

        public async Task<List<KeyValuePair<RawrshakAsset, int>>> GetAllAssetsInWallet(int amount, string lastId)
        {
            List<KeyValuePair<RawrshakAsset, int>> assets = new List<KeyValuePair<RawrshakAsset, int>>();

            GetAssetsInWallet.ReturnData responseData = await GetAssetsInWallet.Fetch(walletAddress, amount, lastId);

            foreach (var balanceData in responseData.data.account.assetBalances)
            {
                RawrshakAsset asset = ScriptableObject.CreateInstance<RawrshakAsset>();
                asset.contractAddress = balanceData.asset.parentContract.id;
                asset.tokenId = balanceData.asset.tokenId;
                
                assets.Add(new KeyValuePair<RawrshakAsset, int>(asset, balanceData.amount));
            }

            return assets;
        }

        public async Task<List<KeyValuePair<RawrshakAsset, int>>> GetAssetsInContract(string contractAddress, int amount, string lastId)
        {
            List<KeyValuePair<RawrshakAsset, int>> assets = new List<KeyValuePair<RawrshakAsset, int>>();

            GetWalletAssetsFromContract.ReturnData responseData = await GetWalletAssetsFromContract.Fetch(walletAddress, contractAddress, amount, lastId);


            foreach (var balanceData in responseData.data.account.assetBalances)
            {
                RawrshakAsset asset = ScriptableObject.CreateInstance<RawrshakAsset>();
                asset.contractAddress = balanceData.asset.parentContract.id;
                asset.tokenId = balanceData.asset.tokenId;
                
                assets.Add(new KeyValuePair<RawrshakAsset, int>(asset, balanceData.amount));
            }

            return assets;
        }

        public async Task<List<KeyValuePair<RawrshakAsset, int>>> GetAssetsOfType(string type, int amount, string lastId)
        {
            List<KeyValuePair<RawrshakAsset, int>> assets = new List<KeyValuePair<RawrshakAsset, int>>();
            GetWalletAssetsOfType.ReturnData responseData = await GetWalletAssetsOfType.Fetch(walletAddress, type, amount, lastId);

            foreach (var balanceData in responseData.data.account.assetBalances)
            {
                RawrshakAsset asset = ScriptableObject.CreateInstance<RawrshakAsset>();
                asset.contractAddress = balanceData.asset.parentContract.id;
                asset.tokenId = balanceData.asset.tokenId;
                
                assets.Add(new KeyValuePair<RawrshakAsset, int>(asset, balanceData.amount));
            }

            return assets;
        }

        public async Task<List<KeyValuePair<RawrshakAsset, int>>> GetAssetsOfSubtype(string subtype, int amount, string lastId)
        {
            List<KeyValuePair<RawrshakAsset, int>> assets = new List<KeyValuePair<RawrshakAsset, int>>();

            GetWalletAssetsOfSubtype.ReturnData responseData = await GetWalletAssetsOfSubtype.Fetch(walletAddress, subtype, amount, lastId);

            foreach (var balanceData in responseData.data.account.assetBalances)
            {
                RawrshakAsset asset = ScriptableObject.CreateInstance<RawrshakAsset>();
                asset.contractAddress = balanceData.asset.parentContract.id;
                asset.tokenId = balanceData.asset.tokenId;
                
                assets.Add(new KeyValuePair<RawrshakAsset, int>(asset, balanceData.amount));
            }

            return assets;
        }
    }
}
