using Gemmob.API.Ads;
using System;
using System.Diagnostics;

public partial class Mediation {
    public const string RewardedCondition = "ADS_REWARDED";

    RewardedAdmob adsRewarded;

    [Conditional(RewardedCondition)]
    private void InitRewardedVideo() {
        adsRewarded = new RewardedAdmob(AdmobInfo.video, Admob.Type.Rewarded);
    }

    [Conditional(RewardedCondition)]
    public void ShowRewardVideo(string position, Action onCompleted = null, Action onFailed = null, float delayTime = 0) {
        Show(position, adsRewarded, onCompleted, onFailed, delayTime);
    }

    [Conditional(RewardedCondition)]
    private void RequestRewardVideo() {
        if (adsRewarded != null) adsRewarded.Request();
    }

    public bool HasRewardVideo {
        get {
#if ADS_REWARDED
            return adsRewarded != null && adsRewarded.IsLoaded;
#else
            Logs.Log("Your project config is not useRewardVideo.");
            return false;
#endif
        }
    }
}
