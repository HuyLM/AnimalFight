using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Image))]
public class Transition : SingletonResourcesAlive<Transition> {
    [SerializeField] Color maskColor = Color.black;
    [SerializeField] float fadeDuration = 0.3f;
    

    private Image background;
    private bool transitionRunning = false;

    public enum TransitionType { Transition, Enter, Exit }

    protected override void OnAwake() {
        base.OnAwake();
        if (background == null) {
            background = GetComponent<Image>();
            if (background == null) {
                Debug.LogError("Transition is Initialized but background image is NULL!");
                return;
            }
            background.color = maskColor;
        }
    }

    public void StartTransition(System.Action transitionEnterCallback = null, System.Action transitionExitCallback = null, float enterDelayTime = 0, float exitDelayTime = 0.2f) {
        if (transitionRunning) return;
        if (background == null) return;
        if (!gameObject.activeSelf) gameObject.SetActive(true);
        if (!background.gameObject.activeSelf) background.gameObject.SetActive(true);

        DoTransit(TransitionType.Enter, () => {
            if (transitionEnterCallback != null) transitionEnterCallback.Invoke();
            DoTransit(TransitionType.Exit, transitionExitCallback, exitDelayTime);
        }, enterDelayTime, false);
    }

    private void DoTransit(TransitionType transitionType, System.Action callback = null, float delayTime = 0, bool finish = true) {
        transitionRunning = true;

        if (delayTime > 0) {
            DOVirtual.DelayedCall(delayTime, () => {
                Transit(transitionType, callback, finish);
            });
        }
        else Transit(transitionType, callback, finish);
    }

    private void Transit(TransitionType transitionType, System.Action callback = null, bool finish = true) {
        bool exit = transitionType == TransitionType.Exit;

        Color colorBegin = background.color;
        colorBegin.a = exit ? 1 : 0;
        background.color = colorBegin;

        background.DOFade(exit ? 0 : 1, fadeDuration).SetEase(Ease.InOutQuad).OnComplete(() => {
            if (callback != null) callback.Invoke();
            if (finish) {
                transitionRunning = false;
                gameObject.SetActive(false);
            }
        });
    }

}

public static partial class MonoBehaviorExtension {
    public static void DoTransition(this MonoBehaviour mono, System.Action transitionEnterCallback = null, System.Action transitionExitCallback = null,
                                    float enterDelayTime = 0, float exitDelayTime = 0.2f) {
        Transition.Instance.StartTransition(transitionEnterCallback, transitionExitCallback, enterDelayTime, exitDelayTime);
    }
}
