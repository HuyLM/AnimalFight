using GoogleMobileAds.Api;

namespace Gemmob.API.Ads {
    public class BannerAdmob : Admob {

        private BannerView bannerView;

        private AdPosition adPosition;

        public BannerAdmob(string AdUnitId, Type type, AdPosition adPosition = AdPosition.Bottom) : base(AdUnitId, type) {
            this.adPosition = adPosition;
        }

        public override void Initialize() {
            if (bannerView != null) bannerView.Destroy();

            bannerView = new BannerView(AdUnitId, AdSize.SmartBanner, adPosition);
            bannerView.OnAdLoaded += OnAdLoaded;
            bannerView.OnAdFailedToLoad += OnAdFailedToLoad;
            bannerView.OnAdOpening += OnAdOpened;
            bannerView.OnAdClosed += OnAdClosed;
            bannerView.OnAdLeavingApplication += OnAdLeavingApplication;
        }

        protected override void OnRequest() {
            Initialize();
            bannerView.LoadAd(GetAdRequest());
        }

        protected override void OnShow() {
            if (bannerView == null) {
                Request();
                return;
            }
            
            bannerView.Show();
        }

        public override bool IsLoaded {
            get {
                if (bannerView != null) return true;
                Request();
                return false;
            }
        }
        
        public override void Hide() {
            if (bannerView != null)
                bannerView.Hide();
        }
    }
}