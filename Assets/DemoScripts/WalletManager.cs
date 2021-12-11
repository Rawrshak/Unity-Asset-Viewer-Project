using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using WalletConnectSharp.Unity;
using WalletConnectSharp.Core;
using WalletConnectSharp.Core.Models;
using Rawrshak;

public class WalletManager : MonoBehaviour
{
    public GameObject m_loadWalletUI;
    public GameObject m_loggedInUI;
    public WalletConnect m_walletConnect;
    public Text m_walletAddressText;
    public RawrshakWallet m_rawrshakWallet;

    public Static3dAssetManager m_static3dAssetManager;
    public AudioAssetManager m_audioAssetManager;
    public TextAssetManager m_textAssetManager;
    public ImageAssetManager m_imageAssetManager;

    // Start is called before the first frame update
    void Start()
    {
        if (!m_walletConnect)
        {
            Debug.LogError("WalletManager: No WalletConnect object given. Wallet Manager will be disabled");
            enabled = false;
            return;
        }

        m_walletConnect.ConnectedEvent.AddListener(async delegate
        {
            await WalletConnectedEventHandler();
        });

        // Get the instance of the TextAssetManager
        m_textAssetManager = GameObject.FindObjectOfType<TextAssetManager>();
        m_imageAssetManager = GameObject.FindObjectOfType<ImageAssetManager>();
        m_audioAssetManager = GameObject.FindObjectOfType<AudioAssetManager>();
        m_static3dAssetManager = GameObject.FindObjectOfType<Static3dAssetManager>();
        m_rawrshakWallet = GameObject.FindObjectOfType<RawrshakWallet>();
    }

    private async Task WalletConnectedEventHandler()
    {
        Debug.Log("Logged In Wallet: " + WalletConnect.ActiveSession.Accounts[0]);
        m_loadWalletUI.SetActive(false);
        m_loggedInUI.SetActive(true);
        
        m_walletAddressText.text = $"Wallet Address: {WalletConnect.ActiveSession.Accounts[0]}"; 

        // Load Rawrshak Wallet Info
        await m_rawrshakWallet.LoadAccountInfo(WalletConnect.ActiveSession.Accounts[0]);

        // Load Text objects
        List<KeyValuePair<RawrshakAsset, int>> textAssets = await m_rawrshakWallet.GetAssetsOfType("text", 100, String.Empty);
        Debug.Log("Loading Text Assets: " + textAssets.Count);
        await m_textAssetManager.LoadTextAssets(textAssets);

        // Load Image objects
        List<KeyValuePair<RawrshakAsset, int>> imageAssets = await m_rawrshakWallet.GetAssetsOfType("image", 100, String.Empty);
        Debug.Log("Loading Image Assets: " + imageAssets.Count);
        await m_imageAssetManager.LoadImageAssets(imageAssets);

        // Audio Image objects
        List<KeyValuePair<RawrshakAsset, int>> audioAssets = await m_rawrshakWallet.GetAssetsOfType("audio", 100, String.Empty);
        Debug.Log("Loading Audio Assets: " + audioAssets.Count);
        await m_audioAssetManager.LoadAudioAssets(audioAssets);

        // Static 3d Object Image objects
        List<KeyValuePair<RawrshakAsset, int>> staticObjectAssets = await m_rawrshakWallet.GetAssetsOfType("static3dobject", 100, String.Empty);
        Debug.Log("Loading Static Objects Assets: " + staticObjectAssets.Count);
        await m_static3dAssetManager.LoadStaticObjectAssets(staticObjectAssets);
    }
}
