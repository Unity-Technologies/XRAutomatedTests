// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEditor;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Implements functionality for building HoloLens applications
    /// </summary>
    public static class HoloToolkitCommands
    {
        /// <summary>
        /// Do a build configured for the HoloLens, returns the error from BuildPipeline.BuildPlayer
        /// </summary>
        [Obsolete("Use BuildDeployTools.BuildSLN")]
        public static bool BuildSLN()
        {
            return BuildDeployTools.BuildSLN(BuildDeployPrefs.BuildDirectory, false);
        }

        public static bool BuildAPPX(string buildDirectory = "")
        {
            if(buildDirectory == string.Empty)
            {
                buildDirectory = BuildDeployPrefs.BuildDirectory;
            }

            return BuildDeployTools.BuildAppxFromSLN(
                PlayerSettings.productName,
                BuildDeployTools.DefaultMSBuildVersion,
                BuildDeployPrefs.ForceRebuild,
                BuildDeployPrefs.BuildConfig,
                BuildDeployPrefs.BuildPlatform,
                buildDirectory,
                BuildDeployPrefs.IncrementBuildVersion);
        }

        public static void BuildSLNAndAPPX(string path)
        {
            var slnResult = BuildDeployTools.BuildSLN(path, false);

            var appxResult = BuildAPPX(path);
        }

        public static bool InstallAPPX()
        {
            ConnectInfo connectInfo = new ConnectInfo("10.1.18.9", "1234", "12341234", "HoloLens");

            BuildDeployWindow.InstallOnTargetDevice(BuildDeployPrefs.BuildDirectory, connectInfo);

            return BuildDeployWindow.AppxInstallSucces;
        }
    }
}
