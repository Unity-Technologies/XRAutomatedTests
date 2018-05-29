using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Line : MonoBehaviour {

    LineRenderer m_LineRenderer;
    public int m_PointCount = 16;
    public float m_SpiralFactor = 1f;
    public float m_SpiralLength = 10.0f;

    public float m_MinSpiralFactor = 0.05f;
    public float m_MaxSpiralFactor = 0.1f;

    public float m_AnimationSpeed = 2.0f;

    private Vector3[] m_Points;

    public Vector3 GetSphericalSpiralPoint(float a, float t)
    {
        Vector3 point;

        t = (t - 0.5f) * 2.0f;

        float d = t * m_SpiralLength;

        float c = Mathf.Sqrt(1 + (a * a) * (d * d));

        point.x = Mathf.Cos(d) / c;
        point.y = Mathf.Sin(d) / c;
        point.z = -((a * d) / c);

        return point;
    }

    private void Awake()
    {
        m_LineRenderer = GetComponent<LineRenderer>();
        
    }

    private void Start()
    {
        m_Points = new Vector3[m_PointCount];
    }

    private void Update()
    {
        if(m_Points.Length != m_PointCount)
        {
            m_Points = new Vector3[m_PointCount];
        }

        m_SpiralFactor = ((Mathf.Sin(Time.time * m_AnimationSpeed) * 0.5f + 0.5f) * (m_MaxSpiralFactor - m_MinSpiralFactor)) + m_MinSpiralFactor;

        float denominator = 1.0f / (float)m_PointCount;
        for(int i = 0; i < m_PointCount; i++)
        {
            m_Points[i] = transform.TransformPoint(GetSphericalSpiralPoint(m_SpiralFactor, (float)i * denominator));
        }

        m_LineRenderer.useWorldSpace = true;
        m_LineRenderer.positionCount = m_PointCount;
        m_LineRenderer.SetPositions(m_Points);
    }
}
