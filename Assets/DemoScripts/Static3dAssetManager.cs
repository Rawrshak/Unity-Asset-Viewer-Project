using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Rawrshak;

public class Static3dAssetManager : MonoBehaviour
{
    public GameObject m_defaultObject;
    public GameObject m_childObject;
    public GameObject m_placeholder;
    public Dropdown m_staticObjectSelectorDropdown;

    private List<KeyValuePair<Asset, int>> m_assets;

    // Start is called before the first frame update
    void Start()
    {
        if (m_defaultObject == null)
        {
            Debug.LogError("Static3dAssetManager: No Default Object was set. Static3dAssetManager will be disabled");
            enabled = false;
            return;
        }

        if (m_staticObjectSelectorDropdown == null)
        {
            Debug.LogError("Static3dAssetManager: No Static Object Selector Dropdown UI was set. Static3dAssetManager will be disabled");
            enabled = false;
            return;
        }
        
        m_assets = new List<KeyValuePair<Asset, int>>();
        
        ClearAssets();
        m_staticObjectSelectorDropdown.onValueChanged.AddListener(async delegate {
            await StaticObjectSelectedValueChanged();
        });
    }

    // Update is called once per frame
    void Update()
    {
        m_placeholder.transform.Rotate(0.0f, 0.05f, 0.0f, Space.Self);
    }

    public void ClearAssets()
    {   
        // Clear All Saved Assets
        m_assets.Clear();

        // Delete child object, enable default object
        m_defaultObject.SetActive(true);
        Destroy(m_childObject);

        // Clear dropdown
        m_staticObjectSelectorDropdown.ClearOptions();
        m_staticObjectSelectorDropdown.value = 0;
    }

    public async Task LoadStaticObjectAssets(List<KeyValuePair<Asset, int>> staticObjectAssets)
    {
        m_assets = staticObjectAssets;
        foreach (var pair in staticObjectAssets)
        {
            await pair.Key.Load();
            Debug.Log($"Asset: {pair.Key.assetName}, amount: {pair.Value}");
            
            // Load text asset component
            pair.Key.CreateAssetComponent();
            
            Dropdown.OptionData data = new Dropdown.OptionData();
            data.text = pair.Key.assetName;
            m_staticObjectSelectorDropdown.options.Add(data);
        }
        
        m_staticObjectSelectorDropdown.RefreshShownValue();
        m_staticObjectSelectorDropdown.value = 0;
        await StaticObjectSelectedValueChanged();
    }

    async Task StaticObjectSelectedValueChanged()
    {
        if (m_staticObjectSelectorDropdown.value < m_assets.Count)
        {
            Asset asset = m_assets[m_staticObjectSelectorDropdown.value].Key;
            Debug.Log("Static Object Name: " + asset.assetName);
            
            // // Load the Static 3D Object
            Static3dObjectAssetBase static3dObjectAsset = asset.assetComponent as Static3dObjectAssetBase;
            List<Static3dObjectAssetBase.Platform> supportedPlatforms = static3dObjectAsset.GetSupportedPlatforms();
            List<Static3dObjectAssetBase.Fidelity> supportedFidelities = static3dObjectAsset.GetSupportedFidelities();
            List<Static3dObjectAssetBase.RenderPipeline> supportedRenderPipelines = static3dObjectAsset.GetSupportedRenderPipelines();

            // Find StandaloneWindows Platform
            bool platformFound = false;
            foreach (var platform in supportedPlatforms)
            {
                if (platform == Static3dObjectAssetBase.Platform.StandaloneWindows)
                {
                    platformFound = true;
                    break;
                }
            }

            // Find Render Pipeline BuiltInRenderPipeline
            bool renderPipelineFound = false;
            foreach (var renderPipeline in supportedRenderPipelines)
            {
                if (renderPipeline == Static3dObjectAssetBase.RenderPipeline.BuiltInRenderPipeline)
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
            // foreach (var fidelity in supportedFidelities)
            // {
            //     if (fidelity == Static3dObjectAssetBase.Fidelity.Low)
            //     {
            //         fidelityFound = true;
            //         break;
            //     }
            // }

            if (!platformFound || !renderPipelineFound || !fidelityFound)
            {
                Debug.LogError("No supported prefab available for this game");
                return;
            }

            GameObject prefab = await static3dObjectAsset.LoadAndSetPrefab(
                Static3dObjectAssetBase.Platform.StandaloneWindows, 
                Static3dObjectAssetBase.RenderPipeline.BuiltInRenderPipeline,
                // Static3dObjectAssetBase.Fidelity.Low);
                supportedFidelities[0]);

            if (prefab == null) {
                Debug.LogError("Error: Couldn't load \"" + asset.assetName + "\" prefab.");
                return;
            }
            
            GameObject prefabInstance = Instantiate(prefab, m_placeholder.transform.position, m_placeholder.transform.rotation);
            m_defaultObject.SetActive(false);
            m_childObject = prefabInstance;
            m_childObject.transform.parent = m_placeholder.transform;
        }
    }
}
