using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace Unity.PerformanceTesting
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PerformanceSettingsAttribute : Attribute
    {
        public bool enableGC;

        public PerformanceSettingsAttribute(bool enableGc)
        {
            this.enableGC = enableGc;
        }
    }
}