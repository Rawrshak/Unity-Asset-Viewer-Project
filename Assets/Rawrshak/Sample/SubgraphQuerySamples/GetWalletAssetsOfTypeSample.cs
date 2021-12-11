using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetWalletAssetsOfTypeSample : MonoBehaviour
{
    public Rawrshak.GetWalletAssetsOfType.ReturnData data;

    // Start is called before the first frame update
    async void Start()
    {
        data = await Rawrshak.GetWalletAssetsOfType.Fetch("0xB796BCe3db9A9DFb3F435A375f69f43a104b4caF", "text", 3, "");
    }
}
