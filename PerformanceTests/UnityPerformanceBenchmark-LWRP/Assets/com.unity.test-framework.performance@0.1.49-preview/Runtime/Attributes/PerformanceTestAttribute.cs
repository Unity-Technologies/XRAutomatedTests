using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;
using NUnit.Framework.Internal.Commands;
using UnityEngine.TestTools;

namespace Unity.PerformanceTesting
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PerformanceTestAttribute : TestAttribute, IWrapTestMethod
    {
        public TestCommand Wrap(TestCommand command)
        {
        #if UNITY_2018_2_OR_NEWER
            return new PerformanceTestCommand(command);
        #else
            return new PerformanceTestCommand((TestMethod)command.Test);
        #endif
        }
    }
}