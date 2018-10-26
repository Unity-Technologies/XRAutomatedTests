using UnityEngine;

public class SwitchQuality : MonoBehaviour
{
    public QualityLevel level;
    public int switchBackAfterFrames;
    private QualityLevel oldLevel;
    private bool switchBackDone;

    public void Start()
    {
        oldLevel = (QualityLevel)QualitySettings.GetQualityLevel();
        QualitySettings.SetQualityLevel((int)level);
    }

    public void Update()
    {
        --switchBackAfterFrames;
        if (switchBackAfterFrames <= 0 && !switchBackDone)
        {
            QualitySettings.SetQualityLevel((int)oldLevel);
            switchBackDone = true;
        }
    }

    public void OnDisable()
    {
        if (!switchBackDone)
        {
            QualitySettings.SetQualityLevel((int)oldLevel);
            switchBackDone = true;
        }
    }

    public SwitchQuality()
    {
        level = QualityLevel.Fantastic;
        switchBackAfterFrames = -1;
        oldLevel = QualityLevel.Good;
    }
}
