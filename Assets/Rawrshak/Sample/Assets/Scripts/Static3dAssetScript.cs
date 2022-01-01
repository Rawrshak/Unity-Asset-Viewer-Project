using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Rawrshak;

public class Static3dAssetScript : MonoBehaviour
{    
    public GameObject m_defaultObject;
    public string accountAddress;
    string lastItemId;
    public string contractAddress;
    public string tokenId;
    public int amount;

    public string assetName;

    private Rawrshak.Asset staticAsset;
    private Rawrshak.GetWalletAssetsOfType.ReturnData data;
    
    // Start is called before the first frame update
    async void Start()
    {
        // Todo: Fix orientation and location of the asset
        staticAsset = ScriptableObject.CreateInstance<Rawrshak.Asset>();
        data = await Rawrshak.GetWalletAssetsOfType.Fetch(accountAddress, "static3dobject", 1, lastItemId);
        if (data.data.account.assetBalances.Length > 0)
        {
            lastItemId = data.data.account.assetBalances[0].id;
            contractAddress = data.data.account.assetBalances[0].asset.parentContract.id;
            tokenId = data.data.account.assetBalances[0].asset.tokenId;

            // There might be a delay between the transaction and when the graphql updates this amount. 
            // Alternatively, you could use ContentManager.BalanceOf() to get the correct value.
            amount = data.data.account.assetBalances[0].amount;

            // Query the Static 3D Asset Metadata
            staticAsset.contractAddress = data.data.account.assetBalances[0].asset.parentContract.id;
            staticAsset.tokenId = data.data.account.assetBalances[0].asset.tokenId;

            // Load Static 3D Asset info
            if (!(await staticAsset.Load()))
            {
                Debug.LogError("Static 3d Asset metadata was not loaded properly.");
                return;
            }

            staticAsset.CreateAssetComponent();

            if (!staticAsset.IsStatic3dObjectAsset())
            {
                Debug.LogError("Asset loaded is not an Static 3d Asset!");
                return;
            }
        

            var assetComponent = (Rawrshak.Static3dObjectAssetBase)staticAsset.assetComponent;
            if (!assetComponent.IsValidAsset())
            {
                Debug.LogError("Invalid Audio Metadata.");
                return;
            }

            await LoadStatic3dAssets();
        }
    }

    public async Task LoadStatic3dAssets()
    {
        assetName = staticAsset.assetName;
        
        // Load the Static 3D Object
        Rawrshak.Static3dObjectAssetBase assetComponent = staticAsset.assetComponent as Rawrshak.Static3dObjectAssetBase;
        List<Rawrshak.Static3dObjectAssetBase.Platform> supportedPlatforms = assetComponent.GetSupportedPlatforms();
        List<Rawrshak.Static3dObjectAssetBase.Fidelity> supportedFidelities = assetComponent.GetSupportedFidelities();
        List<Rawrshak.Static3dObjectAssetBase.RenderPipeline> supportedRenderPipelines = assetComponent.GetSupportedRenderPipelines();

        // Find StandaloneWindows Platform
        bool platformFound = false;
        foreach (var platform in supportedPlatforms)
        {
            if (platform == Rawrshak.Static3dObjectAssetBase.Platform.StandaloneWindows)
            {
                platformFound = true;
                break;
            }
        }

        // Find Render Pipeline BuiltInRenderPipeline
        bool renderPipelineFound = false;
        foreach (var renderPipeline in supportedRenderPipelines)
        {
            if (renderPipeline == Rawrshak.Static3dObjectAssetBase.RenderPipeline.BuiltInRenderPipeline)
            {
                renderPipelineFound = true;
                break;
            }
        }
        
        // Todo: Default to low fidelity only
        bool fidelityFound = false;
        if (supportedFidelities.Count > 0)
        {
            fidelityFound = true;
        }

        if (!platformFound || !renderPipelineFound || !fidelityFound)
        {
            Debug.LogError("No supported prefab available for this game");
            return;
        }

        GameObject prefab = await assetComponent.LoadAndSetPrefab(
            Static3dObjectAssetBase.Platform.StandaloneWindows, 
            Static3dObjectAssetBase.RenderPipeline.BuiltInRenderPipeline,
            // Static3dObjectAssetBase.Fidelity.Low);
            supportedFidelities[0]);
        GameObject prefabInstance = Instantiate(prefab, transform.position, transform.rotation);
        Destroy(m_defaultObject);
        m_defaultObject = prefabInstance;
        m_defaultObject.transform.parent = transform;
    }
}
