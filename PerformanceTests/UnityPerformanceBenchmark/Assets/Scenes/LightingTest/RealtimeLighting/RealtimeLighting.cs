using System.Collections;
using UnityEngine;

public class RealtimeLighting : MonoBehaviour {

    public RealtimeLight[] m_RealtimeLights;
    public float m_FadeDuration = 0.5f;
    public float m_IdleDuration = 2.0f;

    public Orbit m_Orbit;
    public MeshFilter m_LabelMeshFilter;

    private IEnumerator Fade(float direction, RealtimeLight light)
    {
        
        light.gameObject.SetActive(true);
        direction = Mathf.Sign(direction);

        Debug.Log("Begin Fade on " + light.gameObject.name + " direction=" + direction);

        light.SetIntensity(direction > 0f ? 0f : 1.0f);

        if (m_LabelMeshFilter != null)
        {
            m_LabelMeshFilter.mesh = light.textMesh;
        }

        float timer = 0.0f;
        while(timer < m_FadeDuration)
        {
            float t = Mathf.Clamp01(timer / m_FadeDuration);

            light.SetIntensity(direction > 0 ? t : (1.0f - t));
            
            if(m_LabelMeshFilter != null)
            {
                if (direction > 0f)
                {
                    m_LabelMeshFilter.transform.localRotation = Quaternion.AngleAxis(180.0f - t * 180.0f, Vector3.right);
                }
                else
                {
                    m_LabelMeshFilter.transform.localRotation = Quaternion.AngleAxis( 180.0f + (1.0f - t) * 180.0f , Vector3.right);
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }

        light.SetIntensity(direction > 0f ? 1f : 0.0f);
        light.gameObject.SetActive(false);

        if(m_LabelMeshFilter != null)
        {
            m_LabelMeshFilter.transform.localRotation = direction > 0 ? Quaternion.AngleAxis(0.0f, Vector3.right) : Quaternion.AngleAxis(90.0f, Vector3.right);
        }

        Debug.Log("Ending Fade on " + light.gameObject.name + " direction=" + direction);
    }

    private IEnumerator Idle(RealtimeLight light)
    {
        Debug.Log("Begin Idle on " + light.gameObject.name);
        light.gameObject.SetActive(true);
        yield return new WaitForSeconds(m_IdleDuration);
        Debug.Log("Ending Idle on " + light.gameObject.name);
    }

    public IEnumerator Start()
    {
        int index = 0;

        //Initialize
        //DisableLights();
        if(m_RealtimeLights.Length > 0)
        {
            m_RealtimeLights[0].SetIntensity(1.0f);
        }

        //Loop through lights
        if (m_RealtimeLights.Length > 1)
        {
            while (true)
            {
                yield return StartCoroutine(Idle(m_RealtimeLights[index]));
                yield return StartCoroutine(Fade(-1.0f, m_RealtimeLights[index]));
                index++;
                if (index >= m_RealtimeLights.Length)
                {
                    index = 0;
                }
                yield return StartCoroutine(Fade(1.0f, m_RealtimeLights[index]));
            }
        }
    }

    public void Update()
    {
        if (m_Orbit != null)
        {
            foreach (RealtimeLight light in m_RealtimeLights)
            {
                light.transform.position = m_Orbit.transform.position;
            }
        }
    }

    private void DisableLights()
    {
        foreach(RealtimeLight light in m_RealtimeLights)
        {
            light.gameObject.SetActive(false);
        }
    }
}
