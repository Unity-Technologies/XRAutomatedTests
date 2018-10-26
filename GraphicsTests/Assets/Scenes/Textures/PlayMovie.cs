using UnityEngine;

public class PlayMovie : MonoBehaviour
{
    public Texture m;

#if ENABLE_MOVIES
    public void Start()
    {
        var movieTex = m as MovieTexture;

        if (GetComponent<GUITexture>())
            GetComponent<GUITexture>().texture = movieTex;
        else
            GetComponent<Renderer>().material.mainTexture = movieTex;
        movieTex.Play();
    }

#endif
}
