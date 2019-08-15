using Gemmob.API.Ads;
using GoogleMobileAds.Api;
using System;
using System.Diagnostics;

public partial class Mediation {
    public const string BannerTopCondition = "ADS_BANNER_TOP";
    public const string BannerBottomCondition = "ADS_BANNER_BOTTOM";

    BannerAdmob adsBannerTop, adsBannerBottom;

    [Conditional(BannerTopCondition)]
    private void InitBannerTop() {
        adsBannerTop = new BannerAdmob(AdmobInfo.banner, Admob.Type.BannerTop, AdPosition.Top);
    }

    [Conditional(BannerBottomCondition)]
    private void InitBannerBottom() {
        adsBannerBottom = new BannerAdmob(AdmobInfo.banner, Admob.Type.BannerBottom, AdPosition.Bottom);
    }

    [Conditional(BannerTopCondition)]
    public void ShowBannerTop(string position, Action onCompleted = null, Action onFailed = null, float delayTime = 0) {
        Show(position, adsBannerTop, onCompleted, onFailed, delayTime);
    }

    [Conditional(BannerBottomCondition)]
    public void ShowBannerBottom(string position, Action onCompleted = null, Action onFailed = null, float delayTime = 0) {
        Show(position, adsBannerBottom, onCompleted, onFailed, delayTime);
    }


    [Conditional(BannerTopCondition)]
    public void RequestBannerTop() {
        if (adsBannerTop != null) adsBannerTop.Request();
    }

    [Conditional(BannerBottomCondition)]
    public void RequestBannerBottom() { 
        if (adsBannerBottom != null) adsBannerBottom.Request();
    }

    [Conditional(BannerTopCondition)]
    public void HideBannerTop() {
        if (adsBannerTop != null) adsBannerTop.Hide();
    }

    [Conditional(BannerBottomCondition)]
    public void HideBannerBottom() {
        if (adsBannerBottom != null) adsBannerBottom.Hide();
    }

    public bool HasBannerTop {
        get {
#if ADS_BANNER_TOP
            return adsBannerTop != null && adsBannerTop.IsLoaded;
#else
            Logs.Log("Your project config is not useBanner.");
            return false;
#endif
        }
    }

    public bool HasBannerBottom {
        get {
#if ADS_BANNER_BOTTOM
            return adsBannerBottom != null && adsBannerBottom.IsLoaded;
#else
            Logs.Log("Your project config is not useBanner.");
            return false;
#endif
        }
    }

}
