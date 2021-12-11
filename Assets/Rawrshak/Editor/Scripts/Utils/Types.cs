using UnityEngine;

namespace Rawrshak {

    public enum WalletType {
        None,
        PrivateWallet,
        WalletConnect
    };
    
    public enum EthereumNetwork {
        Mainnet,
        Rinkby,
        Kovan,
        Localhost
    };

    public enum SupportedBuildTargets {
        StandaloneWindows,
        StandaloneWindows64,
        Android,
        iOS,
        WebGL
    };

    public enum SupportedEngine {
        Unity
    };
    
    // [Flags]
    public enum Role
    {
        None = 0,
        Minter = 1 << 0,
        Burner = 1 << 1
    };

    public enum AssetType
    {
        Text,
        Image,
        Audio,
        Static_Object
    };

    public enum AssetSubtype
    {
        Text_Custom,
        Text_Title,
        Text_Lore,
        Image_Custom,
        Image_Standard_Texture,
        Image_Spray,
        Image_Emblem,
        Image_Banner,
        Image_Decal,
        Audio_Custom,
        Audio_Sound_Effect,
        Audio_Voice_Line,
        Audio_BGM,
        Static_Object_Custom,
        Static_Object_Trophy
    };
}