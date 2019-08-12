using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoScene : MonoBehaviour {

    private void Awake() {
        Time.timeScale = 1f;
    }

    public void ShowGameScene()
    {
		//UnityEngine.SceneManagement.SceneManager.LoadScene("_SwordManNewStyle/Scenes/Splash");
		SceneManager.LoadScene(1);
	}
}
