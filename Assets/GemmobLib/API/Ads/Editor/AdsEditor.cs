//using GoogleMobileAds.Editor;
using GoogleMobileAds.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using FileMode = System.IO.FileMode;
using Gemmob.API.Ads;

namespace Gemmob.EditorTools {
    internal class AdsEditor : EditorWindow {
        public class ConfigViewer : ScriptableObject {
            public Config.ApiInfo iosConfig;
            public Config.ApiInfo androidConfig;
        }

        private static readonly string[] labels = { "Android Config", "iOs Config" };
        private int currentTab;

        private const string ConfigFileFolder = "Assets/Resources/";


        [MenuItem("GEM Tools/Gemmob Ads", false, 10)]
        public static void OpenLevelEditorWindow() {
            GetWindow<AdsEditor>("GEM ADS CONFIG").Show();
            //GetWindowWithRect(typeof(AdsEditor), new Rect(0, 0, 500, 400), true,
            //    "Gemmob Ads Tool");
        }


        private SerializedObject serializedObject;

        private GemmobAdsConfig adsConfig;
        private SerializedObject configViewerSerializedObject;
        private ConfigViewer configViewer;
        private Vector2 scroller;

        private Config.ApiInfo Download(string url) {
            EditorUtility.DisplayCancelableProgressBar("Download", "Downloading...", 0);
            Logs.LogFormat("[ADS] request download url : {0} ", url);
            var www = UnityWebRequest.Get(url);
            www.SendWebRequest();

            while (!www.isDone) {
                var progress = www.downloadProgress;
                if (EditorUtility.DisplayCancelableProgressBar("Download", "Downloading...", progress)) {
                    return null;
                }
            }

            EditorUtility.ClearProgressBar();
            if (www.error != null) {
                ShowNotification(new GUIContent(string.Format("Error when download url : {0}, error: {1}", url,
                    www.error)));
                return null;
            }

            var text = www.downloadHandler.text;
            try {
                return JsonUtility.FromJson<Config.ApiInfo>(text);
            } catch (Exception) {
                ShowNotification(new GUIContent(string.Format("Canot parse json  from  download  url: {0}", url)));
                return null;
            }
        }

        private static GemmobAdsConfig LoadOrCreateAdsConfig() {
            if (!Directory.Exists(ConfigFileFolder)) {
                Directory.CreateDirectory(ConfigFileFolder);
            }

            var adsConfig = LoadAdsConfigResouce();
            if (adsConfig == null) {
                adsConfig = CreateInstance<GemmobAdsConfig>();
                AssetDatabase.CreateAsset(adsConfig, ConfigFileFolder + GemmobAdsConfig.ResourceName + ".asset");
                AssetDatabase.SaveAssets();
            }

            return adsConfig;
        }

        public static GemmobAdsConfig LoadAdsConfigResouce() {
            return Resources.Load<GemmobAdsConfig>(GemmobAdsConfig.ResourceName);
        }

        private void ReloadAdsConfigViewer(GemmobAdsConfig gemmobAdsConfig) {
            configViewer = CreateInstance<ConfigViewer>();
            configViewer.androidConfig = null;
            configViewer.iosConfig = null;


            if (gemmobAdsConfig.enableAndroid) {
                configViewer.androidConfig = AdsSetting.LoadAndroidConfigFromResouceFolder();
            }

            if (gemmobAdsConfig.enableIos) {
                configViewer.iosConfig = AdsSetting.LoadIosConfigFromResouceFolder();
            }

            configViewerSerializedObject = new SerializedObject(configViewer);
        }

        private void OnEnable() {
            adsConfig = LoadOrCreateAdsConfig();
            serializedObject = new SerializedObject(adsConfig);

            ReloadAdsConfigViewer(adsConfig);
        }

        void OnGUI() {
            if (serializedObject != null) {
                scroller = GUILayout.BeginScrollView(scroller);
                EditorGUILayout.BeginHorizontal();

                var serializedProperty = serializedObject.FindProperty("enableAndroid");
                EditorGUILayout.PropertyField(serializedProperty, new GUIContent("Android"));
                var enableAndroid = serializedProperty.boolValue;
                EditorGUI.BeginDisabledGroup(!enableAndroid);
                var androidApiLink = serializedObject.FindProperty("androidApiLink");
                EditorGUILayout.PropertyField(androidApiLink, GUIContent.none);
                if (GUILayout.Button("Open", GUILayout.Width(60))) {
                    Application.OpenURL(androidApiLink.stringValue);
                }

                EditorGUI.EndDisabledGroup();

                EditorGUILayout.EndHorizontal();


                EditorGUILayout.BeginHorizontal();
                var findProperty = serializedObject.FindProperty("enableIos");
                EditorGUILayout.PropertyField(findProperty, new GUIContent("IOS"));
                var enableIos = findProperty.boolValue;


                EditorGUI.BeginDisabledGroup(!enableIos);

                var iosApiLink = serializedObject.FindProperty("iosApiLink");
                EditorGUILayout.PropertyField(iosApiLink, GUIContent.none);
                if (GUILayout.Button("Open", GUILayout.Width(60))) {
                    Application.OpenURL(iosApiLink.stringValue);
                }

                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Use Banner ", GUILayout.Width(150));
                int option = GemmobAdsConfig.Instance.useBannerTop ? 1 : GemmobAdsConfig.Instance.useBannerBottom ? 2 : 0 ;
                option = EditorGUILayout.IntPopup(option, new string[] { "Not use", "Top", "Bottom" }, new int[] { 0, 1, 2}, GUILayout.Width(150));
                GemmobAdsConfig.Instance.useBannerTop = option == 1;
                GemmobAdsConfig.Instance.useBannerBottom = option == 2;
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("useInterstitial"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("useRewardVideo"));
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("enableTest"));

                if (GUILayout.Button("Download Api Config")) {
                    if (enableIos && string.IsNullOrEmpty(iosApiLink.stringValue)) {
                        ShowNotification(new GUIContent("Please set IOS API link"));
                        return;
                    }

                    if (enableAndroid && string.IsNullOrEmpty(androidApiLink.stringValue)) {
                        ShowNotification(new GUIContent("Please set Android API link"));
                        return;
                    }

                    SaveAdsConfig(adsConfig);
                    ReloadAdsConfigViewer(adsConfig);
                }

                if (GUILayout.Button("Save")) {
                    if (enableIos && string.IsNullOrEmpty(iosApiLink.stringValue)) {
                        ShowNotification(new GUIContent("Please set IOS API link"));
                        return;
                    }

                    if (enableAndroid && string.IsNullOrEmpty(androidApiLink.stringValue)) {
                        ShowNotification(new GUIContent("Please set Android API link"));
                        return;
                    }

                    GoogleMobileAdsSettings.Instance.AdMobAndroidAppId = GemmobAdsConfig.Instance.enableTest ? AdsSetting.AndroidAdmobInfo.admob_id : configViewer.androidConfig.ads.admob.admob_id;
                    GoogleMobileAdsSettings.Instance.AdMobIOSAppId = GemmobAdsConfig.Instance.enableTest ? AdsSetting.IosAdmobInfo.admob_id : configViewer.iosConfig.ads.admob.admob_id;
                    GoogleMobileAdsSettings.Instance.WriteSettingsToFile();

                    ValidUnityAdsScriptingDefine(adsConfig, ScriptingDefine.GetBuildTargetGroup());
                    AssetDatabase.SaveAssets();
                }

                serializedObject.ApplyModifiedProperties();
                currentTab = GUILayout.Toolbar(currentTab, labels);
                switch (currentTab) {
                    case 0:
                        var androidConfigProperty = configViewerSerializedObject.FindProperty("androidConfig");
                        if (androidConfigProperty != null && enableAndroid) {
                            EditorGUILayout.PropertyField(androidConfigProperty, true);
                        } else {
                            EditorGUILayout.LabelField(new GUIContent("Please enable Android ads config"));
                        }

                        break;
                    case 1:
                        var iosConfigProperty = configViewerSerializedObject.FindProperty("iosConfig");
                        if (iosConfigProperty != null && enableIos) {
                            EditorGUILayout.PropertyField(iosConfigProperty, true);
                        } else {
                            EditorGUILayout.LabelField(new GUIContent("Please enable Ios ads config"));
                        }

                        break;
                }

                EditorGUI.EndDisabledGroup();


                GUILayout.EndScrollView();
            }
        }

        private static void WriteConfigFileToResouceFolder(Config.ApiInfo config, string fileName) {
            Assert.IsNotNull(config);
            if (!Directory.Exists(ConfigFileFolder)) {
                Logs.LogFormat("Create folder {0}", ConfigFileFolder);
                Directory.CreateDirectory(ConfigFileFolder);
            }

            var path = ConfigFileFolder + fileName + ".txt";
            using (FileStream fs = new FileStream(path,
                FileMode.Create)) {
                using (StreamWriter writer = new StreamWriter(fs)) {
                    writer.Write(Gemmob.Common.Data.Encryption.EncryptString(JsonUtility.ToJson(config)));
                }

                AssetDatabase.Refresh();
                Logs.Log("Save config to " + path);
            }
        }

        public static void ValidUnityAdsScriptingDefine(GemmobAdsConfig adsConfig, BuildTargetGroup buildTargetGroup) {
            if (adsConfig != null) {
                string defineBuild = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
                var defines = new List<string>(defineBuild.Split(';'));

                ScriptingDefine.EnableScriptingDefineFlag(GemmobAdsConfig.Instance.useBannerTop, Mediation.BannerTopCondition, defines);
                ScriptingDefine.EnableScriptingDefineFlag(GemmobAdsConfig.Instance.useBannerBottom, Mediation.BannerBottomCondition, defines);
                ScriptingDefine.EnableScriptingDefineFlag(GemmobAdsConfig.Instance.useInterstitial, Mediation.InterstitialCondition, defines);
                ScriptingDefine.EnableScriptingDefineFlag(GemmobAdsConfig.Instance.useRewardVideo, Mediation.RewardedCondition, defines);

                ScriptingDefine.SaveScriptingDefineSymbolsForGroup(buildTargetGroup, defines.ToArray());
            } else {
                Logs.LogError("Ads Config is null please check");
            }
        }

        public void SaveAdsConfig(GemmobAdsConfig adsConfig) {
            ValidUnityAdsScriptingDefine(adsConfig, ScriptingDefine.GetBuildTargetGroup());


            var androidApiLink = adsConfig.androidApiLink;
            var iosApiLink = adsConfig.iosApiLink;
            if (adsConfig.enableAndroid && !string.IsNullOrEmpty(androidApiLink)) {
                var config = Download(androidApiLink);
                if (config != null) {
                    SetPlayerSetingBuilTargetGroup(config, BuildTargetGroup.Android);
                    WriteConfigFileToResouceFolder(config, AdsSetting.AndroidConfigFileName);
                }
            }

            if (adsConfig.enableIos && !string.IsNullOrEmpty(iosApiLink)) {
                var config = Download(iosApiLink);
                if (config != null) {
                    SetPlayerSetingBuilTargetGroup(config, BuildTargetGroup.iOS);
                    WriteConfigFileToResouceFolder(config, AdsSetting.IosConfigFileName);
                }
            }
        }

        public static void SetPlayerSetingBuilTargetGroup(Config.ApiInfo Infor, BuildTargetGroup buildTargetGroup) {
            if (Infor != null) {
                if (!PlayerSettings.strippingLevel.Equals(StrippingLevel.StripAssemblies)) {
                    PlayerSettings.strippingLevel = StrippingLevel.StripAssemblies;
                }


                if (!PlayerSettings.companyName.Equals(Infor.info.name)) {
                    PlayerSettings.companyName = Infor.info.name;
                }

                if (!PlayerSettings.productName.Equals(Infor.info.name)) {
                    PlayerSettings.productName = Infor.info.name;
                }

                if (!PlayerSettings.applicationIdentifier.Equals(Infor.info.package)) {
                    PlayerSettings.applicationIdentifier = Infor.info.package;
                }

                if (!PlayerSettings.GetApplicationIdentifier(buildTargetGroup).Equals(Infor.info.package)) {
                    PlayerSettings.SetApplicationIdentifier(buildTargetGroup,
                        Infor.info.package);
                }


                if (buildTargetGroup == BuildTargetGroup.Android) {
                    PlayerSettings.Android.forceSDCardPermission = true;
                    if (!AndroidSdkVersions.AndroidApiLevel21.Equals(PlayerSettings.Android.minSdkVersion)) {
                        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel21;
                    }
                }
            }
        }
    }
}