using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Image))]
public class LogoScene : MonoBehaviour {
    [Header("Place this script into the main logo image")]
    [SerializeField] private float duration = 2.0f;
    [SerializeField] private int nextSceneIndex = 1;

    private void Awake() {
        Time.timeScale = 1f;
    }

    private void Start() {
        DoAnimate();
    }

    private void DoAnimate() {
        var imgLogo = GetComponent<Image>();
        if (imgLogo == null) {
            Debug.LogError("Add for me the image logo please :)");
            DOVirtual.DelayedCall(duration, ShowNextScene);
            return;
        }

        var color = imgLogo.color;
        color.a = 10.0f / 255f;
        imgLogo.color = color;

        imgLogo.transform.localScale = Vector3.one;
        imgLogo.transform.DOScale(1.2f, duration);
        imgLogo.DOFade(1.0f, duration).OnComplete(() => {
            this.DoTransition(ShowNextScene);
        });
    }

    private void ShowNextScene() {
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneIndex);
	}
}
