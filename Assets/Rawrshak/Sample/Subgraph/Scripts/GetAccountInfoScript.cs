using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetAccountInfoScript : MonoBehaviour
{
    // Input
    public string accountAddress;

    // Return Value
    public Rawrshak.GetAccountInfo.ReturnData data;
    
    // Start is called before the first frame update
    async void Start()
    {
        data = await Rawrshak.GetAccountInfo.Fetch(accountAddress);
    }
}
