using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace Rawrshak
{
    public abstract class Static3dObjectAssetBase : AssetBase
    {
        public enum RenderPipeline {
            Invalid,
            BuiltInRenderPipeline,
            UniversalRenderPipeline,
            HighDefinitionRenderPipeline
        }

        public enum Platform {
            Invalid,
            Android,
            iOS,
            StandaloneWindows,
            WebGL
        }

        public enum Fidelity {
            Invalid,
            Low,
            Medium,
            High
        }

        public enum Shape {
            Invalid,
            SmallCube,
            MediumCube,
            LargeCube,
            HorizontalX,
            HorizontalY,
            HorizontalZ
        }

        private static string Engine = "unity";

        protected Static3dObjectMetadataBase metadata;
        protected List<PrefabProperties> prefabData;
        protected Fidelity currentFidelity;
        protected Platform currentPlatform;
        protected Shape currentShape;
        protected RenderPipeline currentRenderPipeline;
        protected GameObject currentPrefab;

        public override void Init(PublicAssetMetadataBase baseMetadata)
        {
            // Note: We make the assumption that the Rendering Pipline is set to Built-in Render Pipeline
            if (GraphicsSettings.renderPipelineAsset)
            {
                Debug.LogError("Only the built-in render pipeline is supported.");
                return;
            }

            metadata = Static3dObjectMetadataBase.Parse(baseMetadata.jsonString);
            metadata.jsonString = baseMetadata.jsonString;
            
            prefabData = new List<PrefabProperties>();
            foreach (var prefabProperties in metadata.assetProperties)
            {
                // Filter out non-unity engine assets and unsupported assets
                if (prefabProperties.engine == Engine)
                {
                    // Note: Overwrite duplicates. Does not throw an exception
                    prefabData.Add(prefabProperties);
                }
            }

            currentFidelity = Fidelity.Invalid;
            currentPlatform = Platform.Invalid;
            currentShape = Shape.Invalid;
            currentRenderPipeline = RenderPipeline.Invalid;
        }

        public async Task<GameObject> LoadAndSetPrefab(
            Platform platform,
            RenderPipeline renderPipeline,
            Fidelity fidelity
        )
        {
            if (currentPlatform == platform && currentRenderPipeline == renderPipeline && currentFidelity == fidelity)
            {
                return currentPrefab;
            }

            PrefabProperties  prefabProperty = null;
            foreach (PrefabProperties prefab in prefabData)
            {
                if (platform == ParsePlatform(prefab.platform) &&
                    renderPipeline == ParseRenderPipeline(prefab.renderPipeline) &&
                    fidelity == ParseFidelity(prefab.fidelity))
                {
                    prefabProperty = prefab;
                    break;
                }
            }

            if (prefabProperty == null)
            {
                Debug.LogError("No prefab found.");
                return null;
            }

            if (String.IsNullOrEmpty(prefabProperty.uri))
            {
                Debug.LogError("Prefab metadata uri is not found");
                return null;
            }
            
            // Download the assetbundle
            AssetBundle assetBundle = await Downloader.DownloadAssetBundle(prefabProperty.uri);
            if (assetBundle == null)
            {
                Debug.LogError("AssetBundle not found");
                return null;
            }
            
            GameObject prefabGameObj = assetBundle.LoadAsset<GameObject>(prefabProperty.name);
            if (prefabGameObj == null)
            {
                Debug.LogError("Prefab doesn't exist in AssetBundle");
                assetBundle.Unload(true);
                return null;
            }

            // Todo: Verify if the 3d object exists within the bounds of the default asset for the 
            //       necessary shape.

            // Set new current loaded information
            currentFidelity = fidelity;
            currentPlatform = platform;
            currentRenderPipeline = renderPipeline;
            currentShape = ParseShape(prefabProperty.shape);
            currentPrefab = prefabGameObj;

            assetBundle.Unload(false);

            return currentPrefab;
        }

        public Fidelity GetCurrentFidelity()
        {
            return currentFidelity;
        }
        
        public GameObject GetCurrentPrefab()
        {
            return currentPrefab;
        }

        public Shape GetCurrentShape()
        {
            return currentShape;
        }

        public RenderPipeline GetCurrentRenderPipeline()
        {
            return currentRenderPipeline;
        }

        public List<Platform> GetSupportedPlatforms()
        {
            // HashSet deletes duplicates
            HashSet<Platform> platforms = new HashSet<Platform>();
            foreach (var data in prefabData)
            {
                platforms.Add(ParsePlatform(data.platform));
            }
            return platforms.ToList();
        }

        public List<RenderPipeline> GetSupportedRenderPipelines()
        {
            // HashSet deletes duplicates
            HashSet<RenderPipeline> renderPipelines = new HashSet<RenderPipeline>();
            foreach (var data in prefabData)
            {
                renderPipelines.Add(ParseRenderPipeline(data.renderPipeline));
            }
            return renderPipelines.ToList();
        }

        public List<Fidelity> GetSupportedFidelities()
        {
            // HashSet deletes duplicates
            HashSet<Fidelity> fidelities = new HashSet<Fidelity>();
            foreach (var data in prefabData)
            {
                fidelities.Add(ParseFidelity(data.fidelity));
            }
            return fidelities.ToList();
        }
        
        /***** Helper Functions *****/
        private Fidelity ParseFidelity(string fidelity)
        {
            switch(fidelity)
            {
                case "low":
                {
                    return Fidelity.Low;
                }
                case "medium":
                {
                    return Fidelity.Medium;
                }
                case "high":
                {
                    return Fidelity.High;
                }
                default:
                {
                    return Fidelity.Invalid;
                }
            }
        }
        
        private Platform ParsePlatform(string platform)
        {
            switch(platform)
            {
                case "android":
                {
                    return Platform.Android;
                }
                case "ios":
                {
                    return Platform.iOS;
                }
                case "windows":
                {
                    return Platform.StandaloneWindows;
                }
                case "webgl":
                {
                    return Platform.WebGL;
                }
                default:
                {
                    return Platform.Invalid;
                }
            }
        }
        
        private RenderPipeline ParseRenderPipeline(string renderPipeline)
        {
            switch(renderPipeline)
            {
                case "brp":
                {
                    return RenderPipeline.BuiltInRenderPipeline;
                }
                case "urp":
                {
                    return RenderPipeline.UniversalRenderPipeline;
                }
                case "hdrp":
                {
                    return RenderPipeline.HighDefinitionRenderPipeline;
                }
                default:
                {
                    return RenderPipeline.Invalid;
                }
            }
        }
        
        private Shape ParseShape(string shape)
        {
            switch(shape)
            {
                case "small":
                {
                    return Shape.SmallCube;
                }
                case "medium":
                {
                    return Shape.MediumCube;
                }
                case "large":
                {
                    return Shape.LargeCube;
                }
                case "horizontalx":
                {
                    return Shape.HorizontalX;
                }
                case "horizontaly":
                {
                    return Shape.HorizontalY;
                }
                case "horizontalz":
                {
                    return Shape.HorizontalZ;
                }
                default:
                {
                    return Shape.Invalid;
                }
            }
        }
    }
}
