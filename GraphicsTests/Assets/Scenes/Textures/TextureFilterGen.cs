using UnityEngine;

public class TextureFilterGen : MonoBehaviour
{
    public FilterMode filter;
    public int aniso = 1;
    public float bias;

    public void Start()
    {
        var texture = Instantiate(GetComponent<Renderer>().material.mainTexture) as Texture2D;
        texture.filterMode = filter;
        texture.anisoLevel = aniso;
        texture.mipMapBias = bias;
        GetComponent<Renderer>().material.mainTexture = texture;
        var colors = new Color[9];
        colors[0] = Color.red;
        colors[1] = Color.green;
        colors[2] = Color.blue;
        colors[3] = Color.yellow;
        colors[4] = Color.magenta;
        colors[5] = Color.cyan;
        colors[6] = Color.red;
        colors[7] = Color.green;
        colors[8] = Color.blue;
        var mipCount = Mathf.Min(9, texture.mipmapCount);
        // tint each mip level
        var mip = 0;
        while (mip < mipCount)
        {
            var cols = texture.GetPixels(mip);
            var i = 0;
            while (i < cols.Length)
            {
                cols[i] = Color.Lerp(cols[i], colors[mip], 0.15f);
                ++i;
            }
            texture.SetPixels(cols, mip);
            ++mip;
        }
        // actually apply all SetPixels, don't recalculate mip levels
        texture.Apply(false);
    }
}
