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

public class TransactionCreator : MonoBehaviour
{
    public string to;
    public List<BigInteger> tokenIds;
    public List<BigInteger> amounts;
    public BigInteger nonce = 10;
    public string signer;
    public string signature;
    public MintTransactionData transaction;
    public string privateKey;
    public int chainId;
    public string contract;

    // Start is called before the first frame update
    void Start()
    {
        if (String.IsNullOrEmpty(privateKey)) {
            Debug.LogError("Private Key is Empty");
            return;
        }

        tokenIds = new List<BigInteger>();
        tokenIds.Add(7);
        tokenIds.Add(8);
        tokenIds.Add(9);
        
        amounts = new List<BigInteger>();
        amounts.Add(100);
        amounts.Add(200);
        amounts.Add(300);

        transaction = new MintTransactionData();
        transaction.to = "0x" + to;
        transaction.nonce = nonce;
        transaction.signer = "0x" + signer;
        transaction.tokenIds = tokenIds;
        transaction.amounts = amounts;

        // Todo: Wait for ChainSafe Offline signing service
        // transaction.signature = devWallet.SignEIP712MintTransaction(transaction, chainId, contract);

        signature = transaction.signature;
        Debug.Log("Args: " + transaction.GenerateArgsForCreateContractData());
        
    }

    public async void Update()
    {
        if (Input.GetKeyDown("space")) {
            Debug.Log("Space Was pressed");
            string AbiFileLocation = "Abis/Content";
            string abi = Resources.Load<TextAsset>(AbiFileLocation).text;

            // Currently, only developer wallets can mint. Need offline signing for this to work.
            string data = await EVM.CreateContractData(abi, "mintBatch", transaction.GenerateArgsForCreateContractData());
            Debug.Log("Contract Data: " + data);

            string address = WalletConnect.ActiveSession.Accounts[0];

            Debug.Log("Active Session Address: " + address);

            var transactionData = new TransactionData()
            {
                from = address,
                to = contract,
                data = data
            };
            string response = await WalletConnect.ActiveSession.EthSendTransaction(transactionData);
            Debug.Log("Response: " + response);
        }
    }
}
