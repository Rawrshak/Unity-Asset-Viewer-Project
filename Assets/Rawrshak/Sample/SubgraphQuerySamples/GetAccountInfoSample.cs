using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetAccountInfoSample : MonoBehaviour
{
    public Rawrshak.GetAccountInfo.ReturnData data;
    
    // Start is called before the first frame update
    async void Start()
    {
        data = await Rawrshak.GetAccountInfo.Fetch("0xB796BCe3db9A9DFb3F435A375f69f43a104b4caF");
    }
}
