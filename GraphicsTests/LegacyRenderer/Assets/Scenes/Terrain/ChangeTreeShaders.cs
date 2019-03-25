using UnityEngine;

public class ChangeTreeShaders : MonoBehaviour
{
    public GameObject newTree;
    private GameObject oldTree;
    private int frames;

#if ENABLE_TERRAIN

    public void Update()
    {
        if (frames == 1)
        {
            var terrain  = GetComponent<Terrain>();
            var data  = terrain.terrainData;
            var protos = data.treePrototypes;
            oldTree = protos[0].prefab;
            protos[0].prefab = newTree;
            data.treePrototypes = protos;
            terrain.Flush();
        }
        ++frames;
    }

    public void OnDisable()
    {
        if (oldTree)
        {
            var terrain = GetComponent<Terrain>();
            var data  = terrain.terrainData;
            var protos = data.treePrototypes;
            protos[0].prefab = oldTree;
            data.treePrototypes = protos;
        }
    }

#endif
}
