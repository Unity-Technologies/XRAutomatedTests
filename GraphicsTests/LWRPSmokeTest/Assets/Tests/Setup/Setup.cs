using UnityEditor.TestTools.Graphics;
using UnityEditor.TestTools.ConfigManager;
using UnityEngine.TestTools;

public class TestSetup : IPrebuildSetup
{
    public void Setup()
    {
        new EnablePlatformPrebuildStep().Setup();
        ImageHandlingSetup.Setup();
    }
}

public static class Setup
{
    public static void SetupAll()
    {
        new SetupGraphicsTestCases().Setup();
    }
}
