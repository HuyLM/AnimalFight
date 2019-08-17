using UnityEngine;
using Gemmob.API.Ads;
using UnityEngine.UI;

public class AdsFake : MonoBehaviour {
    private const string PrefabPath = "Assets/GemmobLib/API/Ads/FakeAds/AdsFake.prefab";

    [SerializeField] private Text title;
    [SerializeField] private Button btnSuccess, btnSkip, btnLoadFailed;

    System.Action successCallback, failedCallback;
    
    public static void Show(Admob.Type type, System.Action onSuccessCallback, System.Action onFailedCallback = null) {
#if UNITY_EDITOR
        GameObject prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
#else
        GameObject prefab = Resources.Load<GameObject>("AdsFake");
#endif
        if (prefab == null) {
            Logs.LogError("[AdsFake] Can't find AdsFake prefab from Resources");
            if (onFailedCallback != null) onFailedCallback.Invoke();
            return;
        }
        AdsFake ads = Instantiate(prefab).GetComponent<AdsFake>();
        ads.Init(type, onSuccessCallback, onFailedCallback);
    }

    public void Init(Admob.Type type, System.Action onSuccessCallback, System.Action onFailedCallback = null) {
        this.successCallback = onSuccessCallback;
        this.failedCallback = onFailedCallback;

        if (type == Admob.Type.Interstitial) {
            btnLoadFailed.gameObject.SetActive(false);
            btnSkip.gameObject.SetActive(false);
            btnSuccess.gameObject.SetActive(true);

            title.text = "Interstitial Fake";
            var text = btnSuccess.GetComponentInChildren<Text>();
            if (text != null) text.text = "OK";
        }
        else if (type == Admob.Type.Rewarded) {
            btnLoadFailed.gameObject.SetActive(true);
            btnSkip.gameObject.SetActive(true);
            btnSuccess.gameObject.SetActive(true);

            title.text = "Video Rewarded Fake";
            var text = btnSuccess.GetComponentInChildren<Text>();
            if (text != null) text.text = "Watch Done";
        }
        else {
            btnLoadFailed.gameObject.SetActive(false);
            btnSkip.gameObject.SetActive(false);
            btnSuccess.gameObject.SetActive(true);
#if UNITY_EDITOR
            Vector2 screenSize = UnityEditor.Handles.GetMainGameViewSize();
#else
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);
#endif
            var smartheight = screenSize.y > 720 ? 90 : screenSize.y > 400 ? 50 : 32;

            title.gameObject.SetActive(false);

            var text = btnSuccess.GetComponentInChildren<Text>();
            if (text != null) {
                text.fontSize /= 2;
                text.text = string.Format("This is a banner {0}x{1}\nClick me!", screenSize.x, smartheight);
            }

            var body = title.transform.parent as RectTransform;
            var layout = body.GetComponent<VerticalLayoutGroup>();
            if (layout) {
                layout.childControlHeight = true;
                layout.childForceExpandHeight = true;
            }

            body.sizeDelta = new Vector2(screenSize.x, smartheight);

            int pivot = type == Admob.Type.BannerTop ? 1 : 0;
            body.anchorMin = new Vector2(0, pivot);
            body.anchorMax = new Vector2(1, pivot);
            body.pivot = new Vector2(0.5f, pivot);
            body.anchoredPosition = new Vector2(0, 0);
        }
    }

    public void OnClickSuccessButton() {
        if (successCallback != null) successCallback.Invoke();
        Destroy(gameObject);
    }

    public void OnClickFailedButton() {
        if (failedCallback != null) failedCallback.Invoke();
        Destroy(gameObject);
    }
}
