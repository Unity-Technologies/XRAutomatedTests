using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR;
#else
using UnityEngine.VR;
#endif

public class Performance : MonoBehaviour {

    public UnityEngine.UI.Text m_Text;
    public float m_DeltaTime;
    public int m_FrameCount = 0;
    public float m_TotalGPUFrameTime = 0.0f;
    public float m_RefreshFrequency = 0.5f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (m_Text != null)
        {
            float gpuLastFrameTime = 0f;
            int droppedFrameCount = 0;
            int framePresentCount = 0;

            m_DeltaTime += (Time.unscaledDeltaTime);
            m_FrameCount++;

            string output = "";
#if UNITY_2017_2_OR_NEWER


            if(XRStats.TryGetGPUTimeLastFrame(out gpuLastFrameTime))
            {
                m_TotalGPUFrameTime += gpuLastFrameTime;
            }
            XRStats.TryGetDroppedFrameCount(out droppedFrameCount);
            XRStats.TryGetFramePresentCount(out framePresentCount);

#else
            using UnityEngine.VR;
#endif

            if(m_DeltaTime > m_RefreshFrequency)
            {
                float fps = (float)m_FrameCount/m_DeltaTime;
                m_TotalGPUFrameTime /= (float)m_FrameCount;

                m_DeltaTime -= m_RefreshFrequency;
                output += "fps: " + fps.ToString("F1");
                output += "\ngpuTimeLastFrame: " + m_TotalGPUFrameTime.ToString("F1");
                output += "\ndroppedFrameCount: " + droppedFrameCount.ToString();
                output += "\nframePresentCount: " + framePresentCount.ToString();

                m_Text.text = output;

                m_DeltaTime -= m_RefreshFrequency;
                m_TotalGPUFrameTime = 0.0f;
                m_FrameCount = 0;
            }
            
            
        }

    }
}
