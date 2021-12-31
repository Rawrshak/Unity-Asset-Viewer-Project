using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using Newtonsoft.Json;
using UnityEngine;
using Rawrshak;
using WalletConnectSharp.Core.Models.Ethereum;
using WalletConnectSharp.Unity;

public class MintAssetSample : MonoBehaviour
{    
    // Optimistic Kovan Testnet Chain ID = 69
    public BigInteger chainId = 69;

    public string to;
    public string contract;
    public MintTransactionData transaction;
    public string privateKey;
    
    // Start is called before the first frame update
    async void Start()
    {
        if (String.IsNullOrEmpty(privateKey)) {
            Debug.LogError("Private Key is Empty");
            return;
        }

        Debug.Log("Mint Transaction: Creating the Mint Transaction...");
        List<BigInteger> tokenIds = new List<BigInteger>();
        tokenIds.Add(1);

        List<BigInteger> amounts = new List<BigInteger>();
        amounts.Add(10);

        // Currently, only developer wallets can mint. Need offline signing for this to work.
        transaction = new MintTransactionData();
        transaction.to = to;
        transaction.nonce = BigInteger.Parse(await ContentManager.UserMintNonce("ethereum", "optimistic-kovan", contract, to, "https://kovan.optimism.io")) + 1;
        // transaction.signer = devWallet.GetPublicAddress();
        transaction.tokenIds = tokenIds;
        transaction.amounts = amounts;

        // Todo: Wait for ChainSafe Offline signing service
        // transaction.signature = devWallet.SignEIP712MintTransaction(transaction, chainId, contract);

    }

    public async void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            Debug.Log("Mint Transaction: Creating the Contract Data...");
            string AbiFileLocation = "Abis/Content";
            string abi = Resources.Load<TextAsset>(AbiFileLocation).text;

            string data = await EVM.CreateContractData(abi, "mintBatch", transaction.GenerateArgsForCreateContractData());
            
            string address = WalletConnect.ActiveSession.Accounts[0];
            var transactionData = new TransactionData()
            {
                from = address,
                to = contract,
                data = data,
                chainId = 69
            };

            string response = await WalletConnect.ActiveSession.EthSendTransaction(transactionData);
            Debug.Log("Response: " + response);
        }
    }
}
