using UnityEngine.TestTools;
using UnityEditor.TestTools.ConfigManager;
using UnityEditor.TestTools.Graphics;
using UnityEditor;

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
