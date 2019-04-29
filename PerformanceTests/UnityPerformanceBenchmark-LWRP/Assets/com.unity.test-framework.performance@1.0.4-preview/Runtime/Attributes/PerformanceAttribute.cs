using System;
using System.Collections;
using NUnit.Framework.Interfaces;
using UnityEngine.TestTools;

namespace Unity.PerformanceTesting
{
    public class PerformanceAttribute : Attribute, IOuterUnityTestAction
    {
        public IEnumerator BeforeTest(ITest test)
        {
            PerformanceTest.StartTest(test);
            yield return null;
        }

        public IEnumerator AfterTest(ITest test)
        {
            PerformanceTest.EndTest(test);
            yield return null;
        }
    }
}