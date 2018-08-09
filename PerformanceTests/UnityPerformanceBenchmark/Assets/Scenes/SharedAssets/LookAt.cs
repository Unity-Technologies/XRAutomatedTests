using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour {

    public Transform m_Target;

	// Use this for initialization
	void Start () {
        UpdateOrientation();
    }
	
    public void UpdateOrientation()
    {
        if (m_Target != null)
        {
            transform.LookAt(m_Target);
        }
    }

	// Update is called once per frame
	void Update () {
        UpdateOrientation();

    }
}
