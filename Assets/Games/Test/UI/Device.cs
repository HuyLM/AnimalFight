using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Device {
    static bool? _iPhoneX;
    public static bool iPhoneX {
        get {
#if UNITY_EDITOR || UNITY_IPHONE
            if (_iPhoneX == null) {
                float iPhoneXAspect = 1125f / 2436;
                float thisAspect = (float)Mathf.Min(Screen.width, Screen.height) / Mathf.Max(Screen.width, Screen.height);
                _iPhoneX = Mathf.Abs(iPhoneXAspect - thisAspect) < 0.01f;
            }

            return _iPhoneX.Value;
#endif

            return false;
        }
    }

    static bool? isTablet;
    public static bool IsTablet {
        get {
            if (isTablet == null) {
#if UNITY_IPHONE || UNITY_EDITOR
                // Aspect based detection
                float iPadAspect = 768f / 1024;
                float thisAspect = (float)Mathf.Min(Screen.width, Screen.height) / Mathf.Max(Screen.width, Screen.height);
                isTablet = Mathf.Abs(iPadAspect - thisAspect) < 0.01f;
#else
                isTablet = UtilsAndroid.IsTablet ();
#endif
                Debug.LogFormat("Device Detected: IsTablet ? {0}", isTablet);
            }
            return isTablet.Value;
        }
    }
}

#if UNITY_ANDROID
public static class AndroidDeviceUtils
{

    public const int ORIENTATION_UNDEFINED = 0x00000000;
    public const int ORIENTATION_PORTRAIT = 0x00000001;
    public const int ORIENTATION_LANDSCAPE = 0x00000002;

    public const int ROTATION_0 = 0x00000000;
    public const int ROTATION_90 = 0x00000001;
    public const int ROTATION_180 = 0x00000002;
    public const int ROTATION_270 = 0x00000003;

    public const int PORTRAIT = 0;
    public const int PORTRAIT_UPSIDEDOWN = 1;
    public const int LANDSCAPE = 2;
    public const int LANDSCAPE_LEFT = 3;


    static AndroidJavaObject mConfig;
    static AndroidJavaObject mWindowManager;

    //adapted from http://stackoverflow.com/questions/4553650/how-to-check-device-natural-default-orientation-on-android-i-e-get-landscape/4555528#4555528
    public static int GetDeviceDefaultOrientation()
    {
        if ((mWindowManager == null) || (mConfig == null))
        {
            using (AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").
                            GetStatic<AndroidJavaObject>("currentActivity"))
            {
                mWindowManager = activity.Call<AndroidJavaObject>("getSystemService","window");
                mConfig = activity.Call<AndroidJavaObject>("getResources").Call<AndroidJavaObject>("getConfiguration");
            }
        }
        
        int lRotation = mWindowManager.Call<AndroidJavaObject>("getDefaultDisplay").Call<int>("getRotation");
        int dOrientation = mConfig.Get<int>("orientation");

        if( (((lRotation == ROTATION_0) || (lRotation == ROTATION_180)) && (dOrientation == ORIENTATION_LANDSCAPE)) ||
        (((lRotation == ROTATION_90) || (lRotation == ROTATION_270)) && (dOrientation == ORIENTATION_PORTRAIT)))
        {
            return(LANDSCAPE); //TABLET
        }     

        return (PORTRAIT); //PHONE
    } 

    public static bool IsTablet 
    {
        get { return GetDeviceDefaultOrientation() == LANDSCAPE; }
    }

}
#endif