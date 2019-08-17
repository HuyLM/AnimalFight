using GoogleMobileAds.Api;
using System;
using Gemmob.API.Ads;
using Gemmob.Common.Data;
using Gemmob.Networks;

public partial class Mediation : Singleton<Mediation> {
    #region REMOVE ADS
    private const string NO_ADS = "NO_ADS";
    public static bool NoAds {
        get { return PersitenData.GetBool(NO_ADS); }
        private set { PersitenData.SetBool(NO_ADS, value); }
    }

    public static void BuyRemoveAds() {
        NoAds = true;
    }
    #endregion

    #region Property
    private static AdsConfig.AdmobInfo AdmobInfo {
        get { return AdsSetting.LoadLocalAdmobInfo(); }
    }

    private static string AppId {
        get { return AdmobInfo.admob_id; }
    }


    #endregion

    protected override void Initialize() {
        base.Initialize();
        InitInterstitial();
        InitRewardedVideo();
        InitBannerTop();
        InitBannerBottom();
        MobileAds.Initialize(AppId);
#if UNITY_IOS
        MobileAds.SetiOSAppPauseOnBackground(true);
#endif
    }

    public void Show(string position, Admob ads, Action onComplete = null, Action onFail = null, float delayTime = 0) {
        if (ads == null) {
            Logs.LogError("[ADS] You must call this first: Mediation.Instance.Initialize() \n or Add the Bootstrap.cs & enable flag preloadAds into your first Scene.");
            return;
        }

        if (NoAds && ads.type != Admob.Type.Rewarded) {
            Callback.Call(onComplete, delayTime);
            return;
        }

#if UNITY_EDITOR
        Logs.LogFormat("[ADS] Show {0} - {1}", ads.type.ToString(), position);
        AdsFake.Show(ads.type, onComplete, onFail);
        return;
#endif
        if (ads.IsLoaded) {
            Logs.LogFormat("[ADS] Show {0} - {1}", ads.type.ToString(), position);
            ads.Show(onComplete, onFail);
        }
        else {
            Callback.Call(onFail, delayTime);
            //ads.Request();
        }
    }

}
