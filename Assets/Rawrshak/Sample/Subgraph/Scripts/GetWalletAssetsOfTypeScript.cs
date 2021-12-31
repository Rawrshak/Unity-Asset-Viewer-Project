using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetWalletAssetsOfTypeScript : MonoBehaviour
{
    // Input
    public string walletAddress;
    public string type;
    public int amountToQuery;
    public string lastItemId;

    // Return Value
    public Rawrshak.GetWalletAssetsOfType.ReturnData data;

    // Start is called before the first frame update
    async void Start()
    {
        data = await Rawrshak.GetWalletAssetsOfType.Fetch(walletAddress, type, amountToQuery, lastItemId);
    }
}
