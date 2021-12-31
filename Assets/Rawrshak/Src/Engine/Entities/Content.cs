using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Rawrshak
{
    // Todo: Update Debug.LogError() calls to "throw new Exception()";
    [CreateAssetMenu(fileName="Content", menuName="Rawrshak/Create Content Contract Object")]
    public class Content : ScriptableObject
    {
        public string contractAddress;
        private int statusCheckSleepDuration = 5000;
        public bool usingArweave = false;
        public ContentMetadataBase metadata;

        // Private Variables
        private Network network;
        private bool isValid;
        private Dictionary<BigInteger, BigInteger> assetsToMint;
        private ContentState state;

        public void OnEnable()
        {
            isValid = false;
            assetsToMint = new Dictionary<BigInteger, BigInteger>();
            network = Network.Instance;
            state = ContentState.NoAssetsToMint;
        }
        
        public void OnDisable()
        {
            // Do not save assets to mint
            assetsToMint.Clear();
        }

        public async Task VerifyContracts()
        {
            if (network == null)
            {
                Debug.LogError("Network is not set.");
                isValid = false;
            }
            isValid = await ContentManager.SupportsInterface(network.chain, network.network, contractAddress, Constants.RAWRSHAK_ICONTENT_INTERFACE_ID, network.httpEndpoint);
        }

        public async Task LoadContractMetadata()
        {
            // Checks if contract is valid and network is set
            if (!isValid)
            {
                Debug.LogError("Contract or Network is not valid.");
                return;
            }

            // Get Metadata URI from ethereum
            string uri = await ContentManager.ContractUri(network.chain, network.network, contractAddress, network.httpEndpoint);

            // Download Metadata from Arweave / IPFS
            if (!usingArweave)
            {
                // This means the uri is a IPFS hash id
                uri = String.Format(Constants.IPFS_QUERY_FORMAT, uri);
            }
            string metadataJson = await Downloader.DownloadMetadata(uri);

            if (String.IsNullOrEmpty(metadataJson)) {
                Debug.LogError("Invalid Contract Metadata");
                return;
            }
            // Parse Content Metadata
            metadata = ContentMetadataBase.Parse(metadataJson);
        }

        // Returns the transaction id
        public async Task<string> MintAssets(string receiver)
        {
            if (state == ContentState.Minting) {
                Debug.LogError("Contract is currently minting.");
                return String.Empty;
            }

            if (!isValid)
            {
                Debug.LogError("Contract or Network is not valid.");
                return String.Empty;
            }

            if (assetsToMint.Count == 0)
            {
                Debug.LogWarning("No Assets to mint");
                return String.Empty;
            }

            // Build the Transaction object
            MintTransactionData transaction = new MintTransactionData();
            transaction.to = receiver;
            transaction.nonce = BigInteger.Parse(await ContentManager.UserMintNonce(
                network.chain,
                network.network,
                contractAddress,
                receiver,
                network.httpEndpoint)) + 1;
            
            // transaction.signer = devWallet.GetPublicAddress();
            transaction.tokenIds = new List<BigInteger>();
            transaction.amounts = new List<BigInteger>();
            foreach (var asset in assetsToMint)
            {
                transaction.tokenIds.Add(asset.Key);
                transaction.amounts.Add(asset.Value);
            }

            // Todo: Eip712Signer does not support arrays yet. Update this when it is supported. Mint from in-game doesn't
            // Developer wallet sign the mint transaction. Developer wallet must have correct access rights.
            // transaction.signature = devWallet.SignEIP712MintTransaction(transaction, network.chainId, contractAddress);

            // Send Mint transaction
            string response = String.Empty;
            try
            {
                response= await ContentManager.MintBatch(network.chain, network.network, contractAddress, transaction, network.httpEndpoint);

                state = ContentState.Minting;
            }
            catch (Exception e)
            {
                // Reset state to ready to mint to try again
                state = ContentState.ReadyToMint;
                Debug.LogException(e, this);
            }

            return response;
        }

        public async Task<string> WaitForTransaction(string transactionId) 
        {
            if (state != ContentState.Minting) {
                Debug.LogError("Contract is not minting.");
                return "fail";
            }

            if (!isValid)
            {
                Debug.LogError("Network is not valid.");
                return "fail";
            }

            string transactionStatus = "pending";
            while (transactionStatus == "pending")
            {
                // Poll every duration to check if the transaction has occurred. 
                // Todo: If the transaction id is invalid, does it return success or fail?
                transactionStatus = await EVM.TxStatus(network.chain, network.network, transactionId, network.httpEndpoint);
                Thread.Sleep(statusCheckSleepDuration);
            }

            assetsToMint.Clear();
            state = ContentState.NoAssetsToMint;

            return transactionStatus;
        }

        public bool ClearMintList() {
            if (state == ContentState.Minting) {
                Debug.LogError("Contract is currently minting.");
                return false;
            }

            assetsToMint.Clear();
            state = ContentState.NoAssetsToMint;
            return true;
        }

        public bool AddToMintList(Asset asset, BigInteger amount)
        {
            if (state == ContentState.Minting) {
                Debug.LogError("Contract is currently minting.");
                return false;
            }

            if (contractAddress != asset.contractAddress)
            {
                Debug.LogError("Asset does not belong to this contract.");
                return false;
            }

            BigInteger tokenId = BigInteger.Parse(asset.tokenId);
            if (assetsToMint.ContainsKey(tokenId))
            {
                assetsToMint[tokenId] += amount;
                return true;
            }
            assetsToMint.Add(tokenId, amount);
            
            if (state == ContentState.NoAssetsToMint) {
                state = ContentState.ReadyToMint;
            }
            return true;
        }

        public bool RemoveFromMintList(Asset asset, BigInteger amount)
        {
            if (state == ContentState.Minting) {
                Debug.LogError("Contract is currently minting.");
                return false;
            }
            
            if (contractAddress != asset.contractAddress)
            {
                Debug.LogError("Asset does not belong to this contract.");
                return false;
            }

            BigInteger tokenId = BigInteger.Parse(asset.tokenId);
            if (assetsToMint.ContainsKey(tokenId) && assetsToMint[tokenId] <= amount)
            {
                assetsToMint.Remove(tokenId);
                return true;
            }
            assetsToMint[tokenId] -= amount;

            if (assetsToMint.Count  == 0) 
            {
                state = ContentState.NoAssetsToMint;
            }

            return true;
        }

        public bool IsValid()
        {
            return isValid;
        }
    }
}