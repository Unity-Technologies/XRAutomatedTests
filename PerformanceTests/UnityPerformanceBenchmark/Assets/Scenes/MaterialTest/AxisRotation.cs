using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisRotation : MonoBehaviour {
    public Vector3 axis;
    public float angular_speed;
    public bool m_Local = false;
    
    private void Update()
    {
        if (m_Local)
        {
            transform.localRotation = Quaternion.AngleAxis(angular_speed * Time.time, axis.normalized);
        }
        else
        {
            transform.rotation = Quaternion.AngleAxis(angular_speed * Time.deltaTime, axis.normalized) * transform.rotation;
        }
    }


}
