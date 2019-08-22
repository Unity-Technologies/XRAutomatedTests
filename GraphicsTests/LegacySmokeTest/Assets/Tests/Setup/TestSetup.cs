using UnityEditor.TestTools.Graphics;
using UnityEngine.TestTools;
using com.unity.cliconfigmanager;

public class TestSetup : IPrebuildSetup
{
    public void Setup()
    {
        new PrebuildSettingsConfigurator().ConfigureFromCmdlineArgs();
        SetupGraphicsTestCases.Setup();
    }
}