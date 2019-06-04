using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class IEFadeImage : MonoBehaviour {
    private Image img;

    public Coroutine FadeIn(float duration, System.Action callback = null) {
        if (!img.gameObject.activeSelf) img.gameObject.SetActive(true);
        return StartCoroutine(IEFadeTo(img, 0, 1, duration, 0, callback));
    }

    public Coroutine FadeOut(float delayTime, float duration, System.Action callback = null) {
        //StopCoroutine("ShowProgress");
        return StartCoroutine(IEFadeTo(img, 1, 0, duration, delayTime, () => {
            img.gameObject.SetActive(false);
            if (callback != null) callback.Invoke();
        }));
    }

    public IEnumerator IEFadeTo(Image img, float froA, float toA, float duration, float delayTime = 0, System.Action callback = null) {
        if (img == null) yield break;

        if (delayTime > 0) yield return new WaitForSeconds(delayTime);

        float elapse = 0;
        Color froColor = img.color;
        Color toColor = froColor;
        froColor.a = froA;
        toColor.a = toA;

        while (elapse < duration) {
            elapse += Time.deltaTime;
            img.color = Color.Lerp(froColor, toColor, elapse / duration);
            yield return null;
        }
        img.color = toColor;
        if (callback != null) callback.Invoke();
    }

}
