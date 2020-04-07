using System.Collections;
using com.unity.xr.test.runtimesettings;
using NUnit.Framework;
using UnityEngine;

public abstract class XrFunctionalTestBase
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
		if(XrFunctionalTestHelpers != null)
			XrFunctionalTestHelpers.TestStageSetup(TestStageConfig.CleanStage);
    }

    protected bool IsMobilePlatform()
    {
        return Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;
    }

    protected void AssertNotUsingEmulation()
    {
        if (Settings.SimulationMode != string.Empty)
        {
            Assert.Ignore("This test cannot run in emulation mode. Skipping.");
        }
    }

    protected IEnumerator SkipFrame(int frames)
    {
        Debug.Log(string.Format("Skipping {0} frames.", frames));
        for (int f = 0; f < frames; f++)
        {
            yield return null;
        }
    }

    protected IEnumerator SkipFrame()
    {
        yield return SkipFrame(1);
    }
}
