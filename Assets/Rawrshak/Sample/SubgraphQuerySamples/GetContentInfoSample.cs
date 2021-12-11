using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetContentInfoSample : MonoBehaviour
{
    public Rawrshak.GetContentInfo.ReturnData data;

    // Start is called before the first frame update
    async void Start()
    {
        data = await Rawrshak.GetContentInfo.Fetch("0x393d8e12aa7f22f8999bf9ddac6842db2bb6f096");
    }
}