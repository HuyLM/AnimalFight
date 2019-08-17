using Gemmob.Common;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Gemmob.EditorTools {
    public class BuildToolWindow : EditorWindow {
        private static readonly string[] targetGroupLabels = { "Android Defines", "iOs Defines" };

        private const string JenkinsfilePath = "Jenkinsfile";
        private const string JenkinsFileTplPath = "Jenkinsfile";
        private const string SkypeUsername = "gemmob.auto";
        private const string SkypeUrl = "http://skype.api.gemmob.com";
        private static readonly string[] labels = { "Config", "Android", "iOs" };//, "Set up Jenkins" };
        private int currentTab = 1;
        private SerializedObject serializedObject;
        private BuildConfig buildConfig;
        private ReorderableList reorderableList;
        private string keyGemmobBuildToolTab;
        private int currentTargetGroupTab;

        public BuildToolWindow() {
            keyGemmobBuildToolTab = "gemmob_build_tool_tab";
        }

        private void OnDisable() {
            EditorPrefs.SetInt(keyGemmobBuildToolTab, currentTab);
        }

        private void OnEnable() {
            buildConfig = BuildTool.LoadOrCreateBuildConfig();
            serializedObject = new SerializedObject(buildConfig);
            currentTab = EditorPrefs.GetInt(keyGemmobBuildToolTab, currentTab);
            reorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("scriptingDefines"), true, true, true, true) {
                drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Scripting Define Environment"); }
            };
            reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                var element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;

                var labelWidth = 50;
                var checkboxWidth = 18;
                var targetWidth = 70;
                var oldWidth = rect.x;
                EditorGUI.PropertyField(new Rect(oldWidth, rect.y, checkboxWidth + labelWidth, EditorGUIUtility.singleLineHeight),
                                        element.FindPropertyRelative("develop"), GUIContent.none);
                oldWidth += checkboxWidth;

                EditorGUI.LabelField(
                    new Rect(oldWidth, rect.y, labelWidth, EditorGUIUtility.singleLineHeight)
                    ,
                    "develop");
                oldWidth += labelWidth;

                EditorGUI.PropertyField(new Rect(oldWidth, rect.y, checkboxWidth + labelWidth, EditorGUIUtility.singleLineHeight),
                                        element.FindPropertyRelative("release"), GUIContent.none);
                oldWidth += checkboxWidth;

                EditorGUI.LabelField(new Rect(oldWidth, rect.y, labelWidth, EditorGUIUtility.singleLineHeight), "release");
                oldWidth += labelWidth;

                var targetProperty = element.FindPropertyRelative("target");
                targetProperty.intValue = (int)(BuildConfig.CustomBuildTarget)EditorGUI.EnumFlagsField(
                    new Rect(oldWidth, rect.y, targetWidth, EditorGUIUtility.singleLineHeight),
                    (BuildConfig.CustomBuildTarget)targetProperty.intValue);
                oldWidth += 80;


                EditorGUI.PropertyField(new Rect(oldWidth + 10, rect.y, rect.width - oldWidth - 10, EditorGUIUtility.singleLineHeight),
                                        element.FindPropertyRelative("key"), GUIContent.none);
                oldWidth += checkboxWidth;
            };
        }

        [MenuItem("Gemmob/Build %r", false, 10)]
        public static void OpenLevelEditorWindow() {
            GetWindow<BuildToolWindow>("GEM BUID TOOLs").Show();
        }

        void OnGUI() {
            EditorGUI.BeginChangeCheck();
            serializedObject.Update();
            currentTab = GUILayout.Toolbar(currentTab, labels);
            GUILayout.Space(10);
            try {
                switch (currentTab) {
                    case 0:
                        ModuleConfigTab();
                        break;
                    case 1:
                        BuildAndroidTab();
                        break;
                    case 2:
                        BuildIosTab();
                        break;
                        //case 3:
                        //    SetupJenkins();
                        //    break;
                }

                serializedObject.ApplyModifiedProperties();
            }
            catch (BuildException e) {
                ShowNotification(new GUIContent(e.Message));
            }
        }

        Vector3 configScrollPos;
        private void ModuleConfigTab() {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("enableIap"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("enableAds"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("enableFirebase"));
            EditorGUILayout.Separator();
            reorderableList.DoLayoutList();

            if (GUILayout.Button("Save")) {
                BuildTool.ValidModuleScriptingDefine(Build.IsDebug,
                    BuildTool.GetFromUnityBuildTarget(ScriptingDefine.GetBuildTargetGroup()), buildConfig,
                    ScriptingDefine.GetBuildTargetGroup());
            }

            GUILayout.Space(20);
            this.BeginScrollView(ref configScrollPos, () => {
                currentTargetGroupTab = GUILayout.Toolbar(currentTargetGroupTab, targetGroupLabels);
                switch (currentTargetGroupTab) {
                    case 0:
                        EditorGUILayout.BeginHorizontal();
                        PrintDefines(BuildConfig.CustomBuildTarget.Android, true);
                        PrintDefines(BuildConfig.CustomBuildTarget.Android, false);
                        EditorGUILayout.EndHorizontal();
                        break;
                    case 1:
                        EditorGUILayout.BeginHorizontal();
                        PrintDefines(BuildConfig.CustomBuildTarget.iOS, true);
                        PrintDefines(BuildConfig.CustomBuildTarget.iOS, false);
                        EditorGUILayout.EndHorizontal();
                        break;
                }
            });

            serializedObject.ApplyModifiedProperties();
        }

        private void PrintDefines(BuildConfig.CustomBuildTarget buildTarget, bool isDevelop) {
            EditorGUILayout.BeginVertical("Box");
            var label = buildTarget + (isDevelop ? " [Develop]" : " [Release]");
            EditorGUILayout.LabelField(label + (isDevelop == Build.IsDebug ? " [CURRENT]" : ""));
            var buildDefines =
                BuildTool.GetBuildDefines(isDevelop, buildTarget, buildConfig, ScriptingDefine.GetBuildTargetGroup());
            if (buildDefines != null) {
                using (new EditorGUI.DisabledGroupScope(true)) {
                    foreach (var buildDefine in buildDefines) {
                        EditorGUILayout.LabelField(buildDefine);
                    }
                }
            }

            EditorGUILayout.EndVertical();
        }


        private void SetupJenkins() {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("buildTimeout"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("buildNode"));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();


            var serializedProperty = serializedObject.FindProperty("UnityBuildVersion");
            EditorGUILayout.PropertyField(serializedProperty);
            if (GUILayout.Button("Set Current Version")) {
                serializedProperty.stringValue = Application.unityVersion;
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField(
                new GUIContent(string.Format("Please add skype id {0} then find skype group at url {1}", SkypeUsername, SkypeUrl)));
            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Copy Skype Id")) {
                EditorGUIUtility.systemCopyBuffer = SkypeUsername;
            }


            if (GUILayout.Button("Open Url")) {
                Application.OpenURL(SkypeUrl);
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("GroupSkypeId"));


            EditorGUILayout.EndHorizontal();
            GUILayout.Space(30);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Generate Jenkinsfile")) {
                var textAsset = Resources.Load<TextAsset>(JenkinsFileTplPath);
                if (textAsset == null) {
                    ShowNotification(new GUIContent(string.Format("Not found {0} in Resources folder", JenkinsFileTplPath)));
                }
                else {
                    var textAssetText = textAsset.text;
                    if (string.IsNullOrEmpty(buildConfig.GroupSkypeId) ||
                        string.IsNullOrEmpty(buildConfig.buildNode) ||
                        string.IsNullOrEmpty(buildConfig.UnityBuildVersion)) {
                        ShowNotification(new GUIContent("GroupSkypeId or GroupSkypeId or BuildNode must be not empty"));
                    }
                    else {
                        textAssetText = textAssetText.Replace("%GroupSkypeId%", buildConfig.GroupSkypeId);
                        textAssetText = textAssetText.Replace("%UnityBuildVersion%", buildConfig.UnityBuildVersion);
                        textAssetText = textAssetText.Replace("%BuildNode%", buildConfig.buildNode);
                        textAssetText = textAssetText.Replace("%BuildTimeout%", buildConfig.buildTimeout.ToString());
                        using (FileStream fs = new FileStream(JenkinsfilePath,
                            FileMode.Create)) {
                            using (StreamWriter writer = new StreamWriter(fs)) {
                                writer.Write(textAssetText);
                                ShowNotification(new GUIContent("Jenkins file is generated on " + Path.GetFullPath(JenkinsfilePath)));
                            }
                        }
                    }
                }
            }

            if (GUILayout.Button("Open Jenkinsfile")) {
                EditorUtility.RevealInFinder(Path.GetFullPath(JenkinsfilePath));
            }

            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Save")) {
                SaveAssets(buildConfig, BuildTargetGroup.Android);
            }

            GUILayout.Space(10);
        }

        private bool CheckBeforeBuildIos() {
            return EditorUtility.DisplayDialog("Warning!", "Are you sure you want to build IOS", "Yes", "No");
        }

        private bool CheckBeforeBuildAndroid() {
            return EditorUtility.DisplayDialog("Warning!", "Are you sure you want to build ANDROID", "Yes", "No");
        }

        private void BuildIosTab() {
            EditorGUILayout.LabelField("Basic ", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("iOsIl2CppForRelease"), new GUIContent("Il2CPP Release Build"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("developOptions"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("productionOptions"));

            GUILayout.Space(20);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("iOSTargetDevice"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("iOSSdkVersion"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("iOSTargetOSVersion"));

            var iconKey = "iOSIcon";
            var splashBackground = "iOsSplashBackground";
            var splashLogo = "iOsSplashLogo";
            DrawIconSetup(iconKey, splashBackground, splashLogo);

            if (GUILayout.Button("Save")) {
                SaveAssets(buildConfig, BuildTargetGroup.iOS);
            }

            GUILayout.Space(20);
            EditorGUILayout.LabelField("Build", EditorStyles.boldLabel);
            this.BeginHorizontal(() => {
                if (GUILayout.Button("Build Dev") && CheckBeforeBuildIos()) {
                    BuildTool.BuildIos(buildConfig, new BuildConfig.Options { isDevelop = true, autoOpen = true });
                }

                if (GUILayout.Button("Build Release") && CheckBeforeBuildIos()) {
                    EditorUtility.RevealInFinder(BuildTool.BuildIos(buildConfig, BuildConfig.DefaultOpenOptions));
                }
            });

            GUILayout.Space(20);
            EditorGUILayout.LabelField("Open Folder", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Open Build Folder")) {
                OpenBuildFolder();
            }


            EditorGUILayout.EndHorizontal();
        }

        private void BuildAndroidTab() {
            EditorGUILayout.LabelField("Compression ", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("developOptions"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("productionOptions"));

            GUILayout.Space(20);
            EditorGUILayout.LabelField("AAB Config ", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("AndroidIl2CppForRelease"), new GUIContent("Il2CPP Release Build"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("achitecture"), new GUIContent("Build Achitecture"));

            GUILayout.Space(20);
            this.BeginHorizontal(() => {
                this.BeginVertical(() => {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("BuildVersion"), new GUIContent("Build Version (a.b.c)"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("BuildVerCode"), new GUIContent("Build Vercode (abc)"));
                });

                this.BeginVertical(() => {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("KeyStorePass"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("keyAliasName"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("keyAliasPass"));
                });
            });

            GUILayout.Space(20);
            var iconKey = "androidIcon";
            var splashBackground = "androidSplashBackground";
            var splashLogo = "androidSplashLogo";
            DrawIconSetup(iconKey, splashBackground, splashLogo);

            if (GUILayout.Button("Save")) {
                SaveAssets(buildConfig, BuildTargetGroup.Android);
            }

            GUILayout.Space(20);
            EditorGUILayout.LabelField("Build", EditorStyles.boldLabel);
            this.BeginHorizontal(() => {
                if (GUILayout.Button("Build Dev APK v7")) {
                    BuildTool.BuildAndroidDevAPK();
                    OpenBuildFolder();
                }

                if (GUILayout.Button("Build Dev AAB")) {
                    BuildTool.BuildAndroidDevAAB();
                    OpenBuildFolder();
                }

                if (GUILayout.Button("Build Release (aab)")) {
                    BuildTool.BuildAndroidRelease();
                    OpenBuildFolder();
                }
            });

            GUILayout.Space(20);
            EditorGUILayout.LabelField("Open Folder", EditorStyles.boldLabel);
            this.BeginHorizontal(() => {
                if (GUILayout.Button("Open Build Folder")) {
                    OpenBuildFolder();
                }

                if (GUILayout.Button("Generate KeyStore") && BuildTool.CheckKeyStore(buildConfig)) {
                    ShowNotification(
                        new GUIContent(string.Format("Key đã được tạo {0}", BuildTool.GetKeyStorePath())));
                }

                if (GUILayout.Button("Open KeyStore")) {
                    EditorUtility.RevealInFinder(BuildTool.GetKeyStorePath());
                }
            });
        }

        private static void OpenBuildFolder() {
            EditorUtility.RevealInFinder(BuildTool.GetBuildFolder());
        }

        private void DrawIconSetup(string iconKey, string splashBackground, string splashLogo) {
            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Icon ", EditorStyles.boldLabel, GUILayout.Width(30));
            var iconProperty = serializedObject.FindProperty(iconKey);
            iconProperty.objectReferenceValue = EditorGUILayout.ObjectField(GUIContent.none,
                iconProperty.objectReferenceValue, typeof(Texture2D), false, GUILayout.ExpandWidth(false),
                GUILayout.Width(50));

            EditorGUILayout.LabelField("Splash Background", EditorStyles.boldLabel, GUILayout.Width(110));
            var androidSplashBackground = serializedObject.FindProperty(splashBackground);
            androidSplashBackground.objectReferenceValue = EditorGUILayout.ObjectField(GUIContent.none,
                androidSplashBackground.objectReferenceValue, typeof(Sprite), false, GUILayout.ExpandWidth(false),
                GUILayout.Width(50));

            EditorGUILayout.LabelField("Splash Logo", EditorStyles.boldLabel, GUILayout.Width(80));
            var androidSplashLogo = serializedObject.FindProperty(splashLogo);
            androidSplashLogo.objectReferenceValue = EditorGUILayout.ObjectField(GUIContent.none,
                androidSplashLogo.objectReferenceValue, typeof(Sprite), false, GUILayout.ExpandWidth(false),
                GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();
        }

        private void SaveAssets(BuildConfig buildConfig, BuildTargetGroup targetGroup) {
#if UNITY_ANDROID
            BuildTool.SetupAchitecture(buildConfig);
#endif
            AssetDatabase.SaveAssets();
        }
    }
}