using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Billboard : MonoBehaviour {

	// Use this for initialization
	void Start () {
        UpdateOrientation();	
	}
	
    public void UpdateOrientation()
    {
        transform.forward = (transform.position - Camera.main.transform.position).normalized;
    }

	// Update is called once per frame
	void Update () {
        UpdateOrientation();
	}
}
