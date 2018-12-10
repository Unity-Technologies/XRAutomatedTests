/************************************************************************************

Filename    :   StartupSample.cs
Content     :   An example of how to set up your game for fast loading with a
			:	black splash screen, and a small logo screen that triggers an
			:	async main scene load
Created     :   June 27, 2014
Authors     :   Andrew Welch

Copyright   :   Copyright 2014 Oculus VR, LLC. All Rights reserved.


************************************************************************************/

using UnityEngine;
using System.Collections;				// required for Coroutines

public class StartupSample : MonoBehaviour
{
	public float			delayBeforeLoad = 0.0f;
	public string			sceneToLoad = string.Empty;
	
	/// <summary>
	/// Start a delayed scene load
	/// </summary>
	void Start()
	{
		// start the main scene load
		StartCoroutine(DelayedSceneLoad());
	}
	
	/// <summary>
	/// Asynchronously start the main scene load
	/// </summary>
	IEnumerator DelayedSceneLoad()
	{
		// delay one frame to make sure everything has initialized
		yield return 0;
		
		// this is *ONLY* here for example as our 'main scene' will load too fast
		// remove this for production builds or set the time to 0.0f
		yield return new WaitForSeconds(delayBeforeLoad);

		Debug.Log( "[LoadLevel] " + sceneToLoad + " Time: " + Time.time );

		float startTime = Time.realtimeSinceStartup;
#if !UNITY_PRO_LICENSE
		Application.LoadLevel(sceneToLoad);
#else
		//*************************
		// load the scene asynchronously.
		// this will allow the player to 
		// continue looking around in your loading screen
		//*************************
		AsyncOperation async = Application.LoadLevelAsync(sceneToLoad);
		yield return async;
#endif
		Debug.Log( "[SceneLoad] Completed: " + (Time.realtimeSinceStartup - startTime).ToString("F2") + " sec(s)");
	}
	
}
