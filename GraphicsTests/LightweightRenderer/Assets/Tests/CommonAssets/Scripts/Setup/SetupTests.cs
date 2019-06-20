using UnityEditor;
using UnityEditor.TestTools.Graphics;
using UnityEditor.TestTools.ConfigManager;
using UnityEngine;
using UnityEngine.TestTools;

public class SetupTests : IPrebuildSetup
{
    public void Setup()
    {
        new EnablePlatformPrebuildStep().Setup();
        ImageHandlingSetup.Setup();
        new SetupGraphicsTestCases().Setup();
    }
}