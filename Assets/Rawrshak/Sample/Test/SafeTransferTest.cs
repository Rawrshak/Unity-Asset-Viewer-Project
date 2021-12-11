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

public class SafeTransferTest : MonoBehaviour
{
    public string to;
    public string contract;
    public string from;
    public string tokenId;
    public string amount;

    // Update is called once per frame
    async void Update()
    {
        if (Input.GetKeyDown("t")) {
            BigInteger balance = await Content.BalanceOf("ethereum", "optimistic-kovan", contract, to, tokenId, "https://kovan.optimism.io");

            Debug.Log("Balance: " + balance.ToString());
        }

        if (Input.GetKeyDown("space"))
        {
            // This sends assets from one wallet to another. It uses the current WalletConnect session and 
            // has the user sign the transaction on their connect wallet. Currently, only Metamask is
            // supported.
            string response = await Content.SafeTransferFrom(contract, from, to, tokenId, amount);
            Debug.Log("Response: " + response);
        }
    }
}
