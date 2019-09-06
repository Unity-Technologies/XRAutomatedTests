using UnityEditor.TestTools.Graphics;
using UnityEngine.TestTools;
using com.unity.cliconfigmanager;

public class TestSetup : IPrebuildSetup
{
    public void Setup()
    {
        new CliConfigManager().ConfigureFromCmdlineArgs();
        SetupGraphicsTestCases.Setup();
    }
}