using UnityEditor.TestTools.ConfigManager;

public static class Setup
{
    public static void SetupAll()
    {
        new EnablePlatformPrebuildStep().Setup();
        ImageHandlingSetup.Setup();
    }
}