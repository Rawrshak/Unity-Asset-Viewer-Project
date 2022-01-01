using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetAssetIdsWithTagScript : MonoBehaviour
{
    // Input
    public string assetTag;
    public int amountToQuery;
    public string lastItemId;

    // Return Value
    public Rawrshak.GetAssetIdsWithTag.ReturnData data;

    // Start is called before the first frame update
    async void Start()
    {
        data = await Rawrshak.GetAssetIdsWithTag.Fetch(assetTag, amountToQuery, lastItemId);
    }
}
