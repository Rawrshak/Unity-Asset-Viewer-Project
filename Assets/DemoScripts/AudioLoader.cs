using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rawrshak;

public class AudioLoader : MonoBehaviour
{
    AudioSource source;
    // Start is called before the first frame update
    async void Start()
    {
        AssetBundle abundle = await Downloader.DownloadAssetBundle("https://arweave.net/uWCilCor8E6jYlrK5B-H51qtSEU9s--01Lt5E67fFiA");

        source = GetComponent<AudioSource>();
        source.clip = abundle.LoadAsset<AudioClip>("assets/rawrshak/sample/demo/audio/damnsonwheredyoufindthissoundeffect.mp3");
        abundle.Unload(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("tab"))
        {
            Debug.Log("Playing Audio Clip...");
            source.Play();
        }
    }
}
