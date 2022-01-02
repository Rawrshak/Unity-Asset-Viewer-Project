using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Rawrshak;

public class TextAssetManager : MonoBehaviour
{
    public GameObject m_textAsset;
    public Dropdown m_textSelectorDropdown;

    private Text m_text;
    private List<KeyValuePair<Asset, int>> m_assets;

    // Start is called before the first frame update
    void Start()
    {
        if (m_textAsset == null)
        {
            Debug.LogError("TextAssetManager: No Text UI was set. TextAssetManager will be disabled");
            enabled = false;
            return;
        }

        if (m_textSelectorDropdown == null)
        {
            Debug.LogError("TextAssetManager: No Text Selector Dropdown UI was set. TextAssetManager will be disabled");
            enabled = false;
            return;
        }

        m_assets = new List<KeyValuePair<Asset, int>>();
        m_text = m_textAsset.GetComponent<Text>();
        m_text.text = "Gamer Title:";

        ClearAssets();
        m_textSelectorDropdown.onValueChanged.AddListener(delegate {
            TitleSelectedValueChanged();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async Task LoadTextAssets(List<KeyValuePair<Asset, int>> textAssets)
    {
        m_assets = textAssets;
        foreach (var pair in m_assets)
        {
            await pair.Key.Load();
            Debug.Log($"Asset: {pair.Key.assetName}, amount: {pair.Value}");
            
            // Load text asset component
            pair.Key.CreateAssetComponent();
            
            Dropdown.OptionData data = new Dropdown.OptionData();
            data.text = pair.Key.assetName;
            m_textSelectorDropdown.options.Add(data);
        }

        m_textSelectorDropdown.RefreshShownValue();
        m_textSelectorDropdown.value = 0;
        TitleSelectedValueChanged();
    }

    public void ClearAssets()
    {   
        // Clear All Saved Assets
        m_assets.Clear();

        // Clear text
        m_text.text = "Gamer Title:";

        // Clear dropdown
        m_textSelectorDropdown.ClearOptions();
        m_textSelectorDropdown.value = 0;
    }

    void TitleSelectedValueChanged()
    {
        if (m_textSelectorDropdown.value < m_assets.Count)
        {
            Asset asset = m_assets[m_textSelectorDropdown.value].Key;
            m_text.text = "Gamer Title: " + (asset.assetComponent as TitleAsset).GetTitle();
        }
    }

    // Todo: Load after refresh
}
