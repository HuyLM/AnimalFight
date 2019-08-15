using Gemmob.Common.Data;
using System;
using UnityEngine;

namespace Gemmob.API.Ads {
    public class AdsSetting {
        #region Ads Test ID
        private static readonly string AndroidAppId = "ca-app-pub-3940256099942544~3347511713";
        private static readonly string IOSAppId = "ca-app-pub-3940256099942544~1458002511";

        private static readonly string BannerAndroidAdUnitId = "ca-app-pub-3940256099942544/6300978111";
        private static readonly string BannerIOSAdUnitId = "ca-app-pub-3940256099942544/2934735716";

        private static readonly string InterstitialAndroidAdUnitId = "ca-app-pub-3940256099942544/1033173712";
        private static readonly string InterstitialIOSAdUnitId = "ca-app-pub-3940256099942544/4411468910";

        private static readonly string RewardedAndroidAdUnitId = "ca-app-pub-3940256099942544/5224354917";
        private static readonly string RewardedIOSAdUnitId = "ca-app-pub-3940256099942544/1712485313";
        #endregion

        public const string IosConfigFileName = "GemmobAdsIosConfig";
        public const string AndroidConfigFileName = "GemmobAdsAndroidConfig";

        private const string AdsConfigKey = "GemmobAds";
        private const string ConfigFileFolder = "Assets/Resources/";

        public static Config.AdmobInfo AndroidAdmobInfo {
            get {
                return new Config.AdmobInfo(AndroidAppId, BannerAndroidAdUnitId, InterstitialAndroidAdUnitId, RewardedAndroidAdUnitId);
            }
        }

        public static Config.AdmobInfo IosAdmobInfo {
            get {
                return new Config.AdmobInfo(IOSAppId, BannerIOSAdUnitId, InterstitialIOSAdUnitId, RewardedIOSAdUnitId);
            }
        }

        public static Config.ApiInfo LoadIosConfigFromResouceFolder() {
            return LoadConfigFromResouceFolder(IosConfigFileName);
        }

        public static Config.ApiInfo LoadAndroidConfigFromResouceFolder() {
            return LoadConfigFromResouceFolder(AndroidConfigFileName);
        }

        public static Config.ApiInfo LoadConfigFromResouceFolder(string fileName) {
            try {
                Debug.Log(fileName);
                var textAsset = Resources.Load<TextAsset>(fileName);
                var decryptString = Encryption.DecryptString(textAsset.text);
                var config = JsonUtility.FromJson<Config.ApiInfo>(decryptString);
                Logs.LogFormat("Load Config from json file {0}", fileName);
                return config;
            }
            catch (FormatException) {
                Logs.LogError("Invalid encrypt key, Please download again ads config");
                return null;
            }
            catch (Exception e) {
                Logs.LogErrorFormat("Can not load resource file {0} - {1}", fileName, e.Message);
                return null;
            }
        }

        public static Config.ApiInfo LoadLocalApiInfo() {
            try {
                var jsonText = SecurePlayerPrefs.GetString(AdsConfigKey);
                if (!string.IsNullOrEmpty(jsonText)) {
                    var config = JsonUtility.FromJson<Config.ApiInfo>(jsonText);
                    Logs.Log("Load Config from PlayerPrefs");
                    return config;
                }
                else {
                    var config = LoadConfigFromResouceFolder(GetConfigFileName());
                    SaveToLocalConfig(config);
                    return config;
                }
            }
            catch (Exception e) {
                Logs.LogErrorFormat("Can not load config from PlayerPrefs:  {0}", e.Message);
            }

            return LoadConfigFromResouceFolder(GetConfigFileName());
        }

        public static Config.AdmobInfo LoadLocalAdmobInfo() {

            if (GemmobAdsConfig.Instance.enableTest) {

#if UNITY_ANDROID
                return AndroidAdmobInfo;
#endif

#if UNITY_IOS
            return IosAdmobInfo;
#endif

            }

            return LoadLocalApiInfo().ads.admob;
        }

        private static string GetConfigFileName() {
#if UNITY_ANDROID
            return AndroidConfigFileName;
#endif

#if UNITY_IOS
        return IosConfigFileName;
#endif
        }

        private static void SaveToLocalConfig(Config.ApiInfo config) {
            try {
                SecurePlayerPrefs.SetString(AdsConfigKey, JsonUtility.ToJson(config));
            }
            catch (Exception e) {
                Logs.LogErrorFormat("Error when save ads config to local {0}", e.Message);
            }
        }

    }
}