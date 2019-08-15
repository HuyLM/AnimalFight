using Gemmob.API.Ads;
using System;
using System.Diagnostics;

public partial class Mediation {
    public const string InterstitialCondition = "ADS_INTERSTITIAL";

    InterstitialAdmob adsInterstitial;

    [Conditional(InterstitialCondition)]
    private void InitInterstitial() {
        adsInterstitial = new InterstitialAdmob(AdmobInfo.interstitial, Admob.Type.Interstitial);
    }

    [Conditional(InterstitialCondition)]
    public void ShowInterstitial(string position, Action onCompleted = null, Action onFailed = null, float delayTime = 0) {
        Show(position, adsInterstitial, onCompleted, onFailed);
    }

    [Conditional(InterstitialCondition)]
    public void ShowInterstitialWithPercent(int percent, string position = "", Action onCompleted = null, float delayTime = 0) {
        if (adsInterstitial != null && UnityEngine.Random.Range(0, 101) <= percent) {
            ShowInterstitial(position, onCompleted, null, delayTime);
            return;
        }
    }
    
    [Conditional(InterstitialCondition)]
    public void HideInterstitial() {
        if (adsInterstitial != null) adsInterstitial.Hide();
    }

    public bool HasInterstitial {
        get {
#if ADS_INTERSTITIAL
            return adsInterstitial != null && adsInterstitial.IsLoaded;
#else
            Logs.Log("Your project config is not useInterstitial.");
            return false;
#endif
        }
    }

}
