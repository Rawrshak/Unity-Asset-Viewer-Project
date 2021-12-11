using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetAssetsInWalletSample : MonoBehaviour
{
    public Rawrshak.GetAssetsInWallet.ReturnData data;

    // Start is called before the first frame update
    async void Start()
    {
        data = await Rawrshak.GetAssetsInWallet.Fetch("0xB796BCe3db9A9DFb3F435A375f69f43a104b4caF",  2, "");
        //     await Fetch("0xB796BCe3db9A9DFb3F435A375f69f43a104b4caF",  2, "0xc9EBafF8237740353E0dEd89130fB83be4bd3F90-0xb796bce3db9a9dfb3f435a375f69f43a104b4caf-1");
    }
}
