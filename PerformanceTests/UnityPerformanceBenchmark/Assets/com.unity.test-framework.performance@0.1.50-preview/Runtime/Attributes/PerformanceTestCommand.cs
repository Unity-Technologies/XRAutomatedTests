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
#if UNITY_2019_1_OR_NEWER
using UnityEngine.TestTools;
using System.Reflection;
using System.Linq;
using UnityEngine;
#endif

namespace Unity.PerformanceTesting
{
#if UNITY_2019_1_OR_NEWER
    public class PerformanceTestCommand : DelegatingTestCommand, IEnumerableTestMethodCommand
    {
        public MethodInfo[] SetUps { get; private set; }
        public MethodInfo[] TearDowns { get; private set; }

        public PerformanceTestCommand(TestCommand innerCommand) : base(innerCommand)
        {
            if (Test.TypeInfo.Type != null)
            {
                SetUps = GetMethodsWithAttributeFromFixture(Test.TypeInfo.Type, typeof(UnitySetUpAttribute));
                TearDowns = GetMethodsWithAttributeFromFixture(Test.TypeInfo.Type, typeof(UnityTearDownAttribute));
            }
        }

        private static MethodInfo[] GetMethodsWithAttributeFromFixture(Type fixtureType, Type setUpType)
        {
            MethodInfo[] methodsWithAttribute = Reflect.GetMethodsWithAttribute(fixtureType, setUpType, true);
            return methodsWithAttribute.Where(x => x.ReturnType == typeof(IEnumerator)).ToArray();
        }

        private static IEnumerator Run(ITestExecutionContext testExecutionContext, MethodInfo methodInfo)
        {
            return (IEnumerator) Reflect.InvokeMethod(methodInfo, testExecutionContext.TestObject);
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
            
            var unityContext = (UnityTestExecutionContext) context;
            var state = unityContext.SetUpTearDownState;

            if (state == null)
            {
                // We do not expect a state to exist in playmode
                state = ScriptableObject.CreateInstance<EnumerableSetUpTearDownCommandState>();
            }

            while (state.NextSetUpStepIndex < SetUps.Length)
            {
                var methodInfo = SetUps[state.NextSetUpStepIndex];
                var enumerator = Run(context, methodInfo);
                SetEnumeratorPC(enumerator, state.NextSetUpStepPc);

                var logScope = new LogScope();

                while (true)
                {
                    try
                    {
                        if (!enumerator.MoveNext())
                        {
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        state.TestHasRun = true;
                        Debug.LogException(ex);
                        context.CurrentResult.SetResult(ResultState.Failure, ex.Message);
                        break;
                    }

                    state.NextSetUpStepPc = GetEnumeratorPC(enumerator);
                    yield return enumerator.Current;
                }

                if (logScope.AnyFailingLogs())
                {
                    state.TestHasRun = true;
                    context.CurrentResult.SetResult(ResultState.Failure);
                }

                logScope.Dispose();

                state.NextSetUpStepIndex++;
                state.NextSetUpStepPc = 0;
            }

            if (!state.TestHasRun)
            {
                if (innerCommand is IEnumerableTestMethodCommand)
                {
                    var executeEnumerable = ((IEnumerableTestMethodCommand) innerCommand).ExecuteEnumerable(context);
                    foreach (var iterator in executeEnumerable)
                    {
                        yield return iterator;
                    }
                }
                else
                {
                    context.CurrentResult = innerCommand.Execute(context);
                }

                state.TestHasRun = true;
            }

            if (!state.TestTearDownStarted)
            {
                state.CurrentTestResultStatus = context.CurrentResult.ResultState.Status;
                state.CurrentTestResultLabel = context.CurrentResult.ResultState.Label;
                state.CurrentTestResultSite = context.CurrentResult.ResultState.Site;
                state.CurrentTestMessage = context.CurrentResult.Message;
                state.CurrentTestStrackTrace = context.CurrentResult.StackTrace;
            }

            while (state.NextTearDownStepIndex < TearDowns.Length)
            {
                state.TestTearDownStarted = true;
                var methodInfo = TearDowns[state.NextTearDownStepIndex];
                var enumerator = Run(context, methodInfo);
                SetEnumeratorPC(enumerator, state.NextTearDownStepPc);

                var logScope = new LogScope();

                while (true)
                {
                    try
                    {
                        if (!enumerator.MoveNext())
                        {
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                        context.CurrentResult.SetResult(ResultState.Failure, ex.Message);
                        break;
                    }

                    state.NextTearDownStepPc = GetEnumeratorPC(enumerator);
                    yield return enumerator.Current;
                }

                if (logScope.AnyFailingLogs())
                {
                    state.TestHasRun = true;
                    context.CurrentResult.SetResult(ResultState.Failure);
                }

                logScope.Dispose();

                state.NextTearDownStepIndex++;
                state.NextTearDownStepPc = 0;
            }

            context.CurrentResult.SetResult(
                new ResultState(state.CurrentTestResultStatus, state.CurrentTestResultLabel,
                    state.CurrentTestResultSite), state.CurrentTestMessage, state.CurrentTestStrackTrace);
            state.Reset();
            if (PerformanceTest.Active.Failed)
                context.CurrentResult.SetResult(ResultState.Failure);
            PerformanceTest.EndTest(context.CurrentTest);
        }

        private static void SetEnumeratorPC(IEnumerator enumerator, int pc)
        {
            GetPCFieldInfo(enumerator).SetValue(enumerator, pc);
        }

        private static int GetEnumeratorPC(IEnumerator enumerator)
        {
            if (enumerator == null)
            {
                return 0;
            }

            return (int) GetPCFieldInfo(enumerator).GetValue(enumerator);
        }

        private static FieldInfo GetPCFieldInfo(IEnumerator enumerator)
        {
            var field = enumerator.GetType().GetField("$PC", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null) // Roslyn
                field = enumerator.GetType().GetField("<>1__state", BindingFlags.NonPublic | BindingFlags.Instance);

            return field;
        }
    }
#elif UNITY_2018_2_OR_NEWER
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