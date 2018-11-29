using UnityEngine.TestTools;

public class GraphicsTestSetup : IPrebuildSetup
{
    public void Setup()
    {
        new EnablePlatformPrebuildStep().Setup();
        new UnityEditor.TestTools.Graphics.SetupGraphicsTestCases().Setup();
    }
}
