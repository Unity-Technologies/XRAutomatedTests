using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class RenderingChecks : XrFunctionalTestBase
{
    enum States
    {
        MsaaAndHdr = 0,
        Msaa,
        Hdr,
        NoMsaaAndNoHdr
    }

    private States currentState;
    private bool doVerification;
    private bool stopTest;
    private bool allTestsPassed = true;

    private GameObject colorScreen;
    private Material testMat;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        testMat = new Material(Resources.Load("Materials/YFlipColorMesh", typeof(Material)) as Material);
        currentState = States.MsaaAndHdr;

        colorScreen = GameObject.CreatePrimitive(PrimitiveType.Quad);
        colorScreen.transform.position = new Vector3(0f, 0f, 1f);
        colorScreen.GetComponent<Renderer>().material = testMat;
    }

    [TearDown]
    public override void TearDown()
    {
        GameObject.Destroy(colorScreen);
        base.TearDown();
    }

    [UnityTest]
    [Description("Regression test against fogbugz issue https://fogbugz.unity3d.com/f/cases/1145324/ - particle systems crash mobile XR.")]
    public IEnumerator ParticleSmokeTest()
    {
        Camera = new GameObject();
        Camera.AddComponent<Camera>();
        Camera.tag = "MainCamera";

        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = new Vector3(0, 0, 10);
        ParticleSystem particles = go.AddComponent<ParticleSystem>();
        go.GetComponent<ParticleSystemRenderer>().material = Resources.Load<Material>("Materials/Particle");
        Assert.IsNotNull(go.GetComponent<ParticleSystemRenderer>().material);
        particles.Play();

        // 100 frames ~= 1 or 2 seconds
        for (int i = 0; i < 100; i++)
            yield return null;
    }

    // TODO We need to refactor this so that we have a clear arrange/act/assert
    [UnityTest]
    public IEnumerator VerifyYWorldCoordinate()
    {
        while (!stopTest)
        {
            DoTest();
            yield return SkipFrame(2 * OneSecOfFramesWaitTime);
        }
    }

    void DoTest()
    {
        doVerification = true;

        switch (currentState)
        {
            case States.MsaaAndHdr:
                XrFunctionalTestHelpers.Camera.GetComponent<Camera>().allowHDR = true;
                XrFunctionalTestHelpers.Camera.GetComponent<Camera>().allowMSAA = true;
                Debug.Log("MSAA AND HDR");
                break;
            case States.Msaa:
                XrFunctionalTestHelpers.Camera.GetComponent<Camera>().allowHDR = false;
                XrFunctionalTestHelpers.Camera.GetComponent<Camera>().allowMSAA = true;
                Debug.Log("MSAA");
                break;
            case States.Hdr:
                XrFunctionalTestHelpers.Camera.GetComponent<Camera>().allowHDR = true;
                XrFunctionalTestHelpers.Camera.GetComponent<Camera>().allowMSAA = false;
                Debug.Log("HDR");
                break;
            default:
                XrFunctionalTestHelpers.Camera.GetComponent<Camera>().allowHDR = false;
                XrFunctionalTestHelpers.Camera.GetComponent<Camera>().allowMSAA = false;
                Debug.Log("NO MSAA and NO HDR");
                break;
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
        var originalActiveRenderTexture = RenderTexture.active;

        RenderTexture.active = src;
        var tex = new Texture2D(src.width, src.height, TextureFormat.RGBA32, src.useMipMap, src.sRGB);
        tex.name = "Y Flip Test Texture";
        tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        tex.Apply();

        // We shouldn't sample directly from (0,0) because chances are that will overlap
        // the occlusion mesh.  Therefore we should try to sample closer to the center bottom of the texture.
        var x = src.width * 0.5f;
        var y = src.height * 0.3f;
        var color = tex.GetPixel((int)x, (int)y);
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

    // TODO: How can we move this out of the test?
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
