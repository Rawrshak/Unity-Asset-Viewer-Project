using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetAssetIdsWithTagScript : MonoBehaviour
{
    // Input
    public string tag;
    public int amountToQuery;
    public string lastItemId;

    // Return Value
    public Rawrshak.GetAssetIdsWithTag.ReturnData data;

    // Start is called before the first frame update
    async void Start()
    {
        data = await Rawrshak.GetAssetIdsWithTag.Fetch(tag, amountToQuery, lastItemId);
    }
}
