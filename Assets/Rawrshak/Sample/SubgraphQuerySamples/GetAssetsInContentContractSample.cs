using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetAssetsInContentContractSample : MonoBehaviour
{
    public Rawrshak.GetAssetsInContentContract.ReturnData data;

    // Start is called before the first frame update
    async void Start()
    {
        data = await Rawrshak.GetAssetsInContentContract.Fetch("0x393d8e12aa7f22f8999bf9ddac6842db2bb6f096", 6, "");
        //     // await Fetch("0x393d8e12aa7f22f8999bf9ddac6842db2bb6f096", 6, "0x393d8e12aa7f22f8999bf9ddac6842db2bb6f096-5");
    }
}
