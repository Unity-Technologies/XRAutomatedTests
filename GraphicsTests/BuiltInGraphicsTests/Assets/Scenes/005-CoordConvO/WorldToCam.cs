using UnityEngine;

public class WorldToCam : MonoBehaviour
{
    public void OnGUI()
    {
        var cam = Camera.main;
        var screenPt = cam.WorldToScreenPoint(transform.position);
        var viewPt = cam.WorldToViewportPoint(transform.position);
        GUI.Label(new Rect(screenPt.x, Screen.height - screenPt.y, 200, 50), (((("Wrld: " + transform.position) + "\nScrn: ") + screenPt) + "\nView: ") + viewPt);
    }
}
