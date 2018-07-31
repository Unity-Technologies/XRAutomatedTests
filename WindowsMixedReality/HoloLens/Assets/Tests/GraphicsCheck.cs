using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Boo.Lang.Environments;
using UnityEngine.XR;

internal class GraphicsCheck : HoloLensTestBase
{
    enum States
    {
        MSAA_AND_HDR = 0,
        MSAA,
        HDR,
        NO_MSAA_AND_NO_HDR
    }

    private States currentState;
    private bool doVerification = false;
    private bool stopTest = false;
    private bool allTestsPassed = true;

    private GameObject colorScreen = null;
    private Material testMat;

    [SetUp]
    public void SetUp()
    {
        testMat = new Material(Resources.Load("Materials/YFlipColorMesh", typeof(Material)) as Material);
        currentState = States.MSAA_AND_HDR;

        colorScreen = GameObject.CreatePrimitive(PrimitiveType.Quad);
        colorScreen.transform.position = new Vector3(0f, 0f, 1f);
        colorScreen.GetComponent<Renderer>().material = testMat;
    }

    [TearDown]
    public void TearDown()
    {
        GameObject.Destroy(colorScreen);
    }

    [UnityTest]
    public IEnumerator CheckYWorldCoordinate()
    {
        while (!stopTest)
        {
            DoTest();
            yield return new WaitForSeconds(2.0f);
        }
    }

    void DoTest()
    {
        doVerification = true;

        if (currentState == States.MSAA_AND_HDR)
        {
            m_Camera.GetComponent<Camera>().allowHDR = true;
            m_Camera.GetComponent<Camera>().allowMSAA = true;
            Debug.Log("MSAA AND HDR");
        }
        else if (currentState == States.MSAA)
        {
            m_Camera.GetComponent<Camera>().allowHDR = false;
            m_Camera.GetComponent<Camera>().allowMSAA = true;
            Debug.Log("MSAA");
        }
        else if (currentState == States.HDR)
        {
            m_Camera.GetComponent<Camera>().allowHDR = true;
            m_Camera.GetComponent<Camera>().allowMSAA = false;
            Debug.Log("HDR");
        }
        else
        {
            m_Camera.GetComponent<Camera>().allowHDR = false;
            m_Camera.GetComponent<Camera>().allowMSAA = false;
            Debug.Log("NO MSAA and NO HDR");
        }

        currentState = currentState + 1;
        if ((int)currentState >= System.Enum.GetValues(typeof(States)).Length)
        {
            stopTest = true;

            if (allTestsPassed)
            {
                Debug.Log("The y-flip test passed successfully!");
            }
            else
            {
                Debug.Log("The y-flip test failed!");
            }
        }
    }

    bool IsYFlipCorrect(RenderTexture src)
    {
        RenderTexture originalActiveRenderTexture = RenderTexture.active;

        RenderTexture.active = src;
        Texture2D tex = new Texture2D(src.width, src.height, TextureFormat.RGBA32, src.useMipMap, src.sRGB);
        tex.name = "Y Flip Test Texture";
        tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        tex.Apply();

        // We shouldn't sample directly from (0,0) because chances are that will overlap
        // the occlusion mesh.  Therefore we should try to sample closer to the center bottom of the texture.
        float x = src.width * 0.5f;
        float y = src.height * 0.3f;
        Color color = tex.GetPixel((int)x, (int)y);
        tex = null;

        RenderTexture.active = originalActiveRenderTexture;

        // Texture coordinates start at lower left corner.  So (0,0) should be red.
        // https://docs.unity3d.com/ScriptReference/Texture2D.GetPixel.html
        if (color == Color.red)
        {
            return true;
        }

        return false;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        if (doVerification)
        {
            if (!IsYFlipCorrect(src))
            {
                Debug.LogError(string.Format("The texture is y-flipped incorrectly for camera mode {0}", System.Enum.GetName(typeof(States), currentState)));
                allTestsPassed = false;
            }
            doVerification = false;
        }

        Graphics.Blit(src, dst);
    }
}

