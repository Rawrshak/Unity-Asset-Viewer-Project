using System.Collections;
using System.Numerics;
using System.Collections.Generic;
using UnityEngine;
using Rawrshak;
using System.Threading.Tasks;

public class WalletQueries : MonoBehaviour
{
    public Rawrshak.Network network;

    // Optimism Kovan Testnet - Sample ContentManager Contract Address
    public string contractAddress;
    public string playerAddress;
    public string operatorAddress;

    // Start is called before the first frame update
    void Start()
    {
        network = Rawrshak.Network.Instance;
        
        Debug.Log("Player Wallet: " + playerAddress);
        Debug.Log("Contract Address: " + contractAddress);
        Debug.Log("Operator Address: " + operatorAddress);

        Debug.Log("Network chain: " + network.chain);
        Debug.Log("Network network: " + network.network);
        Debug.Log("Network endpoint: " + network.httpEndpoint);
    }

    public async Task Query()
    {
        // Check Balance
        BigInteger balance = await ContentManager.BalanceOf(network.chain, network.network, contractAddress, playerAddress, "3", network.httpEndpoint);
        Debug.Log("Player Address: " + playerAddress + ", Token ID: 3, Balance: " + balance.ToString());

        // Check BalanceOfBatch
        string[] accounts = new string[] {playerAddress, playerAddress};
        string[] tokenIds = new string[] {"3", "4"};
        List<BigInteger> balances = await ContentManager.BalanceOfBatch(network.chain, network.network, contractAddress, accounts, tokenIds, network.httpEndpoint);

        int counter = 0;
        balances.ForEach(delegate(BigInteger balance) {
            Debug.Log("Player Address: " + accounts[counter] + ", Token ID: " + tokenIds[counter] + ",  Balance: " + balance.ToString());
            counter++;
        });

        /****** ContentManager Contract Calls ******/
        // IsApprovedForAll()
        bool isApproved = await ContentManager.isApprovedForAll(network.chain, network.network, contractAddress, playerAddress, operatorAddress, network.httpEndpoint);
        Debug.Log("isApproved: " + isApproved);
        
        // ContractUri()
        string contractUri = await ContentManager.ContractUri(network.chain, network.network, contractAddress, network.httpEndpoint);
        Debug.Log("contractUri: " + contractUri);
        
        // ContractRoyalty()
        ContentManager.RoyaltyResponse contractRoyalty = await ContentManager.ContractRoyalty(network.chain, network.network, contractAddress, network.httpEndpoint);
        float rate = float.Parse(contractRoyalty.rate) / 1000000.0f;
        Debug.Log("contractRoyalty: [Receiver: " + contractRoyalty.receiver + ", Rate: " + rate.ToString("n2") + "%]");
        
        // TokenUri()
        string tokenUri = await ContentManager.TokenUri(network.chain, network.network, contractAddress, "1", network.httpEndpoint);
        Debug.Log("current tokenUri: " + tokenUri);
        
        // TokenUriWithVersion()
        string tokenUriWithVersion = await ContentManager.TokenUriWithVersion(network.chain, network.network, contractAddress, "1", "0", network.httpEndpoint);
        Debug.Log("tokenUriWithVersion (version 0): " + tokenUriWithVersion);
        
        // Total Supply
        BigInteger totalSupply = await ContentManager.TotalSupply(network.chain, network.network, contractAddress, "1", network.httpEndpoint);
        Debug.Log("Current Token supply: " + totalSupply.ToString());
        
        // Max Supply
        BigInteger maxSupply = await ContentManager.MaxSupply(network.chain, network.network, contractAddress, "1", network.httpEndpoint);
        Debug.Log("Token maxSupply: " + maxSupply.ToString());
        
        // Supports Interface
        bool supportsInterface = await ContentManager.SupportsInterface(network.chain, network.network, contractAddress, "0xd9b67a26", network.httpEndpoint);
        Debug.Log("Supports Interface: " + supportsInterface.ToString());
        
        // Todo: Create Mint, Burn, and Safe Batch Transfer transactions with WalletConnect
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}