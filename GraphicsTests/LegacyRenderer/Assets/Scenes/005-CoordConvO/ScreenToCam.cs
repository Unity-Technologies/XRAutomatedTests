using UnityEngine;

public class ScreenToCam : MonoBehaviour
{
    public Vector3 pos;

    public void Update()
    {
        var cam = Camera.main;
        transform.position = cam.ScreenToWorldPoint(pos);
    }

    public void OnGUI()
    {
        GUI.Label(new Rect(pos.x, Screen.height - pos.y, 200, 50), (("Scrn: " + pos) + "\nWrld: ") + transform.position);
    }
}
