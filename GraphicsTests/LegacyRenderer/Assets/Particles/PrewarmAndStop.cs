using UnityEngine;

public class PrewarmAndStop : MonoBehaviour
{
    public float prewarmto = 4f;
    public ParticleSystem theParticleSystem;

    public void Start()
    {
        if (theParticleSystem)
            theParticleSystem.Simulate(prewarmto);
    }

    public void OnPostRender()
    {
        if (theParticleSystem)
            theParticleSystem.Stop();
    }
}
