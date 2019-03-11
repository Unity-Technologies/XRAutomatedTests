using System;
using NUnit.Framework;
using NUnit.Framework.Internal.Commands;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;
using UnityEngine;
using UnityEngine.TestTools;

#if UNITY_2019_1_OR_NEWER
using UnityEngine.TestRunner.NUnitExtensions.Runner;
#endif

namespace Unity.PerformanceTesting
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PerformanceUnityTestAttribute : 
#if UNITY_2018_2_OR_NEWER
        CombiningStrategyAttribute, ISimpleTestBuilder, IWrapSetUpTearDown, IImplyFixture 
    #else
        CombiningStrategyAttribute, ISimpleTestBuilder, IWrapTestMethod, IImplyFixture 
#endif
    {
        public PerformanceUnityTestAttribute() : base(new UnityCombinatorialStrategy(),
            new ParameterDataSourceProvider())
        {
        }

        readonly NUnitTestCaseBuilder _builder = new NUnitTestCaseBuilder();

        TestMethod ISimpleTestBuilder.BuildFrom(IMethodInfo method, Test suite)
        {
            TestCaseParameters parms = new TestCaseParameters();
            parms.ExpectedResult = new object();
            parms.HasExpectedResult = true;

            var t = _builder.BuildTestMethod(method, suite, parms);

            if (t.parms != null)
                t.parms.HasExpectedResult = false;
            return t;
        }

        public TestCommand Wrap(TestCommand command)
        {
        #if UNITY_2019_1_OR_NEWER
            return new PerformanceTestCommand(new UnityEngine.TestTools.SetUpTearDownCommand(new UnityLogCheckDelegatingCommand(new EnumerableTestMethodCommand((TestMethod)command.Test))));
        #elif UNITY_2018_2_OR_NEWER
            return new PerformanceTestCommand(new EnumerableSetUpTearDownCommand(new EnumerableTestMethodCommand((TestMethod)command.Test)));
        #else
            return new PerformanceTestCommand((TestMethod) command.Test);
        #endif
        }
    }
}