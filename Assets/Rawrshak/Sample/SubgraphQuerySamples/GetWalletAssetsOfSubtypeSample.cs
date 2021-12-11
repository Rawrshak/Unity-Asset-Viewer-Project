using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetWalletAssetsOfSubtypeSample : MonoBehaviour
{
    public Rawrshak.GetWalletAssetsOfSubtype.ReturnData data;

    // Start is called before the first frame update
    async void Start()
    {
        data = await Rawrshak.GetWalletAssetsOfSubtype.Fetch("0xB796BCe3db9A9DFb3F435A375f69f43a104b4caF", "title", 3, "");
    }
}
