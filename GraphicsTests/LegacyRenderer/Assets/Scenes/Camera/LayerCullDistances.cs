using UnityEngine;

public class LayerCullDistances : MonoBehaviour
{
    public void Start()
    {
        var distances = new float[32];
        distances[1] = 20;
        distances[2] = 10;
        GetComponent<Camera>().layerCullDistances = distances;
    }
}
