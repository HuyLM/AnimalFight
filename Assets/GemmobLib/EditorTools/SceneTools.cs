#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Gemmob.Common.EditorTools {
    public class SceneTools {
        const string ProjectName = "Games/";

        #region Editor Scenes Menu

        [MenuItem(ProjectName + "Play", false, 0)]
        private static void PlayGame() {
            OpenLogoScene();
            EditorApplication.isPlaying = true;
        }

        [MenuItem(ProjectName + "Scenes/Open Logo Scene", false, 1)]
        private static void OpenLogoScene() {
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(GetScenePath("Logo"));
        }

        [MenuItem(ProjectName + "Scenes/Open Home Scene", false, 1)]
        private static void OpenSplashScene() {
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(GetScenePath("Home"));
        }

        [MenuItem(ProjectName + "Scenes/Open Game Scene", false, 1)]
        private static void OpenNewGameScene() {
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(GetScenePath("GamePlay"));
        }

        private static string GetScenePath(string sceneName) {
            return string.Format("Assets/Games/Scenes/{0}.unity", sceneName);
        }
        #endregion

        #region GameData 
        [MenuItem(ProjectName + "Utility/Ignore tutorial")]
        private static void IgnoreTutorial() {

        }

        [MenuItem(ProjectName + "Utility/Enable tutorial")]
        private static void EnableTutorial() {

        }

        [MenuItem(ProjectName + "Utility/Clear All Local Data")]
        private static void ClearLocalData() {
            PlayerPrefs.DeleteAll();
        }

        [MenuItem(ProjectName + "Utility/Clear Server User Data")]
        private static void ClearServerUserData() {

        }
        #endregion
    }
}
#endif