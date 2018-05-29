using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Translate : MonoBehaviour {

    public float m_Rate;
    public float m_MaxTranslation;
    public Vector3 m_Axis;
    
    private Vector3 m_PosePosition;
    public float m_AnimationTimeOffset;
    public bool m_Randomize;

	// Use this for initialization
	void Start () {
        m_PosePosition = transform.position;
        if(m_Randomize)
        {
            m_AnimationTimeOffset = Random.Range(0.0f, 360.0f);
        }
	}
	
	// Update is called once per frame
	void Update () {
        float offset = Mathf.Sin( (Time.time + m_AnimationTimeOffset) * m_Rate) * 0.5f + 0.5f;

        transform.position = ((m_Axis.normalized) * m_MaxTranslation * offset) + m_PosePosition;
	}
}
