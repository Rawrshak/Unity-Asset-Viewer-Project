using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetAssetInfoSample : MonoBehaviour
{
    public Rawrshak.GetAssetInfo.ReturnData data;

    // Start is called before the first frame update
    async void Start()
    {
        // Test Query
        data = await Rawrshak.GetAssetInfo.Fetch("0x393d8e12aa7f22f8999bf9ddac6842db2bb6f096", "2");
    }
}
