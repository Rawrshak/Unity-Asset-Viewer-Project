using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Rawrshak;

public class ImageAssetManager : MonoBehaviour
{
    public GameObject m_imageFrame;
    public Dropdown m_imageSelectorDropdown;
    
    private List<KeyValuePair<Asset, int>> m_assets;
    private Material m_material;
    private Texture m_defaultTexture;
    
    // Start is called before the first frame update
    void Start()
    {
        if (m_imageFrame == null)
        {
            DisableManagerWithMessage("ImageAssetManager: No Image Frame was set. ImageAssetManager will be disabled");
            return;
        }

        if (m_imageSelectorDropdown == null)
        {
            DisableManagerWithMessage("ImageAssetManager: No Image Selector Dropdown UI was set. ImageAssetManager will be disabled");
            return;
        }
        
        m_assets = new List<KeyValuePair<Asset, int>>();
        
        m_material = GetComponent<MeshRenderer>().material;
        Debug.Log("Material Name: " + m_material.name);
        m_defaultTexture = m_material.mainTexture;
        
        ClearAssets();
        m_imageSelectorDropdown.onValueChanged.AddListener(async delegate {
            await ImageSelectedValueChanged();
        });

        // Set internal values
        if (GetComponent<MeshRenderer>() == null)
        {
            DisableManagerWithMessage("ImageAssetManager: Image Frame does not contain a mesh renderer. ImageAssetManager will be disabled");
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClearAssets()
    {   
        // Clear All Saved Assets
        m_assets.Clear();
        
        m_material.mainTexture = m_defaultTexture;

        // Clear dropdown
        m_imageSelectorDropdown.ClearOptions();
        m_imageSelectorDropdown.value = 0;
    }

    public async Task LoadImageAssets(List<KeyValuePair<Asset, int>> imageAssets)
    {
        m_assets = imageAssets;
        foreach (var pair in imageAssets)
        {
            await pair.Key.Load();
            Debug.Log($"Asset: {pair.Key.assetName}, amount: {pair.Value}");
            
            // Load text asset component
            pair.Key.CreateAssetComponent();
            
            Dropdown.OptionData data = new Dropdown.OptionData();
            data.text = pair.Key.assetName;
            m_imageSelectorDropdown.options.Add(data);
        }
        
        m_imageSelectorDropdown.RefreshShownValue();
        m_imageSelectorDropdown.value = 0;
        await ImageSelectedValueChanged();
    }

    async Task ImageSelectedValueChanged()
    {
        if (m_imageSelectorDropdown.value < m_assets.Count)
        {
            Asset asset = m_assets[m_imageSelectorDropdown.value].Key;
            Debug.Log("Image Name: " + asset.assetName);
            
            // Load the Image Texture 2D
            ImageAssetBase imgAsset = asset.assetComponent as ImageAssetBase;
            List<List<int>> resolutions = imgAsset.GetAvailableResolutions();

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
            m_material.mainTexture = await imgAsset.LoadAndSetTexture2D(resolutions[0][0], resolutions[0][1]);
        }
    }

    void DisableManagerWithMessage(string msg)
    {
        Debug.LogError(msg);
        enabled = false;
    }
}
