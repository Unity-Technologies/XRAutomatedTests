using UnityEditor.TestTools.Graphics;
using UnityEditor.TestTools.ConfigManager;
using UnityEngine.TestTools;

public class TestSetup : IPrebuildSetup
{
    public void Setup()
    {
        ImageHandlingSetup.Setup();
        new SetupGraphicsTestCases().Setup();
    }
}

public static class Setup
{
    public static void SetupAll()
    {
        new EnablePlatformPrebuildStep().Setup();
    }
}