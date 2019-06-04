using System.Collections;
using UnityEngine;

public class IEScaleTo : MonoBehaviour {
    public enum ScaleType { None, ZoomIn, ZoomOut, Flicker }
    [SerializeField] private bool autoRun = false;
    [SerializeField] private bool loop = false;
    [SerializeField] ScaleType scaleType = ScaleType.None;
    [SerializeField] private Vector3 scaleFrom = Vector3.one;
    [SerializeField] private Vector3 scaleTo = Vector3.one;
    [SerializeField] private float duration = 0.3f;
    [SerializeField] private float startDelay = 0;

    private void OnEnable() {
        if (autoRun && scaleType != ScaleType.None) {
            StartCoroutine(IEAutoRun(scaleType));
        }
    }

    private void OnDisable() {
        StopAllCoroutines();
    }

    public void ZoomIn(float time, float delayTime = 0, UnityEngine.Events.UnityAction callback = null) {
        ScaleTo(Vector3.zero, transform.localScale, time, delayTime, callback);
    }

    public void ZoomOut(float time, float delayTime = 0, UnityEngine.Events.UnityAction callback = null) {
        ScaleTo(transform.localScale, Vector3.zero, time, delayTime, callback);
    }

    public void ScaleTo(Vector3 fro, Vector3 to, float time, float delayTime = 0, UnityEngine.Events.UnityAction callback = null) {
        StartCoroutine(IEScale(fro, to, time, delayTime, callback));
    }

    IEnumerator IEScale(Vector3 fro, Vector3 to, float time, float delayTime = 0, UnityEngine.Events.UnityAction callback = null) {
        if (delayTime > 0) {
            yield return new WaitForSeconds(delayTime);
        }

        transform.localScale = fro;

        float elapse = 0;
        while (elapse < time) {
            elapse += Time.deltaTime;
            transform.localScale = Vector3.Lerp(fro, to, elapse / time);
            yield return null;
        }

        transform.localScale = to;

        if (callback != null) {
            callback.Invoke();
        }
    }

    IEnumerator IEAutoRun(ScaleType type) {
        if (type == ScaleType.Flicker) {
            yield return IEScale(scaleFrom, scaleTo, duration);
            yield return IEScale(scaleTo, scaleFrom, duration);
        }
        else {
            yield return IEScale(scaleFrom, scaleTo, duration, startDelay);
        }

        if (loop) {
            StartCoroutine(IEAutoRun(type));
        }
    }
}
