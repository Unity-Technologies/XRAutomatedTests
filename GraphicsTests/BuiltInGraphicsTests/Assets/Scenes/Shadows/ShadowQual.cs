using UnityEngine;

public class ShadowQual : MonoBehaviour
{
    public float shadowDist;
    public int shadowCasc;
    public ShadowProjection shadowProj;
    private float oldShadowDistance;
    private int oldShadowCasc;
    private ShadowProjection oldShadowProj;

    public void Start()
    {
        oldShadowDistance = QualitySettings.shadowDistance;
        QualitySettings.shadowDistance = shadowDist;
        oldShadowCasc = QualitySettings.shadowCascades;
        QualitySettings.shadowCascades = shadowCasc;
        oldShadowProj = QualitySettings.shadowProjection;
        QualitySettings.shadowProjection = shadowProj;
    }

    public void OnDisable()
    {
        QualitySettings.shadowDistance = oldShadowDistance;
        QualitySettings.shadowCascades = oldShadowCasc;
        QualitySettings.shadowProjection = oldShadowProj;
    }

    public ShadowQual()
    {
        shadowDist = 20f;
        shadowCasc = 2;
        shadowProj = ShadowProjection.CloseFit;
        oldShadowCasc = 2;
        oldShadowProj = ShadowProjection.CloseFit;
    }
}
