using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour {

    public GameObject m_Target;
    public float m_AngularVelocity;
    public Vector3 m_RotationAxis;

    private float m_Radius;
    private Vector3 m_Direction;
    private float m_AngleOffset = 0.0f;

	// Use this for initialization
	void Start () {
        m_Direction = (transform.position - m_Target.transform.position);
        m_Radius = m_Direction.magnitude;
        m_Direction.Normalize();
    }
	
	// Update is called once per frame
	void Update ()
    {
        m_AngleOffset += Time.deltaTime * m_AngularVelocity;
        transform.position = m_Target.transform.position + Quaternion.AngleAxis(m_AngleOffset, m_RotationAxis.normalized) * m_Direction * m_Radius;
        
	}
}
