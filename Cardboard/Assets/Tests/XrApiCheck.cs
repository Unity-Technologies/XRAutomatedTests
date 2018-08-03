using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.XR;

namespace Tests
{
    public class XrApiCheck : CardboardSetup
    {
        [Test]
        public void MobilePlatformCheck()
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                Assert.IsTrue(Application.isMobilePlatform, "Cardboard returned as a non mobile platform ");
            }
            else
            {
                Assert.IsFalse(Application.isMobilePlatform, "Standalone XR returned as a mobile platform");
            }
        }

        [Test]
        public void XrPresentCheck()
        {
            Assert.IsTrue(XRDevice.isPresent, "XR Device is not present");
        }

        [Test]
        public void UserPresenceCheck()
        {
            if (XRDevice.userPresence == UserPresenceState.Present)
            {
                Assert.AreEqual(UserPresenceState.Present, XRDevice.userPresence, "User Presence reported reported unexpected value");
            }

            if (XRDevice.userPresence == UserPresenceState.NotPresent)
            {
                Assert.AreEqual(UserPresenceState.NotPresent, XRDevice.userPresence, "User Presence reported reported unexpected value");
            }
        }

        [Test]
        public void XrSettingsCheck()
        {
            Assert.IsTrue(XRSettings.isDeviceActive, "XR Device is not active");
        }

        [Test]
        public void DeviceCheck()
        {
            Assert.AreEqual("cardboard", XRSettings.loadedDeviceName, "Wrong XR Device reported");
        }

        [Test]
        public void XrModel()
        {
            string model = XRDevice.model;
            Assert.IsNotEmpty(model, "Model is empty");
        }

        [Test]
        public void NativePtr()
        {
            string ptr = XRDevice.GetNativePtr().ToString();
            Assert.IsNotEmpty(ptr, "Native Ptr is empty");
        }

        [Test]
        public void RefreshRate()
        {
            float refreshRate = XRDevice.refreshRate;
            Assert.AreNotEqual(refreshRate, 0, "Refresh is 0, something went wrong");
        }
    }
}
