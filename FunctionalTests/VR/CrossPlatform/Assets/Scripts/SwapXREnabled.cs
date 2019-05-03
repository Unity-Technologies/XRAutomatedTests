using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;
using UnityEngine.XR;

public class SwapXrEnabled : MonoBehaviour, IMonoBehaviourTest {

	public bool IsTestFinished => isTestFinished;
	
	private const string None = "";

	private bool isTestFinished;

	private CurrentSettings settings;
	
	
	// Use this for initialization
	void Start ()
	{
		settings = Resources.Load<CurrentSettings>("settings");
		StartCoroutine(DisableXr());
	}
	

	IEnumerator DisableXr()
	{
		Assert.IsTrue(XRSettings.loadedDeviceName == settings.EnabledXrTarget);
		Assert.IsTrue(XRSettings.enabled);
		
		XRSettings.LoadDeviceByName(None);

		yield return null;
		
		Debug.Log($"Loaded Device = {XRSettings.loadedDeviceName}");
		Debug.Log($"XR Enabled = {XRSettings.enabled}");

		Assert.IsTrue(XRSettings.loadedDeviceName == string.Empty, $"Loaded Device = {XRSettings.loadedDeviceName}");
		Assert.IsFalse(XRSettings.enabled);
		
		StartCoroutine(EnableXr());
	}

	IEnumerator EnableXr()
	{
		Assert.IsTrue(XRSettings.loadedDeviceName == string.Empty, $"Loaded Device = {XRSettings.loadedDeviceName}");
		Assert.IsFalse(XRSettings.enabled);
		
		XRSettings.LoadDeviceByName(settings.EnabledXrTarget);
		
		yield return null;

		XRSettings.enabled = true;
		
		yield return null;
		
		Debug.Log($"Loaded Device = {XRSettings.loadedDeviceName}");
		Debug.Log($"XR Enabled = {XRSettings.enabled}");
		
		Assert.IsTrue(XRSettings.loadedDeviceName == settings.EnabledXrTarget, $"Loaded Device = {XRSettings.loadedDeviceName}");
		Assert.IsTrue(XRSettings.enabled);

	    isTestFinished = true;
	}
}
