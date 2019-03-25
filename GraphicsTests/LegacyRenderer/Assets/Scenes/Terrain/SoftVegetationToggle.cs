using UnityEngine;

public class SoftVegetationToggle : MonoBehaviour
{
    private bool oldSoft;

    public void OnPreCull()
    {
        oldSoft = QualitySettings.softVegetation;
        QualitySettings.softVegetation = false;
    }

    public  void OnPostRender()
    {
        QualitySettings.softVegetation = oldSoft;
    }
}
