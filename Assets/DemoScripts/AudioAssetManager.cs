using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Rawrshak;

public class AudioAssetManager : MonoBehaviour
{
    public AudioSource m_source;
    public Dropdown m_audioSelectorDropdown;

    private List<KeyValuePair<Asset, int>> m_assets;

    // Start is called before the first frame update
    void Start()
    {
        if (m_source == null)
        {
            Debug.LogError("AudioAssetManager: No Audio Source was set. AudioAssetManager will be disabled");
            enabled = false;
            return;
        }

        if (m_audioSelectorDropdown == null)
        {
            Debug.LogError("AudioAssetManager: No Audio Selector Dropdown UI was set. AudioAssetManager will be disabled");
            enabled = false;
            return;
        }
        
        m_assets = new List<KeyValuePair<Asset, int>>();
        
        ClearAssets();
        m_audioSelectorDropdown.onValueChanged.AddListener(async delegate {
            await AudioSelectedValueChanged();
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.name == transform.name)
                {
                    if (m_source.clip)
                    {
                        m_source.Play();
                    }
                    else
                    {
                        Debug.LogError("No Audio Clip selected.");
                    }
                }
            }
        }
    }

    public void ClearAssets()
    {   
        // Clear All Saved Assets
        m_assets.Clear();

        // Clear audio clip
        m_source.clip = null;

        // Clear dropdown
        m_audioSelectorDropdown.ClearOptions();
        m_audioSelectorDropdown.value = 0;
    }

    public async Task LoadAudioAssets(List<KeyValuePair<Asset, int>> audioAssets)
    {
        ClearAssets();
        foreach (var pair in audioAssets)
        {
            // Load metadata
            if (!(await pair.Key.Load()))
            {
                Debug.LogError($"Unable to load Asset: {pair.Key.assetName}");
                continue;
            }
            Debug.Log($"Asset: {pair.Key.assetName}, amount: {pair.Value}");

            // Load text asset component
            pair.Key.CreateAssetComponent();
            
            Dropdown.OptionData data = new Dropdown.OptionData();
            data.text = IsAudioSupported(pair.Key) ? pair.Key.assetName : "[WebGL Unsupported] " + pair.Key.assetName;
            m_assets.Add(pair);
            m_audioSelectorDropdown.options.Add(data);
        }
        
        m_audioSelectorDropdown.RefreshShownValue();
        m_audioSelectorDropdown.value = 0;
        await AudioSelectedValueChanged();
    }

    async Task AudioSelectedValueChanged()
    {
        if (m_audioSelectorDropdown.value < m_assets.Count)
        {
            // Clearing source clip
            m_source.clip = null;

            Asset asset = m_assets[m_audioSelectorDropdown.value].Key;
            Debug.Log("Audio Name: " + asset.assetName);

            // Load the Audio Clip
            AudioAssetBase audioAsset = asset.assetComponent as AudioAssetBase;

            if (!IsAudioSupported(asset))
            {
                Debug.LogError("No supported audio files available for the audio clip");
                return;
            }

            // Just use the default for now
            if (audioAsset.IsAudioTypeSupported(AudioType.MPEG))
            {
                m_source.clip = await audioAsset.LoadAndSetAudioClipFromAudioType(AudioType.MPEG);
            }
            else if (audioAsset.IsAudioTypeSupported(AudioType.WAV))
            {
                m_source.clip = await audioAsset.LoadAndSetAudioClipFromAudioType(AudioType.WAV);
            }
            else if (audioAsset.IsAudioTypeSupported(AudioType.OGGVORBIS))
            {
                m_source.clip = await audioAsset.LoadAndSetAudioClipFromAudioType(AudioType.OGGVORBIS);
            }
            else if (audioAsset.IsAudioTypeSupported(AudioType.AIFF))
            {
                m_source.clip = await audioAsset.LoadAndSetAudioClipFromAudioType(AudioType.AIFF);
            }

            if (m_source.clip == null) {
                Debug.LogError("Error: Couldn't load \"" + asset.assetName + "\" audio clip.");
            }
        }
    }

    bool IsAudioSupported(Asset asset)
    {
        AudioAssetBase audioAsset = asset.assetComponent as AudioAssetBase;

        List<AudioType> audioTypes = audioAsset.GetAvailableAudioTypes();
        if (audioTypes.Count > 0)
        {
            return true;
        }
        return false;
    }
}
