/************************************************************************************

Filename    :   BlockSplosionBuild.cs
Content     :   Build scripts for Blocksplosion
Created     :   May 8, 2014
Authors     :   Alex Howland

Copyright   :   Copyright 2014 Oculus VR, LLC. All Rights reserved.

Use of this software is subject to the terms of the Oculus LLC license
agreement provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

************************************************************************************/
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

//-------------------------------------------------------------------------------------
// ***** OculusBuildApp
//
// OculusBuildApp allows us to build other Oculus apps from the command line.
//
partial class OculusBuildSamples
{
	private static GraphicsDeviceType[] testGAPIs = new GraphicsDeviceType[1];

	//[MenuItem ("TestMenu/BuildBlockSplosion")]
	static void BuildBlockSplosion()
	{
		string[] scenes = { "Assets/BlockSplosion/Scenes/Startup_Sample.unity", "Assets/BlockSplosion/Scenes/Main.unity" };

		PlayerSettings.virtualRealitySupported = true;
#if UNITY_ANDROID
		EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
		#if UNITY_5_6_OR_NEWER
		PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.oculus.integ.blocksplosion");
		#else
		PlayerSettings.bundleIdentifier = "com.oculus.integ.blocksplosion";
		#endif
		BuildPipeline.BuildPlayer(scenes, "BlockSplosion.apk", BuildTarget.Android, BuildOptions.None);

		//Graphics Jobs (Experimental)
		PlayerSettings.graphicsJobs = true;
		PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.oculus.integ.blocksplosiongraphicsjobs");
		BuildPipeline.BuildPlayer(scenes, "BlockSplosionGraphicsJobs.apk", BuildTarget.Android, BuildOptions.None);
		PlayerSettings.graphicsJobs = false;

	#if UNITY_5_6_OR_NEWER
		//Multi-view on
		PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.oculus.integ.blocksplosionmultiview");
		PlayerSettings.stereoRenderingPath = StereoRenderingPath.SinglePass;
		BuildPipeline.BuildPlayer(scenes, "BlockSplosionMultiView.apk", BuildTarget.Android, BuildOptions.None);
		PlayerSettings.stereoRenderingPath = StereoRenderingPath.MultiPass;

		//Development Build
		PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.oculus.integ.blocksplosiondevelopment");
		BuildPipeline.BuildPlayer(scenes, "BlockSplosionDevelopment.apk", BuildTarget.Android, BuildOptions.Development);
	#endif
#elif UNITY_STANDALONE_WIN
		BuildPipeline.BuildPlayer(scenes, "./BlockSplosion64/BlockSplosion64.exe", BuildTarget.StandaloneWindows64, BuildOptions.None);
		BuildPipeline.BuildPlayer(scenes, "./BlockSplosion32/BlockSplosion32.exe", BuildTarget.StandaloneWindows, BuildOptions.None);

		//Development Build
		EditorUserBuildSettings.SetPlatformSettings("Standalone", "CopyPDBFiles", "true");
		BuildPipeline.BuildPlayer(scenes, "./BlockSplosion64_Development/BlockSplosion64_Development.exe", BuildTarget.StandaloneWindows64, BuildOptions.Development);

		//Switch to DirectX 12
		testGAPIs [0] = GraphicsDeviceType.Direct3D12;
		PlayerSettings.SetGraphicsAPIs (BuildTarget.StandaloneWindows64, testGAPIs);
		BuildPipeline.BuildPlayer(scenes, "./BlockSplosion64_DX12/BlockSplosion64_DX12.exe", BuildTarget.StandaloneWindows64, BuildOptions.None);
#endif
	}
}
