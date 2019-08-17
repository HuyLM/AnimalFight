using Gemmob.Networks;
using GoogleMobileAds.Api;
using System;
using UniRx;

namespace Gemmob.API.Ads {
    public abstract class Admob {

        public enum Type { Rewarded, Interstitial, BannerBottom, BannerTop }
        public string AdUnitId;
        public Type type;

        protected Action OnComplete;
        protected Action OnFail;

        protected bool requesting;

        private double[] retryTimes = { 2, 5, 10, 20, 60, 60, 120, 120, 240, 240, 400, 400, 600 };
        protected int retry;

        protected double GetRetryTime(int retry) {
            return retry < retryTimes.Length && retry >= 0 ? retryTimes[retry] : retryTimes[retryTimes.Length - 1];
        }

        public Admob(string AdUnitId, Type type) {
            this.AdUnitId = AdUnitId;
            this.type = type;
            MobileAds.SetiOSAppPauseOnBackground(true);
        }
        
        public void Show(Action complete, Action fail) {
            this.OnComplete = complete;
            this.OnFail = fail;
            OnShow();
        }

        public void Request() {
            if (requesting) return;
            if (!Network.IsInternetAvaiable) return;

            requesting = true;
            var delayRequest = GetRetryTime(retry);
            Logs.LogFormat("[ADS] Request {0}: delay={1}s, retry={2}", type.ToString(), delayRequest, retry);

            Scheduler.MainThreadIgnoreTimeScale.Schedule(TimeSpan.FromSeconds(delayRequest), action => {
                try {
                    OnRequest();
                }
                catch (Exception e) {
                    requesting = false;
                    Logs.LogErrorFormat("[ADS] Error when request {0}: {2}", type.ToString(), e.Message);
                }
            });
        }

        protected AdRequest GetAdRequest() {
            var adRequest = new AdRequest.Builder();
#if PRODUCTION_BUILD
            adRequest.AddTestDevice(AdRequest.TestDeviceSimulator)
                    .AddTestDevice(GemmobAdsConfig.Instance.testDeviceId);
#endif
            return adRequest.Build();
        }
        
        public virtual void Hide() { }

#region Abstract
        public abstract void Initialize();

        protected abstract void OnRequest();

        protected abstract void OnShow();

        public abstract bool IsLoaded { get; }
#endregion

#region Handle
        protected virtual void OnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args) {
            requesting = false;
            retry++;
            Logs.LogErrorFormat("[ADS] FailedToLoad {0}: {1}", type.ToString(), args.Message);
            Callback.Call(OnFail);
            Request();
        }

        protected virtual void OnAdClosed(object sender, EventArgs args) {
            Callback.Call(OnComplete);
            Request();
        }

        protected virtual void OnAdLoaded(object sender, EventArgs args) {
            requesting = false;
            retry = 0;
            Logs.LogFormat("[ADS] Request Loaded Successfully {0}. Ready to show", type.ToString());
        }

        protected virtual void OnAdOpened(object sender, EventArgs args) {

        }

        public virtual void OnAdLeavingApplication(object sender, EventArgs args) {

        }
#endregion
    }
}