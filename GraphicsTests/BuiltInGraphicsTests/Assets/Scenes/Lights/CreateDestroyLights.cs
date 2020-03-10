using System.Collections;
using UnityEngine;

public class CreateDestroyLights : MonoBehaviour
{
    public int count;
    public ArrayList lights;
    private LightType[] lightTypes;

    public void Start()
    {
        var i = 0;
        while (i < 20)
        {
            Update();
            ++i;
        }
    }

    private void CreateLight()
    {
        var l = new GameObject("l", typeof(Light)).GetComponent<Light>();
        l.transform.localPosition = transform.position + (Random.onUnitSphere * 2f);
        l.transform.localRotation = Random.rotation;
        l.type = lightTypes[Random.Range(0, 3)];
        l.color = new Color(Random.value, Random.value, Random.value, 1f);

        l.intensity = (l.type == LightType.Directional) ? 0.2f : Random.Range(0.4f, 1f);

        if (Random.value < 0.3f)
        {
            l.renderMode = LightRenderMode.ForcePixel;
        }

        l.range = Random.Range(3f, 6f);
        lights.Add(l);
    }

    public void Update()
    {
        var i = 0;
        while (i < count)
        {
            var rnd = Random.value;
            if (rnd < 0.4f)
            {
                CreateLight();
            }
            else
            {
                Light l;
                int idx;
                if ((rnd > 0.6f) && (lights.Count > 0))
                {
                    idx = Random.Range(0, lights.Count);
                    l = lights[idx] as Light;
                    lights.RemoveAt(idx);
                    DestroyImmediate(l.gameObject);
                }
                else
                {
                    if (lights.Count > 0)
                    {
                        idx = Random.Range(0, lights.Count);
                        l = lights[idx] as Light;
                        l.transform.localPosition = l.transform.localPosition + Random.insideUnitSphere;
                    }
                }
            }
            ++i;
        }
    }

    public CreateDestroyLights()
    {
        count = 20;
        lights = new ArrayList();
        lightTypes = new[] { LightType.Directional, LightType.Point, LightType.Spot };
    }
}
