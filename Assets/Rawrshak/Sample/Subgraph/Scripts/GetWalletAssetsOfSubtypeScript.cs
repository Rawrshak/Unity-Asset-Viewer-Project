using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetWalletAssetsOfSubtypeScript : MonoBehaviour
{
    // Input
    public string walletAddress;
    public string subtype;
    public int amountToQuery;
    public string lastItemId;

    // Return Value
    public Rawrshak.GetWalletAssetsOfSubtype.ReturnData data;

    // Start is called before the first frame update
    async void Start()
    {
        data = await Rawrshak.GetWalletAssetsOfSubtype.Fetch(walletAddress, subtype, amountToQuery, lastItemId);
    }
}
