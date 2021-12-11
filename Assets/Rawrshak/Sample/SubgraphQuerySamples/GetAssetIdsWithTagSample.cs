using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetAssetIdsWithTagSample : MonoBehaviour
{
    public Rawrshak.GetAssetIdsWithTag.ReturnData data;

    // Start is called before the first frame update
    async void Start()
    {
        data = await Rawrshak.GetAssetIdsWithTag.Fetch("Rawrshak", 5, "0x393d8e12aa7f22f8999bf9ddac6842db2bb6f096-2");
    }
}
