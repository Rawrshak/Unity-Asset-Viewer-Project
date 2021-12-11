using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.IO;
using System.Collections.Generic;

namespace Rawrshak
{
    public class UploadConfig : ScriptableObject
    {
        public string gatewayUri;
        public string walletAddress;
        public string walletBalance;
        public string walletFiatBalance;

        // DWS
        public string dwsBucketName;
        public string dwsFolderPath;
        public string dwsApiKey;
        
        Box mHelpBoxHolder;


        public static UploadConfig CreateInstance()
        {
            var config = ScriptableObject.CreateInstance<UploadConfig>();
            
            config.gatewayUri = "http://arweave.net";
            config.walletAddress = String.Empty;
            config.walletBalance = "0.0 AR";
            config.walletFiatBalance = "0.00 USD";
            config.dwsBucketName = String.Empty;
            config.dwsFolderPath = String.Empty;
            config.dwsApiKey = String.Empty;

            return config;
        }
    }
}
