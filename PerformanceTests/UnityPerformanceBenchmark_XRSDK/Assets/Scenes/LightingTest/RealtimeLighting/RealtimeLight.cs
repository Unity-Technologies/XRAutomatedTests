using UnityEngine;

public class RealtimeLight : MonoBehaviour {
    public Light Light { get; private set; }
    public Mesh TextMesh;
    public Billboard LightHalo { get; private set; }
    private Material lightHaloMaterial;

    public float MaxIntensity { get; private set; }

    public float Intensity = 0.0f;

    private Color PoseHaloColor;

    //SetIntensity between 0 and 1
    public void SetIntensity(float factor)
    {
        if(Light != null)
        {
            Light.intensity = factor * MaxIntensity;
        }
        if(lightHaloMaterial != null)
        {
            Color haloColor = PoseHaloColor;
            haloColor.a *= factor;
            lightHaloMaterial.SetColor("_TintColor", haloColor);
        }
        Intensity = factor;
    }
    
    // Use this for initialization
    void Start()
    {
        Light = GetComponentInChildren<Light>();
        LightHalo = GetComponentInChildren<Billboard>();
        if (Light != null)
        {
            MaxIntensity = Light.intensity;
        }

        if (LightHalo != null) {
            lightHaloMaterial = LightHalo.gameObject.GetComponentInChildren<MeshRenderer>().material;
            PoseHaloColor = lightHaloMaterial.GetColor("_TintColor");
        }
        SetIntensity(Intensity);
    }
    
}
