using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealtimeLight : MonoBehaviour {
    public new Light light { get; private set; }
    public Mesh textMesh;
    public Billboard lightHalo { get; private set; }
    private Material lightHaloMaterial;

    public float maxIntensity { get; private set; }

    public float m_Intensity = 0.0f;

    private Color m_PoseHaloColor;

    //SetIntensity between 0 and 1
    public void SetIntensity(float factor)
    {
        if(light != null)
        {
            light.intensity = factor * maxIntensity;
        }
        if(lightHaloMaterial != null)
        {
            Color haloColor = m_PoseHaloColor;
            haloColor.a *= factor;
            lightHaloMaterial.SetColor("_TintColor", haloColor);
        }
        m_Intensity = factor;
    }
    
    // Use this for initialization
    void Start()
    {
        light = GetComponentInChildren<Light>();
        lightHalo = GetComponentInChildren<Billboard>();
        if (light != null)
        {
            maxIntensity = light.intensity;
        }

        if (lightHalo != null) {
            lightHaloMaterial = lightHalo.gameObject.GetComponentInChildren<MeshRenderer>().material;
            m_PoseHaloColor = lightHaloMaterial.GetColor("_TintColor");
        }
        SetIntensity(m_Intensity);
    }
    
}
