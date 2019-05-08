using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

[UnityPlatform(include = new[]
{
    RuntimePlatform.WindowsEditor,
    RuntimePlatform.WindowsPlayer,
    RuntimePlatform.Android,
    RuntimePlatform.IPhonePlayer,
    RuntimePlatform.OSXEditor,
    RuntimePlatform.OSXPlayer,
    RuntimePlatform.Lumin,
    RuntimePlatform.WSAPlayerARM,
    RuntimePlatform.WSAPlayerX64,
    RuntimePlatform.WSAPlayerX86
})]
public class XrFunctionalTestBase
{
    protected XrFunctionalTestHelpers XrFunctionalTestHelpers;
    protected CurrentSettings Settings;
    protected static int DefaultFrameSkipCount = 1;

    public GameObject Camera;
    public GameObject Light;
    public GameObject Cube;

    [OneTimeSetUp]
    public virtual void OneTimeSetUp()
    {
        Settings = Resources.Load<CurrentSettings>("settings");
    }

    [SetUp]
    public virtual void SetUp()
    {
        XrFunctionalTestHelpers = new XrFunctionalTestHelpers(this);

        XrFunctionalTestHelpers.TestStageSetup(TestStageConfig.BaseStageSetup);
    }

    [TearDown]
    public virtual void TearDown()
    {
        XrFunctionalTestHelpers.TestStageSetup(TestStageConfig.CleanStage);
    }

    protected bool IsMobilePlatform()
    {
        return Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;
    }

    protected IEnumerator SkipFrame(int frames)
    {
        for (int f = 0; f < frames; f++)
        {
            yield return null;
            Debug.Log("Skip Frame");
        }
    }

    protected IEnumerator SkipFrame()
    {
        yield return SkipFrame(1);
    }
}
