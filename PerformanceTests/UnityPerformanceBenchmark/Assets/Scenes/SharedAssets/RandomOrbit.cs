using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomOrbit : MonoBehaviour {

    public GameObject m_OrbitTarget;

    public Vector3 m_EulerFactors = Vector3.one;
    public float m_VelocityMagnitude;
    public Vector3 m_OffsetScale = Vector3.one;
    
    public float m_Radius;
    private Vector3 m_Euler;
    private Vector3 m_EulerVelocity;
    private Vector3 m_PosePosition;

    

	// Use this for initialization
	void Start () {
        m_EulerVelocity = Vector3.forward * m_VelocityMagnitude;
        m_PosePosition = transform.position;
        UpdatePosition();
	}
	
    private void UpdatePosition()
    {
        m_EulerVelocity = m_VelocityMagnitude * m_EulerFactors;
        
        m_Euler += m_EulerVelocity * Time.deltaTime;

        Quaternion rot = Quaternion.Euler(m_Euler);
        Vector3 offset = Vector3.Scale((rot * Vector3.forward) * m_Radius, m_OffsetScale);

        if (m_OrbitTarget != null)
        {
            transform.position = m_OrbitTarget.transform.position + offset;
        }
        else
        {
            transform.position = m_PosePosition + offset;
        }
    }

	// Update is called once per frame
	void Update () {
        UpdatePosition();
	}
}
