using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scaler : MonoBehaviour {

    public float m_ScaleDelta = 0.25f;
    public float m_Rate = 1.0f;
    public Vector3 m_AxisScale = Vector3.zero;

    private Vector3 m_PoseScale;

	// Use this for initialization
	void Start () {
        m_PoseScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {

        float scale = Mathf.Sin(Time.time * m_Rate) * 0.5f + 0.5f;

        scale *= m_ScaleDelta;

        transform.localScale = m_PoseScale + Vector3.Scale(new Vector3(scale, scale, scale), m_AxisScale);

	}
}
