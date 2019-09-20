using System.Collections;
using UnityEngine;

public class RealtimeLighting : MonoBehaviour {

    public RealtimeLight[] RealtimeLights;
    public float FadeDuration = 0.5f;
    public float IdleDuration = 2.0f;

    public Orbit Orbit;
    public MeshFilter LabelMeshFilter;

    private IEnumerator Fade(float direction, RealtimeLight light)
    {
        
        light.gameObject.SetActive(true);
        direction = Mathf.Sign(direction);

        Debug.Log("Begin Fade on " + light.gameObject.name + " direction=" + direction);

        light.SetIntensity(direction > 0f ? 0f : 1.0f);

        if (LabelMeshFilter != null)
        {
            LabelMeshFilter.mesh = light.TextMesh;
        }

        float timer = 0.0f;
        while(timer < FadeDuration)
        {
            float t = Mathf.Clamp01(timer / FadeDuration);

            light.SetIntensity(direction > 0 ? t : (1.0f - t));
            
            if(LabelMeshFilter != null)
            {
                if (direction > 0f)
                {
                    LabelMeshFilter.transform.localRotation = Quaternion.AngleAxis(180.0f - t * 180.0f, Vector3.right);
                }
                else
                {
                    LabelMeshFilter.transform.localRotation = Quaternion.AngleAxis( 180.0f + (1.0f - t) * 180.0f , Vector3.right);
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }

        light.SetIntensity(direction > 0f ? 1f : 0.0f);
        light.gameObject.SetActive(false);

        if(LabelMeshFilter != null)
        {
            LabelMeshFilter.transform.localRotation = direction > 0 ? Quaternion.AngleAxis(0.0f, Vector3.right) : Quaternion.AngleAxis(90.0f, Vector3.right);
        }

        Debug.Log("Ending Fade on " + light.gameObject.name + " direction=" + direction);
    }

    private IEnumerator Idle(RealtimeLight light)
    {
        Debug.Log("Begin Idle on " + light.gameObject.name);
        light.gameObject.SetActive(true);
        yield return new WaitForSeconds(IdleDuration);
        Debug.Log("Ending Idle on " + light.gameObject.name);
    }

    public IEnumerator Start()
    {
        int index = 0;

        //Initialize
        //DisableLights();
        if(RealtimeLights.Length > 0)
        {
            RealtimeLights[0].SetIntensity(1.0f);
        }

        //Loop through lights
        if (RealtimeLights.Length > 1)
        {
            while (true)
            {
                yield return StartCoroutine(Idle(RealtimeLights[index]));
                yield return StartCoroutine(Fade(-1.0f, RealtimeLights[index]));
                index++;
                if (index >= RealtimeLights.Length)
                {
                    index = 0;
                }
                yield return StartCoroutine(Fade(1.0f, RealtimeLights[index]));
            }
        }
    }

    public void Update()
    {
        if (Orbit != null)
        {
            foreach (RealtimeLight light in RealtimeLights)
            {
                light.transform.position = Orbit.transform.position;
            }
        }
    }

    private void DisableLights()
    {
        foreach(RealtimeLight light in RealtimeLights)
        {
            light.gameObject.SetActive(false);
        }
    }
}
