using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;

namespace Rawrshak
{
    [Serializable]
    public class ABData : ScriptableObject
    {
        public string mHash;
        public string mName;
        public string mFileLocation;
        public string mStatus;
        public int mNumOfConfirmations;
        public string mTransactionId;
        public string mUri;
        public string mUploadTimestamp;
        public string mUploaderAddress;
        public List<string> mAssets;
        public Int64 mFileSize;
        public SupportedBuildTargets mBuildTarget;
        public SupportedEngine mEngine = SupportedEngine.Unity;
        public string mUnityVersion;
        public int mVersion;
        public float mUploadCost;

        // Non-Serialized Data
        [NonSerialized]
        public Hash128 mHashId;
        [NonSerialized]
        public TemplateContainer mVisualElement;
        [NonSerialized] 
        public bool mMarkedForDelete;
        [NonSerialized] 
        public bool mSelectedForUploading;

        public static ABData CreateInstance(Hash128 hash, string name, string fileLocation, SupportedBuildTargets buildTarget)
        {
            var data = ScriptableObject.CreateInstance<ABData>();
            data.Init(hash, name, fileLocation, buildTarget);
            return data;
        }

        private void Init(Hash128 hash, string name, string fileLocation, SupportedBuildTargets buildTarget)
        {
            mHash = hash.ToString();
            mName = name;
            mFileLocation = fileLocation;
            mStatus = "Untracked";
            mNumOfConfirmations = 0;
            mTransactionId = String.Empty;
            mUri = String.Empty;
            mHashId = hash;
            mVisualElement = null;
            mSelectedForUploading = false;
            mMarkedForDelete = false;
            mUploadTimestamp = String.Empty;
            mUploaderAddress = String.Empty;
            mBuildTarget = buildTarget;
            mUnityVersion = Application.unityVersion;
            mVersion = 0;
            mUploadCost = 0.0f;
            UpdateFileSize();
            UpdateAssetNames();
        }

        public void SetHash128()
        {
            mHashId = Hash128.Parse(mHash);
        }

        public void UpdateAssetNames()
        {
            var assetBundle = AssetBundle.LoadFromFile(mFileLocation);
            if (assetBundle == null)
            {
                Debug.LogError("Asset Bundle Not Loaded: " + mName);
                return;
            }

            mAssets = new List<string>();
            var names = assetBundle.GetAllAssetNames();
            foreach(string name in names)
            {
                mAssets.Add(name);
            }
            assetBundle.Unload(true);
        }

        public void UpdateFileSize()
        {
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(mFileLocation);
            if (fileInfo.Exists)
            {
                mFileSize = fileInfo.Length;
            }
            else
            {
                mFileSize = 0;
            }
        }
    }
}