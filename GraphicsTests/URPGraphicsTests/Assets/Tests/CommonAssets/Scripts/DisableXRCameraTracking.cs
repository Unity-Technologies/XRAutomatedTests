using UnityEngine;
using UnityEngine.SceneManagement;

public class DisableXRCameraTracking : MonoBehaviour
{
    public void Awake()
    {
        SceneManager.sceneLoaded += DisableTracking;
    }

    public void DisableTracking(Scene loadedScene, LoadSceneMode mode)
    {
        foreach(var cam in FindObjectsOfType<Camera>())
        {
            UnityEngine.XR.XRDevice.DisableAutoXRCameraTracking(cam, true);
        }
    }
}
