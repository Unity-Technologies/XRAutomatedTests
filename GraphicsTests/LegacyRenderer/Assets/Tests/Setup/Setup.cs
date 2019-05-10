using UnityEditor.TestTools.ConfigManager;
using UnityEditor.TestTools.Graphics;
using UnityEditor;

public static class Setup
{
    [MenuItem("Tests/SetupAll")]
    public static void SetupAll()
    {
        new EnablePlatformPrebuildStep().Setup();
        ImageHandlingSetup.Setup();
        SetupGraphicsTestCases.Setup();
    }
}
