using System;
using System.Collections.Generic;
using System.Text;
using Unity.PerformanceTesting;
using Unity.PerformanceTesting.Exceptions;
using UnityEngine;
#if ENABLE_VR
using UnityEngine.XR;

#endif

namespace Unity.PerformanceTesting.Runtime
{
    public static class Utils
    {
        public static readonly Guid k_sendTestDataToEditor = new Guid("f4920df8-1fd9-4e70-9e43-cfa29f0604f3");
        public static readonly Guid k_sendPlayerDataToEditor = new Guid("da0678cb-d501-42ec-84ed-33f5e4b9f1fe");

        public static double DateToInt(DateTime date)
        {
            return date.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                .TotalMilliseconds;
        }

        public static double ConvertSample(SampleUnit from, SampleUnit to, double value)
        {
            double f = RelativeSampleUnit(@from);
            double t = RelativeSampleUnit(to);

            return value * (t / f);
        }

        public static double RelativeSampleUnit(SampleUnit unit)
        {
            switch (unit)
            {
                case SampleUnit.Nanosecond:
                    return 1000000;
                case SampleUnit.Microsecond:
                    return 1000;
                case SampleUnit.Millisecond:
                    return 1;
                case SampleUnit.Second:
                    return 0.001;
                default:
                    throw new PerformanceTestException(
                        "Wrong SampleUnit type used. Are you trying to convert between time and size units?");
            }
        }

        public static int GetZeroValueCount(List<double> samples)
        {
            var zeroValues = 0;
            foreach (var sample in samples)
            {
                if (Math.Abs(sample) < .0001f)
                {
                    zeroValues++;
                }
            }

            return zeroValues;
        }

        public static double GetMedianValue(List<double> samples)
        {
            var samplesClone = new List<double>(samples);
            samplesClone.Sort();

            var middleIdx = samplesClone.Count / 2;
            return samplesClone[middleIdx];
        }

        public static double GetPercentile(List<double> samples, double percentile)
        {
            if (percentile < 0.00001D)
                return percentile;

            var samplesClone = new List<double>(samples);
            samplesClone.Sort();

            if (samplesClone.Count == 1)
            {
                return samplesClone[0];
            }

            var rank = percentile * (samplesClone.Count + 1);
            var integral = (int) rank;
            var fractional = rank % 1;
            return samplesClone[integral - 1] + fractional * (samplesClone[integral] - samplesClone[integral - 1]);
        }

        public static double GetStandardDeviation(List<double> samples, double average)
        {
            double sumOfSquaresOfDifferences = 0.0D;
            foreach (var sample in samples)
            {
                sumOfSquaresOfDifferences += (sample - average) * (sample - average);
            }

            return Math.Sqrt(sumOfSquaresOfDifferences / samples.Count);
        }

        public static double Min(List<double> samples)
        {
            double min = Mathf.Infinity;
            foreach (var sample in samples)
            {
                if (sample < min) min = sample;
            }

            return min;
        }

        public static double Max(List<double> samples)
        {
            double max = Mathf.NegativeInfinity;
            foreach (var sample in samples)
            {
                if (sample > max) max = sample;
            }

            return max;
        }

        public static double Average(List<double> samples)
        {
            return Sum(samples) / samples.Count;
        }

        public static double Sum(List<double> samples)
        {
            double sum = 0.0D;
            foreach (var sample in samples)
            {
                sum += sample;
            }

            return sum;
        }

        public static PlayerSystemInfo GetSystemInfo()
        {
            return new PlayerSystemInfo
            {
                OperatingSystem = SystemInfo.operatingSystem,
                DeviceModel = SystemInfo.deviceModel,
                DeviceName = SystemInfo.deviceName,
                ProcessorType = SystemInfo.processorType,
                ProcessorCount = SystemInfo.processorCount,
                XrModel = XRDevice.model,
                GraphicsDeviceName = SystemInfo.graphicsDeviceName,
                SystemMemorySize = SystemInfo.systemMemorySize,
#if ENABLE_VR
                XrDevice = XRSettings.loadedDeviceName
#endif
            };
        }

        public static Unity.PerformanceTesting.QualitySettings GetQualitySettings()
        {
            return new Unity.PerformanceTesting.QualitySettings
            {
                Vsync = UnityEngine.QualitySettings.vSyncCount,
                AntiAliasing = UnityEngine.QualitySettings.antiAliasing,
                ColorSpace = UnityEngine.QualitySettings.activeColorSpace.ToString(),
                AnisotropicFiltering = UnityEngine.QualitySettings.anisotropicFiltering.ToString(),
                BlendWeights = UnityEngine.QualitySettings.blendWeights.ToString()
            };
        }

        public static ScreenSettings GetScreenSettings()
        {
            return new ScreenSettings
            {
                ScreenRefreshRate = Screen.currentResolution.refreshRate,
                ScreenWidth = Screen.currentResolution.width,
                ScreenHeight = Screen.currentResolution.height,
                Fullscreen = Screen.fullScreen
            };
        }

        public static string GetArg(string name)
        {
            var args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == name && args.Length > i + 1)
                {
                    return args[i + 1];
                }
            }

            return null;
        }

        public static bool VerifyTestRunMetadata(PerformanceTestRun run)
        {
            List<String> errors = new List<string>();

            if (run.TestSuite != "Playmode" && run.TestSuite != "Editmode") errors.Add("TestSuite");
            if (run.StartTime < 1.0D) errors.Add("StartTime");
            if (run.EndTime < 1.0D) errors.Add("EndTime");

            if (run.BuildSettings == null) errors.Add("BuildSettings");
            else
            {
                if (run.BuildSettings.BuildTarget.Length == 0) errors.Add("BuildSettings.BuildTarget");
                if (run.BuildSettings.Platform.Length == 0) errors.Add("BuildSettings.Platform");
            }

            if (run.EditorVersion == null) errors.Add("EditorVersion");
            else
            {
                if (run.EditorVersion.DateSeconds == 0) errors.Add("EditorVersion.DateSeconds");
                if (run.EditorVersion.FullVersion.Length == 0) errors.Add("EditorVersion.FullVersion");
                if (run.EditorVersion.Branch.Length == 0) errors.Add("EditorVersion.Branch");
                if (run.EditorVersion.RevisionValue == 0) errors.Add("EditorVersion.RevisionValue");
            }

            if (run.PlayerSettings == null) errors.Add("Performance test has its build settings unassigned.");
            else
            {
                if (run.PlayerSettings.ScriptingBackend.Length == 0) errors.Add("PlayerSettings.ScriptingBackend");
                if (run.PlayerSettings.GraphicsApi.Length == 0) errors.Add("PlayerSettings.GraphicsAp");
                if (run.PlayerSettings.Batchmode.Length == 0) errors.Add("PlayerSettings.Batchmode");
            }

            if (run.PlayerSystemInfo == null) errors.Add("Performance test has its build settings unassigned.");
            else
            {
                if (run.PlayerSystemInfo.ProcessorCount == 0) errors.Add("PlayerSystemInfo.ProcessorCount");
                if (run.PlayerSystemInfo.OperatingSystem.Length == 0) errors.Add("PlayerSystemInfo.OperatingSystem");
                if (run.PlayerSystemInfo.ProcessorType.Length == 0) errors.Add("PlayerSystemInfo.ProcessorType");
                if (run.PlayerSystemInfo.GraphicsDeviceName.Length == 0)errors.Add("PlayerSystemInfo.GraphicsDeviceName");
                if (run.PlayerSystemInfo.SystemMemorySize == 0) errors.Add("PlayerSystemInfo.SystemMemorySize");
            }

            if (run.QualitySettings == null) errors.Add("Performance test has its build settings unassigned.");
            else
            {
                if (run.QualitySettings.ColorSpace.Length == 0) errors.Add("QualitySettings.ColorSpace");
                if (run.QualitySettings.BlendWeights.Length == 0) errors.Add("QualitySettings.BlendWeights");
                if (run.QualitySettings.AnisotropicFiltering.Length == 0) errors.Add("QualitySettings.AnisotropicFiltering");
            }

            if (run.ScreenSettings == null) errors.Add("ScreenSettings");

            if (errors.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var error in errors)
                {
                    sb.Append(error + ", ");
                }
                
                Debug.LogError("Performance run has missing metadata. Please report this as a bug on #devs-performance. The following fields have not been set: " + sb);
                return false;
            }

            return true;
        }
    }
}