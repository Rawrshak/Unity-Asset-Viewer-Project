using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetWalletAssetsFromContractSample : MonoBehaviour
{
    public Rawrshak.GetWalletAssetsFromContract.ReturnData data;

    // Start is called before the first frame update
    async void Start()
    {
        data = await Rawrshak.GetWalletAssetsFromContract.Fetch("0xB796BCe3db9A9DFb3F435A375f69f43a104b4caF", "0xc9EBafF8237740353E0dEd89130fB83be4bd3F90", 3, "");
        // await Fetch("0xB796BCe3db9A9DFb3F435A375f69f43a104b4caF", "0xc9EBafF8237740353E0dEd89130fB83be4bd3F90", 3, "0xc9EBafF8237740353E0dEd89130fB83be4bd3F90-0xB796BCe3db9A9DFb3F435A375f69f43a104b4caF-1");
    }
}
