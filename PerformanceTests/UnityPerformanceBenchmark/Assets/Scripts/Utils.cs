using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

public class Utils 
{

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    static extern float GetBatteryTempiOS();
#endif

  const int baselineBestOfCountPerIter     = 3;

 private static int[] memBuffer1 = new int[4000000]; // 16MB to counter iPhone X cache (8MB)
    private static int[] memBuffer2 = new int[4000000]; // 16MB to counter iPhone X cache (8MB)
    private static Vector2 systemRes = Vector2.zero;

  public static Vector2 GetSystemRes()
   {
      // We need to store res manually on first call as Display.main.systemWidth doesn't preserve original value
     // after SetResolution call
        if (systemRes.x < 0.1f)
        {
          systemRes = new Vector2(Screen.width, Screen.height);
      }

     return systemRes;
  }

 public static Matrix4x4 GetScaleMatrixForGUI()
 {
      float scale = Screen.dpi / 150f;

      if (scale < 1.0f)
          scale = 1.0f;

     scale *= Screen.width / GetSystemRes().x;

     return Matrix4x4.TRS(new Vector3(scale, scale, 0), Quaternion.identity, scale * Vector3.one);
  }

    public static double GetDeltaInSeconds(long tickstart)
    {
        return (DateTime.Now.Ticks - tickstart) / 10e6;
    }

    const float microTestTarget = 0.002f;
    static bool microBenchmarkCalibrated = false;
    static int microBenchmarkBusyLoopCount = 1000000;

    static void CalibrateMicroBenchmark()
    {
        if (microBenchmarkCalibrated)
            return;

        long starttime = DateTime.Now.Ticks;
        Vector3 one = Vector3.one;
        Vector3 v = Vector3.zero;
        for (int i = 0; i < microBenchmarkBusyLoopCount; i++)
        {
            v = v + one * 0.001f;
        }
        double duration = GetDeltaInSeconds(starttime);

        if (duration > microTestTarget)
            microBenchmarkBusyLoopCount = (int)(microBenchmarkBusyLoopCount * microTestTarget / duration);

        microBenchmarkCalibrated = true;
    }

    public static float GetMicroBenchmarkScore()
   {
        CalibrateMicroBenchmark();

        int busyLoopCount = microBenchmarkBusyLoopCount;
     int bestOfCount = baselineBestOfCountPerIter;
      Vector3 one = Vector3.one;

        double bestResult = double.MaxValue;
        for(int c = 0; c < bestOfCount; c++)
        {
            Vector3 v = Vector3.zero;
            long starttime = DateTime.Now.Ticks;
            for (int i = 0; i < busyLoopCount; i++)
            {
                v = v + one * 0.001f;
            }
            double result = GetDeltaInSeconds(starttime);
            if(result < bestResult)
            {
                bestResult = result;
            }
        }
      return (float)(busyLoopCount / bestResult) * 0.0166666f;
    }

    static bool memBenchmarkCalibrated = false;
    static int memBenchmarkBusyLoopCount = 3000000;

    static void CalibrateMemBenchmark()
    {
        if (memBenchmarkCalibrated)
            return;

        int len = memBuffer1.Length;

        long starttime = DateTime.Now.Ticks;
        for (uint i = 0; i < memBenchmarkBusyLoopCount; i++)
        {
            // use 128B stride to counter iPhone cache
            memBuffer1[(i << 5) % len] = memBuffer2[(i << 5) % len];
        }
        double duration = GetDeltaInSeconds(starttime);

        if (duration > microTestTarget)
            memBenchmarkBusyLoopCount = (int)(memBenchmarkBusyLoopCount * microTestTarget / duration);

        memBenchmarkCalibrated = true;
    }

 public static float GetMemBenchmarkScore()
 {
        CalibrateMemBenchmark();

        int bestOfCount = baselineBestOfCountPerIter;
        int busyLoopCount = memBenchmarkBusyLoopCount;
       int len = memBuffer1.Length;


     double bestResult = double.MaxValue;
        for(int c = 0; c < bestOfCount; c++)
        {
            long starttime = DateTime.Now.Ticks;        
           for (uint i = 0; i < busyLoopCount; i++)
            {
             // use 128B stride to counter iPhone cache
             memBuffer1[(i<<5) % len] = memBuffer2[(i<<5) % len];
            }           
            double result = GetDeltaInSeconds(starttime);
            if(result < bestResult)
            {
                bestResult = result;
            }
        }
     return (float)(busyLoopCount / bestResult) * 0.0166666f;
    }

#if UNITY_ANDROID && !UNITY_EDITOR
 private static AndroidJavaObject _androidActivity;
 private static AndroidJavaObject GetAndroidActivity()
  {
      if (_androidActivity == null)
      {
          var actClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
         _androidActivity = actClass.GetStatic<AndroidJavaObject>("currentActivity");
       }

     return _androidActivity;
   }

 private static AndroidJavaObject _androidBatteryManager;
   private static AndroidJavaObject GetAndroidBatteryManager()
    {
      if (_androidBatteryManager == null)
        {
          AndroidJavaObject androidActivity = GetAndroidActivity();
          AndroidJavaObject context = androidActivity.Call<AndroidJavaObject>("getApplicationContext");
          AndroidJavaClass contextClass = new AndroidJavaClass("android.content.Context");
           string Context_BATTERY_SERVICE = contextClass.GetStatic<string>("BATTERY_SERVICE");
            _androidBatteryManager = context.Call<AndroidJavaObject>("getSystemService", Context_BATTERY_SERVICE);
     }

     return _androidBatteryManager;
 }


    public static AndroidJavaObject GetAndroidIntent()
    {
       AndroidJavaObject androidActivity = GetAndroidActivity();
      AndroidJavaObject context = androidActivity.Call<AndroidJavaObject>("getApplicationContext");
      AndroidJavaObject intentFilter = new AndroidJavaObject("android.content.IntentFilter", "android.intent.action.BATTERY_CHANGED");
           
       return context.Call<AndroidJavaObject>("registerReceiver", null, intentFilter);
    }
#endif

  public static void SetSustainedMode(bool sustainedMode)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
       AndroidJavaObject androidActivity = GetAndroidActivity();

     AndroidJavaObject window = androidActivity.Call<AndroidJavaObject>("getWindow");


     androidActivity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
              window.Call("setSustainedPerformanceMode", sustainedMode);
                 Debug.Log("Set sustained performance mode: " + (sustainedMode ? "ON" : "OFF"));}));
#endif
 }


    public static float GetBatteryCurrent()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
       AndroidJavaObject batteryManager = GetAndroidBatteryManager();
     return batteryManager.Call<long>("getLongProperty", 2); // current now
#else
       return 0f;
#endif
  }

 public static float GetBatteryTemp()
   {
#if UNITY_ANDROID && !UNITY_EDITOR
       AndroidJavaObject intent = GetAndroidIntent();
     float temp = intent.Call<int>("getIntExtra", "temperature", 0) / 10.0f; // temp now
        return temp;
#elif UNITY_IOS && !UNITY_EDITOR
        return GetBatteryTempiOS();
#else
        return 0f;
#endif
  }

 public static bool HasDeviceOverheated()
   {
      float temp = GetBatteryTemp();

        if (Application.platform == RuntimePlatform.Android)
           return temp > 28.5f;

      if (Application.platform == RuntimePlatform.IPhonePlayer)
          return temp > 0.0f;

       return false;
  }

 public static void SwitchRes(bool limitTo, int resX)
   {
      Vector2 sysRes = GetSystemRes();

      if (limitTo)
       {
          if (Screen.width > resX)
           {
                Screen.SetResolution(resX, Screen.height * resX / Screen.width, Screen.fullScreen);
          }
      }
      else
       {
          if (Screen.width < sysRes.x)
           {
              Screen.SetResolution((int)sysRes.x, (int)sysRes.y, Screen.fullScreen);
         }
      }
  }
}

