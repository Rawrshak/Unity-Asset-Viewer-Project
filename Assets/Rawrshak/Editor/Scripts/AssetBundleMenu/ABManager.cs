using System;
using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.Events;
using System.IO;

namespace Rawrshak
{
    public class ABManager : ScriptableObject
    {
        // Singleton instance
        

        // Static Properties
        static string ASSET_BUNDLES_UPLOADED_DIRECTORY = "UploadedAssetBundles";

        // Private Properties
        private string mAssetBundleDirectory; // Asset Bundle Directory
        private AssetBundle mFolderObj; // Asset Bundle Folder Object
        private AssetBundleManifest mManifest; // Asset Bundle Folder Manifest Object
        private UnityEvent<ABData> bundleSelected = new UnityEvent<ABData>();
        // private UnityEvent<List<ABData>> mUploadBundleCallback = new UnityEvent<List<ABData>>();
        private SupportedBuildTargets mCurrentBuildTarget;
        private float mEstimatedTotalCost;

        Dictionary<string, ABData> mUntrackedAssetBundles;
        Dictionary<Hash128, ABData> mUploadedAssetBundles;
        
        // UI
        Box mUntrackedAssetBundleHolder;
        Box mUploadedAssetBundleHolder;
        Box mHelpBoxHolder;
        Label mEstimatedTotalCostLabel;
        VisualTreeAsset mUploadedBundleEntry;
        VisualTreeAsset mUntrackedBundleEntry;

        public static ABManager CreateInstance(string directory, SupportedBuildTargets buildTarget)
        {
            var manager = ScriptableObject.CreateInstance<ABManager>();

            manager.mCurrentBuildTarget = buildTarget;
            manager.LoadAssetBundle(directory, buildTarget);

            return manager;
        }

        public void OnEnable()
        {
            mUntrackedAssetBundles = new Dictionary<string, ABData>();
            mUploadedAssetBundles = new Dictionary<Hash128, ABData>();

            // Create Uploaded Asset Bundles Directory if necessary
            string directoryPath = String.Format("{0}/{1}", AssetBundleMenu.RESOURCES_FOLDER, ASSET_BUNDLES_UPLOADED_DIRECTORY);
            if(!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Load all Asset Bundle Data from storage if any
            foreach (ABData bundle in Resources.LoadAll("UploadedAssetBundles", typeof(ABData)))
            {
                // Only Load asset bundle data that are stored
                if (EditorUtility.IsPersistent(bundle))
                {
                    bundle.SetHash128();

                    // Set Non-serialized data to default values
                    if (!mUploadedAssetBundles.ContainsKey(bundle.mHashId))
                    {
                        mUploadedAssetBundles.Add(bundle.mHashId, bundle);
                        Debug.Log("Adding Uploaded Bundle: " + bundle.mName + ", ID: " + bundle.mHash);
                    }
                }
            }
        }

        public void OnDisable()
        {
            if (mFolderObj)
            {
                mFolderObj.Unload(true);
            }
            mManifest = null;

            // Save any asset bundles whose data changed
            AssetDatabase.SaveAssets();
            Debug.Log("ABManager OnDisable");
        }

        public void SetBundleSelectedCallback(UnityAction<ABData> bundleSelectedCallback)
        {
            bundleSelected.AddListener(bundleSelectedCallback);
        }

        // public void SetUploadBundleCallback(UnityAction<List<ABData>> uploadBundleCallback)
        // {
        //     mUploadBundleCallback.AddListener(uploadBundleCallback);
        // }
        
        public void LoadUI(VisualElement root)
        {
            mHelpBoxHolder = root.Query<Box>("helpbox-holder").First();

            // Asset Bundle Entries
            mUntrackedAssetBundleHolder = root.Query<Box>("new-entries").First();
            mUploadedAssetBundleHolder = root.Query<Box>("uploaded-entries").First();
            
            // Load Entry UXMLs
            mUploadedBundleEntry = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Rawrshak/Editor/UXML/AssetBundleMenu/UploadedAssetBundle.uxml");
            mUntrackedBundleEntry = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Rawrshak/Editor/UXML/AssetBundleMenu/UntrackedAssetBundle.uxml");

            // var uploadButton = root.Query<Button>("upload-button").First();
            // uploadButton.clicked += () => {
            //     var list = BuildUploadList();
            //     mUploadBundleCallback.Invoke(list);
            // };

            mEstimatedTotalCostLabel = root.Query<Label>("estimated-cost").First();
            
            ReloadUntrackedAssetBundles();

            // Load Uploaded Asset Bundles from storage
            var iter = mUploadedAssetBundles.GetEnumerator();
            while(iter.MoveNext())
            {
                AddUploadedAssetBundleForDisplay(iter.Current.Value);
            }
        }
        
        public void ReloadUntrackedAssetBundles()
        {
            // Clear helper box
            mHelpBoxHolder.Clear();

            if (mManifest)
            {
                var bundleNames = mManifest.GetAllAssetBundles();
                
                // mark all bundles for delete
                foreach(var bundle in mUntrackedAssetBundles)
                {
                    bundle.Value.mMarkedForDelete = true;
                }

                foreach(string name in bundleNames)
                {
                    var hash = mManifest.GetAssetBundleHash(name);
                    // Debug.Log("AssetBundle: " + name + ", hash: " + hash);

                    if (mUploadedAssetBundles.ContainsKey(hash))
                    {
                        continue;
                    }
                    
                    if (mUntrackedAssetBundles.ContainsKey(name))
                    {
                        var bundle = mUntrackedAssetBundles[name];
                        bundle.mHashId = hash;
                        bundle.mHash = hash.ToString();
                        bundle.mSelectedForUploading = false;
                        bundle.mFileLocation = Application.dataPath + "/" + mAssetBundleDirectory + "/" + name;
                        bundle.mVisualElement.contentContainer.Query<Label>("asset-bundle-hash").First().text = hash.ToString();
                        bundle.mVisualElement.contentContainer.Query<Toggle>("asset-bundle-selected").First().value = false;
                        bundle.mMarkedForDelete = false;
                        bundle.mBuildTarget = mCurrentBuildTarget;
                        bundle.UpdateAssetNames();
                        bundle.UpdateFileSize();
                    }
                    else
                    {
                        // find or add the asset bundle in the new asset bundle lists
                        string fileLocation = Application.dataPath + "/" + mAssetBundleDirectory + "/" + name;
                        ABData bundle = ABData.CreateInstance(hash, name, fileLocation, mCurrentBuildTarget);
                        mUntrackedAssetBundles.Add(name, bundle);

                        // Add entry to UI
                        TemplateContainer entryTree = mUntrackedBundleEntry.CloneTree();
                        entryTree.contentContainer.Query<Label>("asset-bundle-name").First().text = name;
                        entryTree.contentContainer.Query<Label>("asset-bundle-hash").First().text = hash.ToString();

                        // Set Toggle Callback
                        var selectedToggle = entryTree.contentContainer.Query<Toggle>("asset-bundle-selected").First();
                        selectedToggle.RegisterCallback<ChangeEvent<bool>>((evt) => {
                            bundle.mSelectedForUploading = (evt.target as Toggle).value;

                            // If Bundle is selected, get the estimated cost and add it to total cost;
                            EditorCoroutineUtility.StartCoroutine(UpdateEstimatedCost(bundle, bundle.mSelectedForUploading), this);
                        });

                        // Select Asset Bundle Callback to show info
                        entryTree.RegisterCallback<MouseDownEvent>((evt) => {
                            bundleSelected.Invoke(bundle);;
                        });
                        
                        bundle.mVisualElement = entryTree;

                        // Add entry to UI
                        mUntrackedAssetBundleHolder.Add(entryTree);
                    }
                }

                // Delete bundles marked for delete
                List<string> bundlesToDelete = new List<string>();
                foreach(var bundle in mUntrackedAssetBundles)
                {
                    if (bundle.Value.mMarkedForDelete)
                    {
                        mUntrackedAssetBundleHolder.Remove(bundle.Value.mVisualElement);
                        bundlesToDelete.Add(bundle.Key);
                    }
                }

                foreach(var name in bundlesToDelete)
                {
                    mUntrackedAssetBundles.Remove(name);
                }
            }
            else
            {
                Debug.Log("Manifest Doesn't exist!");
                mUntrackedAssetBundleHolder.Clear();
                mUntrackedAssetBundles.Clear();
            }

            Debug.Log("New Asset Bundle Size: " + mUntrackedAssetBundles.Count);
        }

        private void AddUploadedAssetBundleForDisplay(ABData bundle)
        {
            // Add entry to UI
            TemplateContainer entryTree = mUploadedBundleEntry.CloneTree();
            entryTree.contentContainer.Query<Label>("name").First().text = bundle.mName;
            entryTree.contentContainer.Query<Label>("hash").First().text = bundle.mHash;

            // Have "Uploading" as the status until it's finished uploading.
            entryTree.contentContainer.Query<Label>("date-uploaded").First().text = bundle.mStatus;

            // Select Asset Bundle Callback to show info
            entryTree.RegisterCallback<MouseDownEvent>((evt) => {
                bundleSelected.Invoke(bundle);;
            });
            
            bundle.mVisualElement = entryTree;

            // Add entry to UI
            mUploadedAssetBundleHolder.Add(entryTree);
        }

        public void LoadAssetBundle(string directory, SupportedBuildTargets builtTarget)
        {
            mAssetBundleDirectory = directory;
            mCurrentBuildTarget = builtTarget;
            Debug.Log("Load Asset Bundle: " + directory);
            if (mFolderObj != null)
            {
                mFolderObj.Unload(true);  
                mManifest = null; 
            }
            
            string folderObjName = builtTarget.ToString();
            if (File.Exists("Assets/" + mAssetBundleDirectory + "/" + folderObjName))
            {
                Debug.Log("Folder Exists! " + mAssetBundleDirectory + "/" + folderObjName);
                mFolderObj = AssetBundle.LoadFromFile(Application.dataPath + "/" + mAssetBundleDirectory + "/" + folderObjName);
                mManifest = mFolderObj.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            }
        }

        public void AddErrorHelpbox(string errorMsg)
        {
            mHelpBoxHolder.Add(new HelpBox(errorMsg, HelpBoxMessageType.Error));
        }
        
        private List<ABData> BuildUploadList()
        {
            var list = new List<ABData>();

            var iter = mUntrackedAssetBundles.GetEnumerator();
            while(iter.MoveNext())
            {
                var bundle = iter.Current.Value;
                if (bundle.mSelectedForUploading)
                {
                    list.Add(bundle);
                    
                    // Save uploaded bundle to file
                    SaveAssetBundle(bundle);

                    // Set upload status
                    bundle.mStatus = "Uploading..";
                    
                    // Remove from Untracked Bundles Section
                    mUntrackedAssetBundleHolder.Remove(bundle.mVisualElement);

                    // Add uploaded bundle to dictionary
                    mUploadedAssetBundles.Add(bundle.mHashId, bundle);

                    // Add to Uploaded Bundles Section
                    AddUploadedAssetBundleForDisplay(bundle);
                    
                    // Save the Uploaded Timestamp
                    EditorUtility.SetDirty(bundle);
                    AssetDatabase.SaveAssets();
                }
            }

            // remove mUntrackedAssetBundles
            foreach (var bundle in list)
            {
                mUntrackedAssetBundles.Remove(bundle.mName);
            }

            Debug.Log("Untracked Bundles List Size: " + mUntrackedAssetBundles.Count);

            return list;
        }

        private void SaveAssetBundle(ABData bundle)
        {
            string storageFile = String.Format(
                    "{0}/{1}/{2}-{3}.asset",
                    AssetBundleMenu.RESOURCES_FOLDER,
                    ASSET_BUNDLES_UPLOADED_DIRECTORY, 
                    bundle.mName, 
                    bundle.mHash);

            System.IO.FileInfo fileInfo = new System.IO.FileInfo(storageFile);
            if (!fileInfo.Exists)
            {
                AssetDatabase.CreateAsset(bundle, storageFile);
            }
        }

        private IEnumerator UpdateEstimatedCost(ABData bundle, bool isSelected)
        {
            // Todo: Implement this when you have the Estimate Cost API
            mEstimatedTotalCost += (isSelected) ? 10.0f : -10.0f;
            mEstimatedTotalCostLabel.text = String.Format("{0} AR", mEstimatedTotalCost.ToString("n2"));
            yield return null;
        }
    }
}