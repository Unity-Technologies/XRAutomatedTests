#if ENABLE_MOVIES && ENABLE_WWW
using System.IO;
using UnityEngine;

public class PlayMovieWWW : MonoBehaviour
{
    private WWW www;

    public void Start()
    {
#if UNITY_WINRT_8_1
        var fullpath = Path.Combine(Application.streamingAssetsPath, "TheoraMovie.ogv");
#else
        var appDirInfo = new DirectoryInfo(Application.dataPath);
        var rootDir = appDirInfo.Parent;
        var fullpath = Path.Combine(rootDir.FullName, "TheoraMovie.ogv");
#endif
        www = new WWW("file://" + fullpath);
    }

    public void Update()
    {
        if (www == null)
            return;

        while (!www.isDone) {}

        var movieTex = www.GetMovieTexture();
        movieTex.loop = true;

        if (GetComponent<GUITexture>())
            GetComponent<GUITexture>().texture = movieTex;
        else
            GetComponent<Renderer>().material.mainTexture = movieTex;
        movieTex.Play();

        www = null;
    }
}
#endif
