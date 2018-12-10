using UnityEngine;
using System.Collections;

public class PerformanceGUI : MonoBehaviour 
{
    private float   LastTime = -1.0f;               // time when last frame interval started
    private int     NumAccumulatedFrames = 0;       // current number of accumulated frames
    private int     FramesPerInterval = 30;         // number of frames over which to aveerage frame rate
    private float   AverageFrameTime = 0.0f;        // average frame rate over last frame

    public void Update() {
        // make sure LastTime is initialized to the current time if it's not already
        if ( LastTime < 0.0f ) 
        {
            LastTime = Time.time;
            NumAccumulatedFrames = 0;
        }
        NumAccumulatedFrames++;
        if ( NumAccumulatedFrames >= FramesPerInterval )
        {
            float totalTime = Time.time - LastTime;
            AverageFrameTime = totalTime / NumAccumulatedFrames;
            LastTime = Time.time;
            NumAccumulatedFrames = 0;
        }
    }

    public void OnGUI()
    {
        if ( AverageFrameTime <= 0.0001f )
        {
            // don't show an infinity, thanks
            return;
        }
        int w = Screen.width;
        int h = Screen.height;

        GUIStyle style = new GUIStyle();

        const float HEIGHT_SCALE = 0.05f;
        float REMAINDER_HEIGHT = h * (0.5f - HEIGHT_SCALE);
        Rect rect = new Rect( 0, REMAINDER_HEIGHT, w, h * HEIGHT_SCALE );
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = (int)( h * HEIGHT_SCALE );
        style.normal.textColor = new Color( 1.0f, 1.0f, 1.0f, 1.0f );
        float averageMilliseconds = AverageFrameTime * 1000.0f;
        float averageFPS = 1.0f / AverageFrameTime;
        string text = string.Format( "{0,5:F} ms : {1,5:F} fps", averageMilliseconds, averageFPS );
        //print( text );
        GUI.Label( rect, text, style );
    }
};