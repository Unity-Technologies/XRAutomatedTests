namespace UnityEngine.TestTools.Graphics
{
    public class GraphicsTestSettings : MonoBehaviour
    {
        public ImageComparisonSettings ImageComparisonSettings = new ImageComparisonSettings();

        void Awake()
        {
            XR.XRDevice.DisableAutoXRCameraTracking(Camera.main, true);
        }
    }
}
