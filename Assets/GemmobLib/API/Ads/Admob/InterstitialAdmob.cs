using System;
using GoogleMobileAds.Api;

namespace Gemmob.API.Ads {
    public class InterstitialAdmob : Admob {
        private InterstitialAd interstitial;

        public InterstitialAdmob(string AdUnitId, Type type) : base(AdUnitId, type) {
            Request();
        }

        public override void Initialize() {
            if (interstitial != null) interstitial.Destroy();

            interstitial = new InterstitialAd(AdUnitId);
            this.interstitial.OnAdLoaded += OnAdLoaded;
            this.interstitial.OnAdFailedToLoad += OnAdFailedToLoad;
            this.interstitial.OnAdOpening += OnAdOpened;
            this.interstitial.OnAdClosed += OnAdClosed;
            this.interstitial.OnAdLeavingApplication += OnAdLeavingApplication;
        }

        protected override void OnRequest() {
            Initialize();
            interstitial.LoadAd(GetAdRequest());
        }

        protected override void OnShow() {
            if (IsLoaded) interstitial.Show();
        }

        public override bool IsLoaded {
            get {
                if (interstitial != null && interstitial.IsLoaded()) return true;
                Request();
                return false;
            }
        }
        
        public override void Hide() {

        }
    }
}