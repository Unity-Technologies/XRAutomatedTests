//-----------------------------------------------------------------------
// <copyright file="CloudAnchorPreprocessBuild.cs" company="Google">
//
// Copyright 2018 Google LLC. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace GoogleARCoreInternal
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Text;
    using System.Xml;
    using UnityEditor;
    using UnityEditor.Build;
    using UnityEngine;

    internal class CloudAnchorPreprocessBuild : PreprocessBuildBase
    {
        private const string k_ManifestTemplateGuid = "5e182918f0b8c4929a3d4b0af0ed6f56";
        private const string k_PluginsFolderGuid = "93be2b9777c348648a2d9151b7e233fc";
        private const string k_RuntimeSettingsPath = "GoogleARCore/Resources/RuntimeSettings";

        public override void OnPreprocessBuild(BuildTarget target, string path)
        {
            if (target == BuildTarget.Android)
            {
                _PreprocessAndroidBuild();
            }
            else if (target == BuildTarget.iOS)
            {
                _PreprocessIosBuild();
            }
        }

        private string _getJdkPath()
        {
            string jdkPath = null;

// Unity started offering the embedded JDK in 2018.3
#if UNITY_2018_3_OR_NEWER
            if (UnityEditor.EditorPrefs.GetBool("JdkUseEmbedded"))
            {
                // Use OpenJDK that is bundled with Unity. JAVA_HOME will be set when
                // 'Preferences > External Tools > Android > JDK installed with Unity' is checked.
                jdkPath = System.Environment.GetEnvironmentVariable("JAVA_HOME");
                if (string.IsNullOrEmpty(jdkPath))
                {
                    throw new BuildFailedException(
                        "'Preferences > External Tools > Android > JDK installed with Unity' is " +
                        "checked, but JAVA_HOME is unset or empty. Try unchecking this setting " +
                        "and configuring a valid JDK path under " +
                        "'Preferences > External Tools > Android > JDK'.");
                }
            }
            else
#endif // UNITY_2018_3_OR_NEWER
            {
                // Use JDK path specified by 'Preferences > External Tools > Android > JDK'.
                jdkPath = EditorPrefs.GetString("JdkPath");
                if (string.IsNullOrEmpty(jdkPath))
                {
                    // Use JAVA_HOME from the O/S environment.
                    jdkPath = System.Environment.GetEnvironmentVariable("JAVA_HOME");
                    if (string.IsNullOrEmpty(jdkPath))
                    {
                        throw new BuildFailedException(
                            "'Preferences > External Tools > Android > JDK installed with Unity' " +
                            "is unchecked, but 'Preferences > External Tools > Android > JDK' " +
                            "path is empty and JAVA_HOME environment variable is unset or empty.");
                    }
                }
            }

            if ((File.GetAttributes(jdkPath) & FileAttributes.Directory) == 0)
            {
                throw new BuildFailedException(string.Format("Invalid JDK path '{0}'", jdkPath));
            }

            return jdkPath;
        }

        private void _PreprocessAndroidBuild()
        {
            string cachedCurrentDirectory = Directory.GetCurrentDirectory();
            string pluginsFolderPath = Path.Combine(cachedCurrentDirectory,
                AssetDatabase.GUIDToAssetPath(k_PluginsFolderGuid));
            string cloudAnchorsManifestAarPath =
                Path.Combine(pluginsFolderPath, "cloud_anchor_manifest.aar");

            bool cloudAnchorsEnabled =
                !string.IsNullOrEmpty(ARCoreProjectSettings.Instance.CloudServicesApiKey);
            if (cloudAnchorsEnabled)
            {
                string jarPath = Path.Combine(_getJdkPath(), "bin/jar");

                // If the API Key didn't change then do nothing.
                if (!_IsApiKeyDirty(jarPath, cloudAnchorsManifestAarPath,
                  ARCoreProjectSettings.Instance.CloudServicesApiKey))
                {
                    return;
                }

                // Replace the project's cloud anchor AAR with the newly generated AAR.
                Debug.Log("Enabling Cloud Anchors with API Key Authentication in this build.");

                var tempDirectoryPath =
                    Path.Combine(cachedCurrentDirectory, FileUtil.GetUniqueTempPathInProject());

                try
                {
                    // Move to a temp directory.
                    Directory.CreateDirectory(tempDirectoryPath);
                    Directory.SetCurrentDirectory(tempDirectoryPath);

                    var manifestTemplatePath = Path.Combine(
                        cachedCurrentDirectory,
                        AssetDatabase.GUIDToAssetPath(k_ManifestTemplateGuid));

                    // Extract the "template AAR" and remove it.
                    string output;
                    string errors;
                    ShellHelper.RunCommand(
                        jarPath, string.Format("xf \"{0}\"", manifestTemplatePath), out output,
                        out errors);

                    // Replace Api key template parameter in manifest file.
                    var manifestPath = Path.Combine(tempDirectoryPath, "AndroidManifest.xml");
                    var manifestText = File.ReadAllText(manifestPath);
                    manifestText = manifestText.Replace(
                        "{{CLOUD_ANCHOR_API_KEY}}",
                        ARCoreProjectSettings.Instance.CloudServicesApiKey);
                    File.WriteAllText(manifestPath, manifestText);

                    // Compress the new AAR.
                    var fileListBuilder = new StringBuilder();
                    foreach (var filePath in Directory.GetFiles(tempDirectoryPath))
                    {
                        fileListBuilder.AppendFormat(" {0}", Path.GetFileName(filePath));
                    }

                    string command = string.Format(
                        "cf cloud_anchor_manifest.aar {0}", fileListBuilder.ToString());

                    ShellHelper.RunCommand(
                        jarPath,
                        command,
                        out output,
                        out errors);

                    if (!string.IsNullOrEmpty(errors))
                    {
                        throw new BuildFailedException(
                            string.Format(
                                "Error creating jar for cloud anchor manifest: {0}", errors));
                    }

                    File.Copy(Path.Combine(tempDirectoryPath, "cloud_anchor_manifest.aar"),
                        cloudAnchorsManifestAarPath, true);
                }
                finally
                {
                    // Cleanup.
                    Directory.SetCurrentDirectory(cachedCurrentDirectory);
                    Directory.Delete(tempDirectoryPath, true);

                    AssetDatabase.Refresh();
                }

                AssetHelper.GetPluginImporterByName("cloud_anchor_manifest.aar")
                    .SetCompatibleWithPlatform(BuildTarget.Android, true);
            }
            else
            {
                Debug.Log(
                    "Cloud Anchor API key has not been set in this build.");
                File.Delete(cloudAnchorsManifestAarPath);
                AssetDatabase.Refresh();
            }
        }

        private bool _IsApiKeyDirty(string jarPath, string aarPath, string apiKey)
        {
            bool isApiKeyDirty = true;
            var cachedCurrentDirectory = Directory.GetCurrentDirectory();
            var tempDirectoryPath =
                Path.Combine(cachedCurrentDirectory, FileUtil.GetUniqueTempPathInProject());

            if (!File.Exists(aarPath))
            {
                return isApiKeyDirty;
            }

            try
            {
                // Move to a temp directory.
                Directory.CreateDirectory(tempDirectoryPath);
                Directory.SetCurrentDirectory(tempDirectoryPath);
                var tempAarPath = Path.Combine(tempDirectoryPath, "cloud_anchor_manifest.aar");
                File.Copy(aarPath, tempAarPath, true);

                // Extract the aar.
                string output;
                string errors;
                ShellHelper.RunCommand(jarPath, string.Format("xf \"{0}\"", tempAarPath),
                    out output, out errors);

                // Read Api key parameter in manifest file.
                var manifestPath = Path.Combine(tempDirectoryPath, "AndroidManifest.xml");
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(manifestPath);
                XmlNode metaDataNode =
                    xmlDocument.SelectSingleNode("/manifest/application/meta-data");
                string oldApiKey = metaDataNode.Attributes["android:value"].Value;
                isApiKeyDirty = !apiKey.Equals(oldApiKey);
            }
            finally
            {
                // Cleanup.
                Directory.SetCurrentDirectory(cachedCurrentDirectory);
                Directory.Delete(tempDirectoryPath, true);
            }

            return isApiKeyDirty;
        }

        private void _PreprocessIosBuild()
        {
            var runtimeSettingsPath = Path.Combine(Application.dataPath, k_RuntimeSettingsPath);
            Directory.CreateDirectory(runtimeSettingsPath);
            string cloudServicesApiKey = ARCoreProjectSettings.Instance.IosCloudServicesApiKey;
            File.WriteAllText(
                Path.Combine(runtimeSettingsPath, "CloudServicesApiKey.txt"), cloudServicesApiKey);
            AssetDatabase.Refresh();
        }
    }
}
