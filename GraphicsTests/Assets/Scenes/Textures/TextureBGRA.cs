using UnityEngine;
using UnityEngine.UI;

// Generate BGRA32 data

public class TextureBGRA : MonoBehaviour
{
    void Start()
    {
        const int dim = 128;

        byte[] data = new byte[dim * dim * 4];

        // generate quadrants of each primary colour, full alpha
        // and one of black no alpha:
        //
        // x=0
        //     R | 0
        //     -----
        //     B | G
        //     y=0
        for (int x = 0; x < dim; x++)
        {
            for (int y = 0; y < dim; y++)
            {
                if (x < dim / 2 && y < dim / 2)
                {
                    data[(y * dim + x) * 4 + 0] = 255;
                    data[(y * dim + x) * 4 + 1] = 0;
                    data[(y * dim + x) * 4 + 2] = 0;
                    data[(y * dim + x) * 4 + 3] = 255;
                }
                else if (y < dim / 2)
                {
                    data[(y * dim + x) * 4 + 0] = 0;
                    data[(y * dim + x) * 4 + 1] = 255;
                    data[(y * dim + x) * 4 + 2] = 0;
                    data[(y * dim + x) * 4 + 3] = 255;
                }
                else if (x < dim / 2)
                {
                    data[(y * dim + x) * 4 + 0] = 0;
                    data[(y * dim + x) * 4 + 1] = 0;
                    data[(y * dim + x) * 4 + 2] = 255;
                    data[(y * dim + x) * 4 + 3] = 255;
                }
                else
                {
                    data[(y * dim + x) * 4 + 0] = 0;
                    data[(y * dim + x) * 4 + 1] = 0;
                    data[(y * dim + x) * 4 + 2] = 0;
                    data[(y * dim + x) * 4 + 3] = 0;
                }
            }
        }

        Texture2D tex = new Texture2D(dim, dim, TextureFormat.BGRA32, false);
        tex.LoadRawTextureData(data);
        tex.Apply();
        GetComponent<RawImage>().texture = tex;
    }
}
