using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetContentInfoScript : MonoBehaviour
{
    // Input
    public string contentContractAddress;

    // Return Value
    public Rawrshak.GetContentInfo.ReturnData data;

    // Start is called before the first frame update
    async void Start()
    {
        data = await Rawrshak.GetContentInfo.Fetch(contentContractAddress);
    }
}