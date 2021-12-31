using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetAssetInfoScript : MonoBehaviour
{
    // Input
    public string contentContractAddress;
    public int tokenId;

    // Return Value
    public Rawrshak.GetAssetInfo.ReturnData data;

    // Start is called before the first frame update
    async void Start()
    {
        // Test Query
        data = await Rawrshak.GetAssetInfo.Fetch(contentContractAddress, tokenId.ToString());
    }
}
