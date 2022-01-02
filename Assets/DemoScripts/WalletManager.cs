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
    public Wallet m_rawrshakWallet;

    public Static3dAssetManager m_static3dAssetManager;
    public AudioAssetManager m_audioAssetManager;
    public TextAssetManager m_textAssetManager;
    public ImageAssetManager m_imageAssetManager;

    public Button m_refreshButton;
    public Button m_loadNewWalletButton;

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

        m_refreshButton.onClick.AddListener(async () => await RefreshWallet());
        m_loadNewWalletButton.onClick.AddListener(async () => await ClearWallet());
    }

    void OnDisable()
    {
        m_walletConnect.CLearSession();
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
        List<KeyValuePair<Asset, int>> textAssets = await m_rawrshakWallet.GetAssetsOfType("text", 100, String.Empty);
        Debug.Log("Loading Text Assets: " + textAssets.Count);
        await m_textAssetManager.LoadTextAssets(textAssets);

        // Load Image objects
        List<KeyValuePair<Asset, int>> imageAssets = await m_rawrshakWallet.GetAssetsOfType("image", 100, String.Empty);
        Debug.Log("Loading Image Assets: " + imageAssets.Count);
        await m_imageAssetManager.LoadImageAssets(imageAssets);

        // Audio Image objects
        List<KeyValuePair<Asset, int>> audioAssets = await m_rawrshakWallet.GetAssetsOfType("audio", 100, String.Empty);
        Debug.Log("Loading Audio Assets: " + audioAssets.Count);
        await m_audioAssetManager.LoadAudioAssets(audioAssets);

        // Static 3d Object Image objects
        List<KeyValuePair<Asset, int>> staticObjectAssets = await m_rawrshakWallet.GetAssetsOfType("static3dobject", 100, String.Empty);
        Debug.Log("Loading Static Objects Assets: " + staticObjectAssets.Count);
        await m_static3dAssetManager.LoadStaticObjectAssets(staticObjectAssets);
    }

    public async Task RefreshWallet()
    {
        if (m_walletConnect.Connected)
        {
            Debug.Log("Reloading Wallet Assets.");

            // Load Text objects
            List<KeyValuePair<Asset, int>> textAssets = await m_rawrshakWallet.GetAssetsOfType("text", 100, String.Empty);
            Debug.Log("Loading Text Assets: " + textAssets.Count);
            m_textAssetManager.ClearAssets();
            await m_textAssetManager.LoadTextAssets(textAssets);

            // Load Image objects
            List<KeyValuePair<Asset, int>> imageAssets = await m_rawrshakWallet.GetAssetsOfType("image", 100, String.Empty);
            Debug.Log("Loading Image Assets: " + imageAssets.Count);
            m_imageAssetManager.ClearAssets();
            await m_imageAssetManager.LoadImageAssets(imageAssets);

            // Audio Image objects
            List<KeyValuePair<Asset, int>> audioAssets = await m_rawrshakWallet.GetAssetsOfType("audio", 100, String.Empty);
            Debug.Log("Loading Audio Assets: " + audioAssets.Count);
            m_audioAssetManager.ClearAssets();
            await m_audioAssetManager.LoadAudioAssets(audioAssets);

            // Static 3d Object Image objects
            List<KeyValuePair<Asset, int>> staticObjectAssets = await m_rawrshakWallet.GetAssetsOfType("static3dobject", 100, String.Empty);
            Debug.Log("Loading Static Objects Assets: " + staticObjectAssets.Count);
            m_static3dAssetManager.ClearAssets();
            await m_static3dAssetManager.LoadStaticObjectAssets(staticObjectAssets);
        }
        else
        {
            Debug.LogError("No Wallet Loaded");
        }
    }

    public async Task ClearWallet()
    {
        // Todo: Add Clear Wallet and Reload
        if (m_walletConnect.Connected)
        {
            // Clearing WalletConnect
            m_walletConnect.CLearSession();

            Debug.Log("Clearing Wallet Assets.");
            m_loadWalletUI.SetActive(true);
            m_loggedInUI.SetActive(false);
            
            m_walletAddressText.text = $"Wallet Address:"; 

            m_rawrshakWallet.Reset();

            m_textAssetManager.ClearAssets();
            m_imageAssetManager.ClearAssets();
            m_audioAssetManager.ClearAssets();
            m_static3dAssetManager.ClearAssets();
            
            await m_walletConnect.Connect();
        }
    }
}
