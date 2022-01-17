using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Rawrshak;

public class AudioAssetScript : MonoBehaviour
{    
    public string accountAddress;
    string lastItemId;
    public string contractAddress;
    public string tokenId;
    public int amount;

    public string assetName;

    private Rawrshak.Asset audioAsset;
    public AudioSource m_source;
    private Rawrshak.GetWalletAssetsOfType.ReturnData data;
    
    // Start is called before the first frame update
    async void Start()
    {
        audioAsset = ScriptableObject.CreateInstance<Rawrshak.Asset>();
        data = await Rawrshak.GetWalletAssetsOfType.Fetch(accountAddress, "audio", 1, lastItemId);
        if (data.data.account.assetBalances.Length > 0)
        {
            lastItemId = data.data.account.assetBalances[0].id;
            contractAddress = data.data.account.assetBalances[0].asset.parentContract.id;
            tokenId = data.data.account.assetBalances[0].asset.tokenId;

            // There might be a delay between the transaction and when the graphql updates this amount. 
            // Alternatively, you could use ContentManager.BalanceOf() to get the correct value.
            amount = data.data.account.assetBalances[0].amount;

            // Query the Audio Asset Metadata
            audioAsset.contractAddress = data.data.account.assetBalances[0].asset.parentContract.id;
            audioAsset.tokenId = data.data.account.assetBalances[0].asset.tokenId;

            // Load Audio Asset info
            if (!(await audioAsset.Load()))
            {
                Debug.LogError("Audio Asset metadata was not loaded properly.");
                return;
            }

            audioAsset.CreateAssetComponent();

            if (!audioAsset.IsAudioAsset())
            {
                Debug.LogError("Asset loaded is not an Audio Asset!");
                return;
            }
        

            var assetComponent = (Rawrshak.AudioAssetBase)audioAsset.assetComponent;
            if (!assetComponent.IsValidAsset())
            {
                Debug.LogError("Invalid Audio Metadata.");
                return;
            }

            await LoadAudioAssets();
        }
    }

    public async Task LoadAudioAssets()
    {
        assetName = audioAsset.assetName;
        
        // Load the Audio file
        Rawrshak.AudioAssetBase assetComponent = audioAsset.assetComponent as Rawrshak.AudioAssetBase;
        List<Rawrshak.AudioAssetBase.ContentTypes> contentTypes = assetComponent.GetAvailableContentTypes();

        if (contentTypes.Count == 0)
        {
            Debug.LogError("No supported audio files available for the audio clip");
            return;
        }

        m_source.clip = await assetComponent.LoadAndSetAudioClipFromContentType(contentTypes[0], AudioAssetBase.CompressionType.Raw);
    }
}
