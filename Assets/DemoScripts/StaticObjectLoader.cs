using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rawrshak;

public class StaticObjectLoader : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject prefab;
    async void Start()
    {
        AssetBundle abundle = await Downloader.DownloadAssetBundle("https://arweave.net/yIu_zPYJdJJ6OCX3j4gRphncT1PmVLqia3E217ifmCM");

        prefab = abundle.LoadAsset<GameObject>("assets/rawrshak/sample/demo/regular/ina/smol_ina.prefab");

        Instantiate(prefab, transform.position, transform.rotation);
        abundle.Unload(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
