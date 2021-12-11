using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.Events;
using Unity.EditorCoroutines.Editor;

namespace Rawrshak
{
    public class ABViewer : ScriptableObject
    {
        ABData mAssetBundle;

        // UI
        Box mHelpBoxHolder;
        Box mViewer;
        Button mVerifyButton;
        
        VisualTreeAsset mBundleTreeAsset;
        UnityEvent<ABData> mCheckUploadStatusCallback = new UnityEvent<ABData>();

        public static ABViewer CreateInstance()
        {
            return ScriptableObject.CreateInstance<ABViewer>();
        }

        public void SetAssetBundle(ABData assetBundle)
        {
            mAssetBundle = assetBundle;

            mViewer.Clear();

            TemplateContainer bundleTree = mBundleTreeAsset.CloneTree();
            
            var bundleView = bundleTree.contentContainer.Query<Box>("info-box").First();
            SerializedObject so = new SerializedObject(mAssetBundle);
            bundleView.Bind(so);

            // Register button click for check status callback
            var checkStatusButton = bundleTree.contentContainer.Query<Button>("check-status").First();
            checkStatusButton.clicked += () => {
                if (String.IsNullOrEmpty(mAssetBundle.mTransactionId))
                {
                    AddErrorHelpbox("Bundle has not been uploaded yet.");
                    return;
                }
                mCheckUploadStatusCallback.Invoke(mAssetBundle);
            };

            // Verify Button
            mVerifyButton = bundleTree.contentContainer.Query<Button>("verify-asset-bundle-button").First();
            if (mAssetBundle.mStatus == "Uploaded")
            {
                mVerifyButton.SetEnabled(true);
                mVerifyButton.clicked += () => {
                    EditorCoroutineUtility.StartCoroutine(Verify(mAssetBundle), this);
                };
            }
            else
            {
                mVerifyButton.SetEnabled(false);
            }

            mViewer.Add(bundleTree);
        }

        public void SetCheckStatusCallback(UnityAction<ABData> checkUploadStatusCallback)
        {
            mCheckUploadStatusCallback.AddListener(checkUploadStatusCallback);
        }
        
        public void LoadUI(VisualElement root)
        {
            // Load View UML
            mBundleTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Rawrshak/Editor/UXML/AssetBundleMenu/AssetBundleView.uxml");

            mHelpBoxHolder = root.Query<Box>("helpbox-holder").First();

            // Asset Bundle Entries
            mViewer = root.Query<Box>("asset-bundle-viewer").First();
        }

        public void AddErrorHelpbox(string errorMsg)
        {
            mHelpBoxHolder.Add(new HelpBox(errorMsg, HelpBoxMessageType.Error));
        }

        private IEnumerator Verify(ABData bundle)
        {
            // Todo: Literally just call https://arweave.net/tx/{id}/status
            // Todo: Verify Status
            // Todo: Verify Confirmations > 10;
            // Todo: Download Uri Asset Bundle
            // Todo: Verify Hash
            yield return null;
        }

        private IEnumerator QueryUploadCost(ABData bundle)
        {
            // Todo: Implement this when you have the Estimate Cost API
            bundle.mUploadCost = 0.0f;
            yield return null;
        }
    }

}