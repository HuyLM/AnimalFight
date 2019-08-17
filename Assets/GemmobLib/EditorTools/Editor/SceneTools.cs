#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Gemmob.EditorTools {
    public class SceneTools {
        const string ProjectPath = "Games/";

        #region Editor Scenes Menu
        public static string LogoScenePath = "Assets/GemmobLib/Bootstrap/Logo/Logo.unity";
        public static string[] SceneName = new string[] { "Assets/" + ProjectPath + "Scenes/Home.unity",
                                                            "Assets/" + ProjectPath + "Scenes/GamePlay.unity" };

        [MenuItem(ProjectPath + "Play", false, 0)]
        private static void PlayGame() {
            OpenLogoScene();
            EditorApplication.isPlaying = true;
        }

        [MenuItem(ProjectPath + "Scenes/Open Logo Scene", false, 1)]
        private static void OpenLogoScene() {
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(LogoScenePath);
        }

        [MenuItem(ProjectPath + "Scenes/Open Home Scene", false, 1)]
        private static void OpenSplashScene() {
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(SceneName[0]);
        }

        [MenuItem(ProjectPath + "Scenes/Open GamePlay Scene", false, 1)]
        private static void OpenGamePlayScene() {
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(SceneName[1]);
        }

        static string[] GetCurrentBuildScenePaths() {
            string[] scenes = new string[EditorBuildSettings.scenes.Length];
            for (int i = 0; i < scenes.Length; i++)
                scenes[i] = EditorBuildSettings.scenes[i].path;
            return scenes;
        }
        #endregion

        #region GameData 
        [MenuItem(ProjectPath + "Utility/Ignore tutorial")]
        private static void IgnoreTutorial() {

        }

        [MenuItem(ProjectPath + "Utility/Enable tutorial")]
        private static void EnableTutorial() {

        }

        [MenuItem(ProjectPath + "Utility/Clear All Local Data")]
        private static void ClearLocalData() {
            PlayerPrefs.DeleteAll();
        }

        [MenuItem(ProjectPath + "Utility/Clear Server User Data")]
        private static void ClearServerUserData() {

        }
        #endregion

        #region Editor Setting
        [MenuItem(ProjectPath + "Editor Setting")]
        private static void ShowEditorSetting() {
            EditorSetting.Init();
        }

        #endregion
    }

    public class EditorSetting : EditorWindow {
        string[] tab = new string[] { "Scenes Config" };
        int currentTab;

        public static void Init() {
            GetWindow<EditorSetting>().Show();
        }

        private void OnGUI() {
            currentTab = GUILayout.Toolbar(currentTab, tab);
            GUILayout.Space(20);

            if (currentTab == 0) {
                SceneConfig();
            }
        }

        private void SceneConfig() {
            this.BeginVertical(() => {
                this.BeginVertical("Logo Scene Path: ", () => {
                    SceneTools.LogoScenePath = GUILayout.TextField(SceneTools.LogoScenePath);
                });

                GUILayout.Space(10);
                this.BeginVertical("Other Scene: ", () => {
                    for (int i = 0; i < SceneTools.SceneName.Length; ++i) {
                        SceneTools.SceneName[i] = GUILayout.TextField(SceneTools.SceneName[i]);
                    }
                });
            });
        }

    }
}
#endif