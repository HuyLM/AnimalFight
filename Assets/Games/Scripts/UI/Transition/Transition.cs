using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Transition : MonoSingletonKeepAlive<Transition> {
    [SerializeField] private Image background;

    [Header("Image - FillAmount (Nullable)")]
    [SerializeField] private Image progress;

    Coroutine fadeToCoroutine;

    private Transform progressBar;

    private IEFadeImage ieFadeComponent;

    protected override void OnAwake() {
        base.OnAwake();
        if (background == null) {
            background = GetComponentInChildren<Image>();
        }
    
        if (background) {
            ieFadeComponent = background.GetComponent<IEFadeImage>();
            if (ieFadeComponent == null) {
                ieFadeComponent = background.gameObject.AddComponent<IEFadeImage>();
            }
        }

        progressBar = progress.transform.parent;
        if (progressBar == null) progressBar = progress.transform;
    }
    
    public void StartTransition(float duration = 0.5f, System.Action enterCallback = null, System.Action exitCallback = null) {
        if (background == null) return;
        if (!gameObject.activeSelf) gameObject.SetActive(true);
        if (!background.gameObject.activeSelf) background.gameObject.SetActive(true);

        if (ieFadeComponent == null) return;

        if (fadeToCoroutine != null) {
            StopCoroutine(fadeToCoroutine);
        }

        float dFade = 0.3f;
        float dProgress = duration + 0.5f;

        ShowProgressBar(false);
        fadeToCoroutine = ieFadeComponent.FadeIn(dFade, () => {
            ShowProgressStatus(0, dProgress);
            if (enterCallback != null) enterCallback.Invoke();
            ieFadeComponent.FadeOut(dProgress, dFade, exitCallback);
        });
    }

    #region Progress loading

    private void ShowProgressBar(bool show) {
        if (progressBar == null) return;
        progressBar.gameObject.SetActive(show);
    }

    private void ShowProgressStatus(float delayTime, float duration) {
        if (progress == null) return;
        StartCoroutine(IEProgress(progress, delayTime, duration));
    }

    private IEnumerator IEProgress(Image img, float delayTime, float duration) {
        if (img == null) yield break;

        if (delayTime > 0) yield return new WaitForSeconds(delayTime);
        ShowProgressBar(true);

        float elapse = 0;
        while(elapse < duration) {
            elapse += Time.deltaTime;
            if (elapse > duration) elapse = duration;
            img.fillAmount = elapse / duration;
            yield return null;
        }

        ShowProgressBar(false);
    }
    #endregion
}
