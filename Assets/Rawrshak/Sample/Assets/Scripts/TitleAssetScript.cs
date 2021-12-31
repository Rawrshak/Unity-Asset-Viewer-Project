using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TitleAssetScript : MonoBehaviour
{
    // Input
    public string accountAddress;
    string lastItemId;
    public string contractAddress;
    public string tokenId;
    public int amount;
    public string title;
    public string description;
    private Rawrshak.Asset textAsset;
    
    // Return Value
    private Rawrshak.GetWalletAssetsOfType.ReturnData data;
    
    // Start is called before the first frame update
    async void Start()
    {
        textAsset = ScriptableObject.CreateInstance<Rawrshak.Asset>();

        // Get the first text asset that is owned by the wallet
        data = await Rawrshak.GetWalletAssetsOfType.Fetch(accountAddress, "text", 1, lastItemId);
        if (data.data.account.assetBalances.Length > 0)
        {
            lastItemId = data.data.account.assetBalances[0].id;
            contractAddress = data.data.account.assetBalances[0].asset.parentContract.id;
            tokenId = data.data.account.assetBalances[0].asset.tokenId;

            // There might be a delay between the transaction and when the graphql updates this amount. 
            // Alternatively, you could use ContentManager.BalanceOf() to get the correct value.
            amount = data.data.account.assetBalances[0].amount;

            // Query the Text Asset Metadata
            textAsset.contractAddress = data.data.account.assetBalances[0].asset.parentContract.id;
            textAsset.tokenId = data.data.account.assetBalances[0].asset.tokenId;

            // Load Text Asset info
            if (!(await textAsset.Load()))
            {
                Debug.LogError("Text Asset metadata was not loaded properly.");
                return;
            }

            textAsset.CreateAssetComponent();

            if (!textAsset.IsTextAsset())
            {
                Debug.LogError("Asset loaded is not a Text Asset!");
                return;
            }

            var assetComponent = (Rawrshak.TextAssetBase)textAsset.assetComponent;
            if (!assetComponent.IsValidAsset())
            {
                Debug.LogError("Invalid Title Metadata.");
                return;
            }
            title = assetComponent.GetTitle();
            description = assetComponent.GetDescription();
        }
    }
}
