using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

using UnityEditor;
using EditorSceneManagement = UnityEditor.SceneManagement;

namespace UnityEditor.TestTools.Graphics
{
    /// <summary>
    /// Test framework prebuild step to collect reference images for the current test run and prepare them for use in the
    /// player.
    /// Will also build Lightmaps for specially labelled scenes.
    /// </summary>
    public class SetupGraphicsTestCases : IPrebuildSetup
    {
        static string bakeLabel = "TestRunnerBake";

        private static bool IsBuildingForEditorPlaymode
        {
            get
            {
                var playmodeLauncher =
                    typeof(UnityEditor.TestTools.RequirePlatformSupportAttribute).Assembly.GetType(
                        "UnityEditor.TestTools.TestRunner.PlaymodeLauncher");
                var isRunningField = playmodeLauncher.GetField("IsRunning");

                return (bool)isRunningField.GetValue(null);
            }
        }

        public void Setup()
        {
            ColorSpace colorSpace;
            BuildTarget buildPlatform;
            RuntimePlatform runtimePlatform;
            GraphicsDeviceType[] graphicsDevices;
            StereoRenderingPath stereoPath;
            string[] vrSDK;
            

            // Figure out if we're preparing to run in Editor playmode, or if we're building to run outside the Editor
            if (IsBuildingForEditorPlaymode)
            {
                colorSpace = QualitySettings.activeColorSpace;
                buildPlatform = BuildTarget.NoTarget;
                runtimePlatform = Application.platform;
                graphicsDevices = new[] {SystemInfo.graphicsDeviceType};
                vrSDK = new string[] { "None" };
            }
            else
            {
                buildPlatform = EditorUserBuildSettings.activeBuildTarget;
                runtimePlatform = Utils.BuildTargetToRuntimePlatform(buildPlatform);
                colorSpace = PlayerSettings.colorSpace;
                graphicsDevices = PlayerSettings.GetGraphicsAPIs(buildPlatform);
                vrSDK = PlayerSettings.GetVirtualRealitySDKs(BuildPipeline.GetBuildTargetGroup(buildPlatform));
            }

            stereoPath = PlayerSettings.stereoRenderingPath;

            var bundleBuilds = new List<AssetBundleBuild>();

            foreach (var api in graphicsDevices)
            {
                var images = EditorGraphicsTestCaseProvider.CollectReferenceImagePathsFor(
                    colorSpace, runtimePlatform, api, vrSDK.FirstOrDefault());

                Utils.SetupReferenceImageImportSettings(images.Values);

                if (buildPlatform == BuildTarget.NoTarget)
                    continue;

                bundleBuilds.Add(new AssetBundleBuild
                {
                    assetBundleName = string.Format("referenceimages-{0}-{1}-{2}-{3}", 
                        colorSpace, runtimePlatform, api, vrSDK.FirstOrDefault()).ToLower(),
                    addressableNames = images.Keys.ToArray(),
                    assetNames = images.Values.ToArray()
                });
            }

            if (bundleBuilds.Count > 0)
            {
                if (!Directory.Exists("Assets/StreamingAssets"))
                    Directory.CreateDirectory("Assets/StreamingAssets");

                foreach (var bundle in bundleBuilds)
                {
                    BuildPipeline.BuildAssetBundles("Assets/StreamingAssets", new [] { bundle }, BuildAssetBundleOptions.None,
                        buildPlatform);
                }
            }
            
            // For each scene in the build settings, force build of the lightmaps if it has "DoLightmap" label.
            // Note that in the PreBuildSetup stage, TestRunner has already created a new scene with its testing monobehaviours

            Scene trScene = EditorSceneManagement.EditorSceneManager.GetSceneAt(0);

            EditorBuildSettingsScene[] scenesWithDisabledScenes = EditorBuildSettings.scenes;

            foreach ( EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path);
                
                EditorSceneManagement.EditorSceneManager.OpenScene(scene.path, EditorSceneManagement.OpenSceneMode.Additive);
                Scene currentScene = EditorSceneManagement.EditorSceneManager.GetSceneAt(1);
                EditorSceneManagement.EditorSceneManager.SetActiveScene(currentScene);

                var settings = GameObject.FindObjectOfType<PlatformConfigTestFilters>();

                if (settings != null)
                {
                    var configs = settings.GetComponent<PlatformConfigTestFilters>();

                    if (configs != null)
                    {
                        foreach (var filter in configs.Filters)
                        {
                            if ((filter.BuildPlatform == buildPlatform || filter.BuildPlatform == BuildTarget.NoTarget) &&
                                (filter.GraphicsDevice == graphicsDevices.First() || filter.GraphicsDevice == GraphicsDeviceType.Null) &&
                                (filter.ColorSpace == colorSpace || filter.ColorSpace == ColorSpace.Uninitialized) &&
                                (filter.stereoModes == null || filter.stereoModes.Length == 0|| filter.stereoModes.Contains(stereoPath)))
                            {
                                scenesWithDisabledScenes.First(s => s.path.Contains(currentScene.name)).enabled = false;
                                Debug.Log(string.Format("Removed scene {0} from build settings because {1}", currentScene.name, filter.Reason));
                            }
                        }
                    }
                }

                var labels = new System.Collections.Generic.List<string>(AssetDatabase.GetLabels(sceneAsset));
                if ( labels.Contains(bakeLabel) )
                {   
                    Lightmapping.giWorkflowMode = Lightmapping.GIWorkflowMode.OnDemand;

                    Lightmapping.Bake();

                    // disk cache needs to be cleared to prevent bug 742012 where duplicate lights are double baked
                    Lightmapping.ClearDiskCache();

                    EditorSceneManagement.EditorSceneManager.SaveScene( currentScene );
                }

                EditorSceneManagement.EditorSceneManager.SetActiveScene(trScene);
                EditorSceneManagement.EditorSceneManager.CloseScene(currentScene, true);
            }

            EditorBuildSettings.scenes = scenesWithDisabledScenes;

            if (!IsBuildingForEditorPlaymode)
                new CreateSceneListFileFromBuildSettings().Setup();
        }

        static string lightmapDataGitIgnore = @"Lightmap-*_comp*
LightingData.*
ReflectionProbe-*";

        [MenuItem("Assets/Tests/Toggle Scene for Bake")]
        public static void LabelSceneForBake()
        {
            UnityEngine.Object[] sceneAssets = Selection.GetFiltered(typeof(SceneAsset), SelectionMode.DeepAssets);

            EditorSceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManagement.SceneSetup[] previousSceneSetup = EditorSceneManagement.EditorSceneManager.GetSceneManagerSetup();

            foreach (UnityEngine.Object sceneAsset in sceneAssets)
            {
                List<string> labels = new System.Collections.Generic.List<string>(AssetDatabase.GetLabels(sceneAsset));

                string scenePath = AssetDatabase.GetAssetPath(sceneAsset);
                string gitIgnorePath = Path.Combine( Path.Combine( Application.dataPath.Substring(0, Application.dataPath.Length-6), scenePath.Substring(0, scenePath.Length-6) ) , ".gitignore" );

                if (labels.Contains(bakeLabel))
                {
                    labels.Remove(bakeLabel);
                    File.Delete(gitIgnorePath);
                }
                else
                {
                    labels.Add(bakeLabel);

                    string sceneLightingDataFolder = Path.Combine( Path.GetDirectoryName(scenePath), Path.GetFileNameWithoutExtension(scenePath) );
                    if ( !AssetDatabase.IsValidFolder(sceneLightingDataFolder) )
                        AssetDatabase.CreateFolder( Path.GetDirectoryName(scenePath), Path.GetFileNameWithoutExtension(scenePath) );

                    File.WriteAllText(gitIgnorePath, lightmapDataGitIgnore);

                    EditorSceneManagement.EditorSceneManager.OpenScene(scenePath, EditorSceneManagement.OpenSceneMode.Single);
                    EditorSceneManagement.EditorSceneManager.SetActiveScene( EditorSceneManagement.EditorSceneManager.GetSceneAt(0) );
                    Lightmapping.giWorkflowMode = Lightmapping.GIWorkflowMode.OnDemand;
                    EditorSceneManagement.EditorSceneManager.SaveScene( EditorSceneManagement.EditorSceneManager.GetSceneAt(0) );
                }

                AssetDatabase.SetLabels( sceneAsset, labels.ToArray() );
            }
            AssetDatabase.Refresh();

            if (previousSceneSetup.Length == 0)
                EditorSceneManagement.EditorSceneManager.NewScene(EditorSceneManagement.NewSceneSetup.DefaultGameObjects, EditorSceneManagement.NewSceneMode.Single);
            else
                EditorSceneManagement.EditorSceneManager.RestoreSceneManagerSetup(previousSceneSetup);
        }

        [MenuItem("Assets/Tests/Toggle Scene for Bake", true)]
        public static bool LabelSceneForBake_Test()
        {
            return IsSceneAssetSelected();
        }

        public static bool IsSceneAssetSelected()
        {
            UnityEngine.Object[] sceneAssets = Selection.GetFiltered(typeof(SceneAsset), SelectionMode.DeepAssets);

            return sceneAssets.Length != 0;
        }
    }
}
