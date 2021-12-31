using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Rawrshak;

public class ImageAssetScript : MonoBehaviour
{    
    public string accountAddress;
    string lastItemId;
    public string contractAddress;
    public string tokenId;
    public int amount;

    public string name;

    private Rawrshak.Asset imageAsset;
    private Material m_material;
    private Rawrshak.GetWalletAssetsOfType.ReturnData data;
    
    // Start is called before the first frame update
    async void Start()
    {
        imageAsset = ScriptableObject.CreateInstance<Rawrshak.Asset>();
        
        // Set internal values
        if (GetComponent<MeshRenderer>() == null)
        {
            DisableManagerWithMessage("ImageAssetManager: Image Frame does not contain a mesh renderer. ImageAssetManager will be disabled");
            return;
        }
        m_material = GetComponent<MeshRenderer>().material;
        Debug.Log("Material Name: " + m_material.name);

        data = await Rawrshak.GetWalletAssetsOfType.Fetch(accountAddress, "image", 1, lastItemId);
        if (data.data.account.assetBalances.Length > 0)
        {
            lastItemId = data.data.account.assetBalances[0].id;
            contractAddress = data.data.account.assetBalances[0].asset.parentContract.id;
            tokenId = data.data.account.assetBalances[0].asset.tokenId;

            // There might be a delay between the transaction and when the graphql updates this amount. 
            // Alternatively, you could use ContentManager.BalanceOf() to get the correct value.
            amount = data.data.account.assetBalances[0].amount;

            // Query the Image Asset Metadata
            imageAsset.contractAddress = data.data.account.assetBalances[0].asset.parentContract.id;
            imageAsset.tokenId = data.data.account.assetBalances[0].asset.tokenId;

            // Load Image Asset info
            if (!(await imageAsset.Load()))
            {
                Debug.LogError("Image Asset metadata was not loaded properly.");
                return;
            }

            imageAsset.CreateAssetComponent();

            if (!imageAsset.IsImageAsset())
            {
                Debug.LogError("Asset loaded is not an Image Asset!");
                return;
            }
        

            var assetComponent = (Rawrshak.ImageAssetBase)imageAsset.assetComponent;
            if (!assetComponent.IsValidAsset())
            {
                Debug.LogError("Invalid Image Metadata.");
                return;
            }

            await LoadImageAssets();
        }
    }

    public async Task LoadImageAssets()
    {
        name = imageAsset.assetName;
        
        // Load the Image Texture 2D
        Rawrshak.ImageAssetBase imageAssetComponent = imageAsset.assetComponent as Rawrshak.ImageAssetBase;
        List<List<int>> resolutions = imageAssetComponent.GetAvailableResolutions();

        if (resolutions.Count == 0)
        {
            Debug.LogError("No resolutions available for the image");
            return;
        }

        if (!m_material) 
        {
            Debug.Log("Material doesn't exist.");
        }

        // Just use the default for now
        m_material.mainTexture = await imageAssetComponent.LoadAndSetTexture2D(resolutions[0][0], resolutions[0][1]);
    }


    void DisableManagerWithMessage(string msg)
    {
        Debug.LogError(msg);
        enabled = false;
    }
}
