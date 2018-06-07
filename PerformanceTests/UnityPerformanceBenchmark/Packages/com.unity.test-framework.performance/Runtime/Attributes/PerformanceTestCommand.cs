using System;
using System.Collections;
using UnityEngine.TestTools.Logging;
using UnityEngine.TestTools.TestRunner;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;
#if UNITY_2018_2_OR_NEWER
using UnityEngine.TestRunner.NUnitExtensions.Runner;
#else
using UnityEngine.TestTools.NUnitExtensions;
#endif

namespace Unity.PerformanceTesting
{
#if UNITY_2018_2_OR_NEWER
    public class PerformanceTestCommand : DelegatingTestCommand, IEnumerableTestMethodCommand
    {
        public PerformanceTestCommand(TestCommand innerCommand) : base(innerCommand)
        {
        }

        public override TestResult Execute(ITestExecutionContext context)
        {
            PerformanceTest.StartTest(context.CurrentTest);

            try
            {
                innerCommand.Execute(context);
            }
            catch (Exception exception)
            {
                context.CurrentResult.RecordException(exception);
            }

            if (PerformanceTest.Active.Failed)
                context.CurrentResult.SetResult(ResultState.Failure);
            PerformanceTest.EndTest(context.CurrentTest);
            return context.CurrentResult;
        }

        public IEnumerable ExecuteEnumerable(ITestExecutionContext context)
        {
            PerformanceTest.StartTest(context.CurrentTest);
            var logCollector = new LogScope();

            if (!(innerCommand is IEnumerableTestMethodCommand))
            {
                Execute(context);
                yield break;
            }

            var enumerableTestMethodCommand = (IEnumerableTestMethodCommand) innerCommand;

            IEnumerable executeEnumerable;

            try
            {
                executeEnumerable = enumerableTestMethodCommand.ExecuteEnumerable(context);
            }
            catch (Exception e)
            {
                context.CurrentResult.RecordException(e);
                yield break;
            }

            foreach (var step in executeEnumerable)
            {
                try
                {
                    if (logCollector.AnyFailingLogs())
                    {
                        var failingLog = logCollector.FailingLogs[0];
                        throw new UnhandledLogMessageException(failingLog);
                    }
                }
                catch (Exception e)
                {
                    context.CurrentResult.RecordException(e);
                    break;
                }

                yield return step;
            }

            try
            {
                if (logCollector.AnyFailingLogs())
                {
                    var failingLog = logCollector.FailingLogs[0];
                    throw new UnhandledLogMessageException(failingLog);
                }

                logCollector.ProcessExpectedLogs();
                if (logCollector.ExpectedLogs.Count > 0)
                {
                    throw new UnexpectedLogMessageException(LogScope.Current.ExpectedLogs.Peek());
                }
            }
            catch (Exception exception)
            {
                context.CurrentResult.RecordException(exception);
            }

            logCollector.Dispose();
            if (PerformanceTest.Active.Failed)
                context.CurrentResult.SetResult(ResultState.Failure);
            PerformanceTest.EndTest(context.CurrentTest);
        }
    }
#else
    class PerformanceTestCommand : TestCommand
    {
        private readonly TestMethod _testMethod;

        public PerformanceTestCommand(TestMethod testMethod) : base(testMethod)
        {
            this._testMethod = testMethod;
        }

        public override TestResult Execute(TestExecutionContext context)
        {
            PerformanceTest.StartTest(context.CurrentTest);
            RunTestMethod(context);
            context.CurrentResult.SetResult(ResultState.Success);
            PerformanceTest.EndTest(context.CurrentTest);
            return context.CurrentResult;
        }

        private object RunTestMethod(TestExecutionContext context)
        {
            return TestDelegator.instance.DelegateTest(_testMethod, context);
        }
    }
#endif
}