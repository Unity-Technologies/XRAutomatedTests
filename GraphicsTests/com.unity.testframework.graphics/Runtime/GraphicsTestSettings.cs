namespace UnityEngine.TestTools.Graphics
{
    public class GraphicsTestSettings : MonoBehaviour
    {
        public ImageComparisonSettings ImageComparisonSettings = new ImageComparisonSettings();

        void Awake()
        {
            foreach (var cam in GameObject.FindObjectsOfType<Camera>())
                XR.XRDevice.DisableAutoXRCameraTracking(cam, true);
        }
    }
}
