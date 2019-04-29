using UnityEngine;

public class ShaderKeywords : MonoBehaviour
{
    public string[] keywords;
    private int index;

    public void Update()
    {
        foreach (string k in keywords)
        {
            Shader.DisableKeyword(k);
        }
        Shader.EnableKeyword(keywords[index]);
        index = (index + 1) % keywords.Length;
    }
}
