using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectorRotation : MonoBehaviour {

    public float m_SinusoidalVelocity;
    public float m_SinusoidalMaxRotation;
    public Transform m_Parent;

	// Use this for initialization
	void Start () {
	}
	
    void UpdateRotation()
    {
        if (m_Parent != null)
        {
            float angle = Mathf.Sin(Time.time * m_SinusoidalVelocity) * m_SinusoidalMaxRotation;
            Quaternion sinusoidalRotation = Quaternion.AngleAxis(angle, m_Parent.forward);

            transform.localRotation = sinusoidalRotation;
        }
    }

	// Update is called once per frame
	void Update () {
        UpdateRotation();
	}
}
