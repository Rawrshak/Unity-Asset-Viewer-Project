using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetAssetsInContentScript : MonoBehaviour
{
    // Input
    public string contentContractAddress;
    public int amountToQuery;
    public string lastItemId;

    // Return Value
    public Rawrshak.GetAssetsInContentContract.ReturnData data;

    // Start is called before the first frame update
    async void Start()
    {
        // await Fetch("0x393d8e12aa7f22f8999bf9ddac6842db2bb6f096", 6, "0x393d8e12aa7f22f8999bf9ddac6842db2bb6f096-5");
        data = await Rawrshak.GetAssetsInContentContract.Fetch(contentContractAddress, amountToQuery, lastItemId);
    }
}
