using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.PerformanceTesting.Runtime;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using Unity.PerformanceTesting.Exceptions;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.TestRunner.NUnitExtensions;

namespace Unity.PerformanceTesting
{
    [Serializable]
    public class PerformanceTest
    {
        // Serialized fields
        public string TestName;
        public List<string> TestCategories = new List<string>();
        public string TestVersion;
        public double StartTime;
        public double EndTime;
        public List<SampleGroup> SampleGroups = new List<SampleGroup>();

        public static PerformanceTest Active { get; private set; }
        internal static List<IDisposable> Disposables = new List<IDisposable>(1024);

        public delegate void Callback();

        public static Callback OnTestEnded;
        internal bool Failed;

        public PerformanceTest()
        {
            Active = this;
        }

        internal static void StartTest(ITest currentTest)
        {
            if (currentTest.IsSuite) return;
            Active = new PerformanceTest
            {
                TestName = currentTest.FullName,
                TestCategories = currentTest.GetAllCategoriesFromTest(),
                TestVersion = GetVersion(currentTest)
            };

            Active.StartTime = Utils.DateToInt(DateTime.Now);
        }

        private static string GetVersion(ITest currentTest)
        {
            string version = "";
            var methodVersions = currentTest.Method.GetCustomAttributes<VersionAttribute>(false);
            var classVersion = currentTest.TypeInfo.Type.GetCustomAttributes(typeof(VersionAttribute), true);

            if (classVersion.Length > 0)
                version = ((VersionAttribute) classVersion[0]).Version+".";
            if (methodVersions.Length > 0)
                version += methodVersions[0].Version;
            else
                version += "1";

            return version;
        }

        internal static void EndTest(Test test)
        {
            if (test.IsSuite) return;
            if (test.FullName != Active.TestName) return;

            DisposeMeasurements();
            Active.CalculateStatisticalValues();
            Active.EndTime = Utils.DateToInt(DateTime.Now);
            if (OnTestEnded != null) OnTestEnded();
            Active.LogOutput();

            TestContext.Out.WriteLine("##performancetestresult:" + JsonUtility.ToJson(Active));
            Active = null;
            GC.Collect();
        }
        
        private static void DisposeMeasurements()
        {
            for (var i = 0; i < Disposables.Count; i++)
            {
                Disposables[i].Dispose();
            }

            Disposables.Clear();
        }

        public static SampleGroup GetSampleGroup(SampleGroupDefinition definition)
        {
            foreach (var sampleGroup in Active.SampleGroups)
            {
                if (sampleGroup.Definition.Name == definition.Name)
                    return sampleGroup;
            }

            return null;
        }

        public static void Compare(SampleGroupDefinition group, SampleGroupDefinition group2, float percentage)
        {
            Compare(group.Name, group2.Name, percentage);
        }

        public static void Compare(string oldGroup, string newGroup, float percentage)
        {
            var group = Active.SampleGroups.Find(g => g.Definition.Name == oldGroup);
            var group2 = Active.SampleGroups.Find(g => g.Definition.Name == newGroup);
            if (group == null || group2 == null)
            {
                throw new PerformanceTestException("At leat one of the provided sample groups is null.");
            }

            if (group.Samples.Count == 0 || group2.Samples.Count == 0)
            {
                throw new PerformanceTestException("At least on of the provided sample groups has no values.");
            }

            CalculateStatisticalValue(group);
            CalculateStatisticalValue(group2);

            var from = GetAggregationValue(group);
            var to = GetAggregationValue(group2);

            var diff = (to - from) / from;

            if (group.Definition.IncreaseIsBetter && group2.Definition.IncreaseIsBetter)
            {
                diff = diff * -1;
            }
            else if (group.Definition.IncreaseIsBetter || group2.Definition.IncreaseIsBetter)
            {
                throw new PerformanceTestException(
                    string.Format(
                        "Sample groups {0} and {1} have incompatible definitions. When comparing, sample groups should have a matching SampleGroupDefinition.IncreaseIsBetter value.",
                        group.Definition.Name, group2.Definition.Name));
            }

            if (diff > percentage)
            {
                TestContext.Out.Write(
                    "Test Failed with increase in time of {0:P2}\nOrigin {1:0.00} New {2:0.00} Allowed difference {3:P2}\n---\n\n",
                    diff, from, to, percentage);
                Active.Failed = true;
            }
            else
            {
                TestContext.Out.Write(
                    "Test Passed with difference in time of {0:P2}\nOrigin {1:0.00} New {2:0.00} Allowed increase {3:P2}\n---\n\n",
                    diff, from, to, percentage);
            }
        }

        public void CalculateStatisticalValues()
        {
            foreach (var sampleGroup in SampleGroups)
            {
                CalculateStatisticalValue(sampleGroup);
            }
        }

        private static void CalculateStatisticalValue(SampleGroup sampleGroup)
        {
            if (sampleGroup.Samples == null) return;
            var samples = sampleGroup.Samples;
            if (samples.Count < 2)
            {
                sampleGroup.Min = samples[0];
                sampleGroup.Max = samples[0];
                sampleGroup.Median = samples[0];
                sampleGroup.Average = samples[0];
                sampleGroup.PercentileValue = 0.0D;
                sampleGroup.Zeroes = Utils.GetZeroValueCount(samples);
                sampleGroup.SampleCount = sampleGroup.Samples.Count;
                sampleGroup.Sum = samples[0];
                sampleGroup.StandardDeviation = 0;
            }
            else
            {
                sampleGroup.Min = Utils.Min(samples);
                sampleGroup.Max = Utils.Max(samples);
                sampleGroup.Median = Utils.GetMedianValue(samples);
                sampleGroup.Average = Utils.Average(samples);
                sampleGroup.PercentileValue = Utils.GetPercentile(samples, sampleGroup.Definition.Percentile);
                sampleGroup.Zeroes = Utils.GetZeroValueCount(samples);
                sampleGroup.SampleCount = sampleGroup.Samples.Count;
                sampleGroup.Sum = Utils.Sum(samples);
                sampleGroup.StandardDeviation = Utils.GetStandardDeviation(samples, sampleGroup.Average);
            }
        }

        private void LogOutput()
        {
            TestContext.Out.WriteLine(ToString());
        }

        public override string ToString()
        {
            var logString = new StringBuilder();

            foreach (var sampleGroup in SampleGroups)
            {
                logString.Append(sampleGroup.Definition.Name);

                if (sampleGroup.Samples.Count == 1)
                {
                    logString.AppendFormat(" {0:0.00} {1}", sampleGroup.Samples[0],
                        sampleGroup.Definition.SampleUnit);
                }
                else
                {
                    logString.AppendFormat(
                        " {0} Median:{1:0.00} Min:{2:0.00} Max:{3:0.00} Avg:{4:0.00} Std:{5:0.00} Zeroes:{6} SampleCount: {7} Sum: {8:0.00}",
                        sampleGroup.Definition.SampleUnit, sampleGroup.Median, sampleGroup.Min, sampleGroup.Max,
                        sampleGroup.Average,
                        sampleGroup.StandardDeviation, sampleGroup.Zeroes, sampleGroup.SampleCount, sampleGroup.Sum
                    );
                }

                logString.Append("\n");
            }

            return logString.ToString();
        }

        private static double? GetAggregationValue(SampleGroup sampleGroup)
        {
            switch (sampleGroup.Definition.AggregationType)
            {
                case AggregationType.Average:
                    return sampleGroup.Average;
                case AggregationType.Min:
                    return sampleGroup.Min;
                case AggregationType.Max:
                    return sampleGroup.Max;
                case AggregationType.Median:
                    return sampleGroup.Median;
                case AggregationType.Percentile:
                    return sampleGroup.PercentileValue;
                default:
                    throw new ArgumentOutOfRangeException("sampleGroup");
            }
        }
    }
}