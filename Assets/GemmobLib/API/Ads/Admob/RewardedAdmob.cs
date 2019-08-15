using GoogleMobileAds.Api;
using System;
using Gemmob.Networks;

namespace Gemmob.API.Ads {
    public class RewardedAdmob : Admob {
        private RewardBasedVideoAd rewardBasedVideo;

        private bool getRewarded = false;
        
        public RewardedAdmob(string AdUnitId, Type type) : base(AdUnitId, type) {
            Initialize();
            Request();
        }

        public override void Initialize() {
            this.rewardBasedVideo = RewardBasedVideoAd.Instance;

            rewardBasedVideo.OnAdLoaded += OnAdLoaded;
            rewardBasedVideo.OnAdFailedToLoad += OnAdFailedToLoad;
            rewardBasedVideo.OnAdOpening += OnAdOpened;
            rewardBasedVideo.OnAdRewarded += OnAdRewarded;
            rewardBasedVideo.OnAdClosed += OnVideoClose;
            rewardBasedVideo.OnAdLeavingApplication += OnAdLeavingApplication;
        }

        protected override void OnRequest() {
            rewardBasedVideo.LoadAd(GetAdRequest(), AdUnitId);
        }

        protected override void OnShow() {
            if (IsLoaded) rewardBasedVideo.Show();
        }

        public override bool IsLoaded {
            get {
                if (rewardBasedVideo.IsLoaded()) return true;
                Request();
                return false;
            }
        }

        protected override void OnAdOpened(object sender, EventArgs args) {
            getRewarded = false;
        }

        private void OnAdRewarded(object sender, Reward e) {
            getRewarded = true;
        }

        private void OnVideoClose(object sender, EventArgs args) {
            Callback.Call(getRewarded ? OnComplete : OnFail);
            getRewarded = false;
            Request();
        }
    }
}