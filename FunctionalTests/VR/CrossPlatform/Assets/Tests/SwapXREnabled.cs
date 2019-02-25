using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;
using UnityEngine.XR;

public class SwapXREnabled : MonoBehaviour, IMonoBehaviourTest {

	public bool IsTestFinished => _isTestFinished;
	
	private const string NONE = "";

	private bool _isTestFinished = false;

	private CurrentSettings settings;
	
	
	// Use this for initialization
	void Start ()
	{
		settings = Resources.Load<CurrentSettings>("settings");
		StartCoroutine(DisableXR());
	}
	

	IEnumerator DisableXR()
	{
		Assert.IsTrue(XRSettings.loadedDeviceName == settings.enabledXrTarget);
		Assert.IsTrue(XRSettings.enabled);
		
		XRSettings.LoadDeviceByName(NONE);

		yield return null;
		
		Debug.Log($"Loaded Device = {XRSettings.loadedDeviceName}");
		Debug.Log($"XR Enabled = {XRSettings.enabled}");

		Assert.IsTrue(XRSettings.loadedDeviceName == string.Empty, $"Loaded Device = {XRSettings.loadedDeviceName}");
		Assert.IsFalse(XRSettings.enabled);
		
		StartCoroutine(EnableXR());
	}

	IEnumerator EnableXR()
	{
		Assert.IsTrue(XRSettings.loadedDeviceName == string.Empty, $"Loaded Device = {XRSettings.loadedDeviceName}");
		Assert.IsFalse(XRSettings.enabled);
		
		XRSettings.LoadDeviceByName(settings.enabledXrTarget);
		
		yield return null;

		XRSettings.enabled = true;
		
		yield return null;
		
		Debug.Log($"Loaded Device = {XRSettings.loadedDeviceName}");
		Debug.Log($"XR Enabled = {XRSettings.enabled}");
		
		Assert.IsTrue(XRSettings.loadedDeviceName == settings.enabledXrTarget, $"Loaded Device = {XRSettings.loadedDeviceName}");
		Assert.IsTrue(XRSettings.enabled);

		_isTestFinished = true;
	}
}
