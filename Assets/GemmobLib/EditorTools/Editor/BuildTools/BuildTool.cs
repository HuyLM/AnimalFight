#if ADS_ENABLE
using Gemmob.API.Ads;
#endif
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Gemmob.EditorTools {
    internal class BuildTool {
        #region Config
        internal class OsBuild {
            public string machineName;
        }

        private const string BuildConfigFileName = "GemmobBuildConfig";

        private const string BuildConfigFolderName =
            BuildConfigFolder + "/" + BuildConfigFileName + ".asset";

        private const string BuildConfigFolder = "Assets/Editor Default Resources";
        public const string ProductionBuildString = "PRODUCTION_BUILD";
        public const string AdsEnableString = "ADS_ENABLE";
        public const string FirebaseEnableString = "FIREBASE_ENABLE";
        public const string IapEnableString = "IAP_ENABLE";

        internal static BuildConfig LoadOrCreateBuildConfig() {
            if (!Directory.Exists(BuildConfigFolder)) {
                Directory.CreateDirectory(BuildConfigFolder);
            }

            var buildConfig = (BuildConfig)EditorGUIUtility.Load(BuildConfigFileName + ".asset");
            if (buildConfig == null) {
                buildConfig = ScriptableObject.CreateInstance<BuildConfig>();
                buildConfig.developOptions = BuildOptions.CompressWithLz4;
                buildConfig.productionOptions = BuildOptions.StrictMode;
                buildConfig.AndroidIl2CppForRelease = true;
                buildConfig.iOsIl2CppForRelease = true;
                buildConfig.BuildVersion = "1.0.0";
                buildConfig.BuildVerCode = 100;
                buildConfig.KeyStorePass = "123123";
                buildConfig.iOSTargetDevice = iOSTargetDevice.iPhoneAndiPad;
                buildConfig.iOSSdkVersion = iOSSdkVersion.DeviceSDK;
                buildConfig.achitecture = BuildConfig.BuildAchietecture.ARM_All;
                buildConfig.iOSTargetOSVersion = "8.0";
                buildConfig.buildTimeout = 60;
                buildConfig.buildNode = "gemmob_server";
                buildConfig.enableAds = false;
                buildConfig.enableFirebase = false;
                buildConfig.enableIap = false;
                AssetDatabase.CreateAsset(buildConfig, BuildConfigFolderName);
                AssetDatabase.SaveAssets();
            }

            return buildConfig;
        }

        public static void SetupAchitecture(BuildConfig buildConfig) {
            AndroidArchitecture aa = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
            if (buildConfig.achitecture == BuildConfig.BuildAchietecture.ARMv7) {
                aa = AndroidArchitecture.ARMv7;
            }
            else if (buildConfig.achitecture == BuildConfig.BuildAchietecture.ARM64) {
                aa = AndroidArchitecture.ARM64;
            }
            else if (buildConfig.achitecture == BuildConfig.BuildAchietecture.X86) {
                aa = AndroidArchitecture.X86;
            }
            else if (buildConfig.achitecture == BuildConfig.BuildAchietecture.All) {
                aa = AndroidArchitecture.All;
            }

            PlayerSettings.Android.targetArchitectures = aa;
        }

        public static string GetBuildFolder() {
            var buildFolder = Path.GetDirectoryName(Application.dataPath) + "/Builds/";
            if (!Directory.Exists(buildFolder)) {
                Directory.CreateDirectory(buildFolder);
            }

            return buildFolder;
        }
        #endregion

        #region BUILD ANDROID
        public static void BuildAndroidDevAPK() {
            BuildAndroidWithConfig(LoadOrCreateBuildConfig(), new BuildConfig.Options { isDevelop = true, isBuildAPK = true }, getOsBuild());
        }

        public static void BuildAndroidDevAAB() {
            BuildAndroidWithConfig(LoadOrCreateBuildConfig(), new BuildConfig.Options { isDevelop = true }, getOsBuild());
        }

        public static void BuildAndroidRelease() {
            BuildAndroidWithConfig(LoadOrCreateBuildConfig(), BuildConfig.DefaultOptions, getOsBuild());
        }
        /**<summary> Build AAB with ARMv7 + ARMv64. No x86</summary>*/
        public static string BuildAndroidWithConfig(BuildConfig buildConfig, BuildConfig.Options options, OsBuild osBuild = null) {
            SwitchToAndroid();
            return BuildAndroid(buildConfig, options.isBuildAPK ? ".apk" : ".aab", options, osBuild);
        }

        private static string BuildAndroid(BuildConfig buildConfig, string suffix, BuildConfig.Options options, OsBuild osBuild) {
            CheckKeyStore(buildConfig);
            var buildTargetGroup = BuildTargetGroup.Android;

#if ADS_ENABLE
            AdsConfig.ApiInfo config = AdsSetting.LoadAndroidConfigFromResouceFolder();

			if (config != null) {
				AdsEditor.SetPlayerSetingBuilTargetGroup(config, buildTargetGroup);
			}

			if (!options.isDevelop && config == null && buildConfig.enableAds) {
				throw new BuildException("Please Set up ads api before build production version");
			}
#endif

            ValidModuleScriptingDefine(options.isDevelop, BuildConfig.CustomBuildTarget.Android, buildConfig, buildTargetGroup);
            var defineBuild = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            var productName = PlayerSettings.productName;
            PlayerSettings.bundleVersion = buildConfig.BuildVersion;
            PlayerSettings.Android.bundleVersionCode = buildConfig.BuildVerCode;
            PlayerSettings.Android.preferredInstallLocation = AndroidPreferredInstallLocation.Auto;
            PlayerSettings.Android.forceSDCardPermission = false;

            var splashBackground = buildConfig.androidSplashBackground;
            var splashLogo = buildConfig.androidSplashLogo;
            var icon = buildConfig.androidIcon;

            SetUpSplashAndIcon(splashBackground, splashLogo, icon, buildTargetGroup);

            var buildOptions = options.isDevelop ? buildConfig.developOptions : buildConfig.productionOptions;

            if (options.autorun) {
                buildOptions |= BuildOptions.AutoRunPlayer;
            }

            var filename = GetFileName(suffix);
            filename = (options.isDevelop ? "dev_" : "release_") + filename;
            Debug.LogFormat("Prepare for build {0}", filename);

            var scriptingImplementation = PlayerSettings.GetScriptingBackend(buildTargetGroup);

            if (options.isDevelop && options.isBuildAPK) {
                PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7;
            }
            else {
                if (buildConfig.AndroidIl2CppForRelease) {
                    PlayerSettings.SetScriptingBackend(buildTargetGroup, ScriptingImplementation.IL2CPP);
                }
                SetupAchitecture(buildConfig);
            }

            EditorUserBuildSettings.development = options.isDevelop;
            EditorUserBuildSettings.buildAppBundle = !options.isBuildAPK;

            var locationPathName = Path.Combine(GetBuildFolder(), filename);
            try {
                BuildPipeline.BuildPlayer(GetScenePaths(), locationPathName, BuildTarget.Android, buildOptions);
                if (options.autoOpen) {
                    EditorUtility.RevealInFinder(locationPathName);
                }

                return locationPathName;
            }
            catch (Exception e) {
                throw e;
            }
            finally {
                PlayerSettings.productName = productName;
                PlayerSettings.SetScriptingBackend(buildTargetGroup, scriptingImplementation);
                ScriptingDefine.SaveScriptingDefineSymbolsForGroup(buildTargetGroup, defineBuild);
            }
        }
        #endregion

        #region BUILD IOS
        public static string BuildIos(BuildConfig buildConfig, BuildConfig.Options options) {
            SwitchToiOS();
            var buildTargetGroup = BuildTargetGroup.iOS;

#if ADS_ENABLE
            AdsConfig.ApiInfo adsConfig = AdsSetting.LoadIosConfigFromResouceFolder();

            if (!options.isDevelop && adsConfig == null && buildConfig.enableAds) {
                throw new BuildException("Please set up ads api before build production version");
            }

            if (adsConfig != null) {
                AdsEditor.SetPlayerSetingBuilTargetGroup(adsConfig, buildTargetGroup);
            }
#endif

            ValidModuleScriptingDefine(options.isDevelop, BuildConfig.CustomBuildTarget.iOS, buildConfig, buildTargetGroup);
            var defineBuild = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            var productName = PlayerSettings.productName;
            PlayerSettings.bundleVersion = buildConfig.BuildVersion;
            PlayerSettings.iOS.targetDevice = buildConfig.iOSTargetDevice;
            PlayerSettings.iOS.targetOSVersionString = buildConfig.iOSTargetOSVersion;
            PlayerSettings.iOS.sdkVersion = buildConfig.iOSSdkVersion;

            var splashBackground = buildConfig.iOsSplashBackground;
            var splashLogo = buildConfig.iOsSplashLogo;
            var icon = buildConfig.iOSIcon;
            SetUpSplashAndIcon(splashBackground, splashLogo, icon, buildTargetGroup);

            var isDevelop = options.isDevelop;
            var buildOptions = isDevelop ? buildConfig.developOptions : buildConfig.productionOptions;


            if (options.autorun) {
                buildOptions |= BuildOptions.AutoRunPlayer;
            }

            var locationPathName = Path.Combine(GetBuildFolder(), isDevelop ? "DevIos" : "ReleaseIos");
            var scriptingImplementation = PlayerSettings.GetScriptingBackend(buildTargetGroup);

            if (options.isDevelop) {
                PlayerSettings.productName = "Dev " + productName;
            }
            else if (buildConfig.iOsIl2CppForRelease) {
                PlayerSettings.SetScriptingBackend(buildTargetGroup, ScriptingImplementation.IL2CPP);
            }

            EditorUserBuildSettings.development = options.isDevelop;

            try {
                BuildPipeline.BuildPlayer(GetScenePaths(), locationPathName, BuildTarget.iOS, buildOptions);
                if (options.autoOpen) {
                    EditorUtility.RevealInFinder(locationPathName);
                }

                return locationPathName;
            }
            catch (Exception e) {
                throw e;
            }
            finally {
                ScriptingDefine.SaveScriptingDefineSymbolsForGroup(buildTargetGroup, defineBuild);
                PlayerSettings.SetScriptingBackend(buildTargetGroup, scriptingImplementation);

                PlayerSettings.productName = productName;
            }
        }
        #endregion

        #region Scripting Defines
        public static void ValidModuleScriptingDefine(bool isDevelop, BuildConfig.CustomBuildTarget buildTarget, BuildConfig buildConfig, BuildTargetGroup buildTargetGroup) {
            if (buildConfig != null) {
                var defines = GetBuildDefines(isDevelop, buildTarget, buildConfig, buildTargetGroup);
                ScriptingDefine.SaveScriptingDefineSymbolsForGroup(buildTargetGroup, defines.ToArray());
            }
            else {
                Logs.LogError("buildConfig is null please check");
            }
        }

        private static List<string> ExtractDefineNames(string defineBuild) {
            return new List<string>(defineBuild.Split(';'));
        }

        public static List<string> GetBuildDefines(bool isDevelop, BuildConfig.CustomBuildTarget buildTarget, BuildConfig buildConfig, BuildTargetGroup buildTargetGroup) {
            string defineBuild = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            var defines = ExtractDefineNames(defineBuild);

#if ADS_ENABLE
            var adsConfig = AdsEditor.LoadAdsConfigResouce();
            if (adsConfig == null) {
                throw new BuildException("Please Set up ads api before build production version");
            }
            else {
                AdsEditor.ValidUnityAdsScriptingDefine(adsConfig, buildTargetGroup);
            }
#endif

            ScriptingDefine.EnableScriptingDefineFlag(isDevelop, Logs.EnableLogsString, defines);
            ScriptingDefine.EnableScriptingDefineFlag(!isDevelop, ProductionBuildString, defines);
            ScriptingDefine.EnableScriptingDefineFlag(buildConfig.enableAds, AdsEnableString, defines);
            ScriptingDefine.EnableScriptingDefineFlag(buildConfig.enableFirebase, FirebaseEnableString, defines);

            ScriptingDefine.EnableScriptingDefineFlag(buildConfig.enableIap, IapEnableString, defines);
            if (buildConfig.scriptingDefines != null) {
                foreach (var variable in buildConfig.scriptingDefines) {
                    var enableTargetBuild = (variable.target & buildTarget) == buildTarget;
                    if (isDevelop) {
                        ScriptingDefine.EnableScriptingDefineFlag(enableTargetBuild && variable.develop, variable.key,
                            defines);
                    }
                    else {
                        ScriptingDefine.EnableScriptingDefineFlag(enableTargetBuild && variable.release, variable.key,
                            defines);
                    }
                }
            }

            return defines;
        }
        #endregion

        #region other
        private static void SetUpSplashAndIcon(Sprite splashBackground, Sprite splashLogo, Texture2D icon, BuildTargetGroup buildTargetGroup) {
            PlayerSettings.SplashScreen.show = true;

            if (splashBackground != null) {
                PlayerSettings.SplashScreen.background = splashBackground;
            }

            if (splashLogo != null) {
                PlayerSettings.SplashScreen.showUnityLogo = true;
                PlayerSettings.SplashScreen.animationMode = PlayerSettings.SplashScreen.AnimationMode.Custom;
                PlayerSettings.SplashScreen.animationBackgroundZoom = 0;
                PlayerSettings.SplashScreen.animationLogoZoom = 0.5f;
                PlayerSettings.SplashScreen.drawMode = PlayerSettings.SplashScreen.DrawMode.UnityLogoBelow;
                PlayerSettings.SplashScreen.logos = new[]
                    {PlayerSettings.SplashScreenLogo.Create(2, splashLogo)};
            }
            else {
                PlayerSettings.SplashScreen.showUnityLogo = false;
            }

            if (icon == null) {
                throw new BuildException(string.Format("Please set up {0} icon", buildTargetGroup));
            }

            PlayerSettings.SetIconsForTargetGroup(buildTargetGroup,
                GetIcons(icon, buildTargetGroup == BuildTargetGroup.iOS ? 19 : 6));
        }

        private static Texture2D[] GetIcons(Texture2D icon, int length) {
            var texture2Ds = new Texture2D[length];
            for (var index = 0; index < texture2Ds.Length; index++) {
                texture2Ds[index] = icon;
            }

            return texture2Ds;
        }

        private static string GenerateSlug(string phrase) {
            string str = RemoveAccent(phrase).ToLower();
            // invalid chars           
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            str = str.Replace("-", "");
            // convert multiple spaces into one space   
            str = Regex.Replace(str, @"\s+", " ").Trim();
            // cut and trim 
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
            str = Regex.Replace(str, @"\s", "_"); // hyphens   
            return str;
        }

        private static string RemoveAccent(string txt) {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }

        private static string GetFileName(string suffix) {
            return string.Format("{0}_{1}_{2}_{3}{4}",
                GenerateSlug(PlayerSettings.productName),
                PlayerSettings.bundleVersion,
                PlayerSettings.Android.bundleVersionCode,
                DateStringFormat(),
                suffix
            );
        }

        static void SwitchToAndroid() {
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        }

        static void SwitchToiOS() {
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.iOS)
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
        }

        public static bool CheckKeyStore(BuildConfig config) {
            //string keyStorePath = Path.Combine(projectFoler, PlayerSettings.bundleIdentifier + ".keystore");
            string keyStorePath = GetKeyStorePath();
            // Check tồn tại file keytore
            var isExistsKeyStore = true;
            if (!File.Exists(keyStorePath)) {
                // Tao new keystore 
                var agrument = string.Format(
                    " -genkey -v -keystore \"{0}\" -alias {1} -keyalg RSA -keysize 2048 -validity 10000 -dname \"CN =, OU =, O =, L =, S =, C = \" -storepass {2} -keypass {3}"
                    ,
                    keyStorePath,
                    PlayerSettings.applicationIdentifier,
                    config.KeyStorePass,
                    config.KeyStorePass
                );
#if UNITY_EDITOR_OSX
				RunMacosCmd(agrument);
#elif UNITY_EDITOR_WIN
                var cmdString = string.Format("keytool {0}", agrument);
                RunCmd(cmdString);
#endif
                Debug.Log("Đã tạo keystore mới: " + keyStorePath);
                isExistsKeyStore = false;
            }

            PlayerSettings.Android.keystoreName = keyStorePath;
            PlayerSettings.keystorePass = config.KeyStorePass;

            if (string.IsNullOrEmpty(config.keyAliasName)) {
                PlayerSettings.Android.keyaliasName = PlayerSettings.applicationIdentifier;
            }
            else {
                PlayerSettings.Android.keyaliasName = config.keyAliasName;
            }

            if (string.IsNullOrEmpty(config.keyAliasPass)) {
                PlayerSettings.keyaliasPass = config.KeyStorePass;
            }
            else {
                PlayerSettings.keyaliasPass = config.keyAliasPass;
            }

            return isExistsKeyStore;
        }

        public static string GetKeyStorePath() {
            return Path.GetDirectoryName(Application.dataPath) + "/" + PlayerSettings.applicationIdentifier + ".keystore";
        }

        public static void RunCmd(string cmdString) {
            Debug.Log(cmdString);
            String command = "/C " + cmdString;
            ProcessStartInfo cmdsi = new ProcessStartInfo("cmd.exe");
            cmdsi.Arguments = command;
            Process cmd = Process.Start(cmdsi);
            cmd.WaitForExit();
        }

        public static BuildConfig.CustomBuildTarget GetFromUnityBuildTarget(BuildTargetGroup buildTarget) {
            if (buildTarget == BuildTargetGroup.Android) {
                return BuildConfig.CustomBuildTarget.Android;
            }

            if (buildTarget == BuildTargetGroup.iOS) {
                return BuildConfig.CustomBuildTarget.iOS;
            }

            return BuildConfig.CustomBuildTarget.Standalone;
        }

        public static void RunMacosCmd(string cmdString) {
            Debug.Log(cmdString);
            String command = cmdString;
            ProcessStartInfo cmdsi = new ProcessStartInfo("/usr/bin/keytool");
            cmdsi.Arguments = command;
            Process cmd = Process.Start(cmdsi);
            cmd.WaitForExit();
        }

        static string[] GetScenePaths() {
            string[] scenes = new string[EditorBuildSettings.scenes.Length];
            for (int i = 0; i < scenes.Length; i++)
                scenes[i] = EditorBuildSettings.scenes[i].path;
            return scenes;
        }

        public static string DateStringFormat() {
            return DateTime.Now.ToString("dd_MM_HH_mm");
        }

        public static OsBuild getOsBuild() {
            var environmentVariables = Environment.GetEnvironmentVariables();
            var osBuild = new OsBuild();
            if (environmentVariables.Contains("COMPUTERNAME")) {
                osBuild.machineName = environmentVariables["COMPUTERNAME"].ToString();
            }

            return osBuild;
        }
        #endregion
    }
}