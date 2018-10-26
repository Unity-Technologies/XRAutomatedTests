using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR;
using UnityEditor;

public class GraphicsTestSetup : IPrebuildSetup
{
    public void Setup()
    {
        new EnablePlatformPrebuildStep().Setup();
        new UnityEditor.TestTools.Graphics.SetupGraphicsTestCases().Setup();
    }
}
