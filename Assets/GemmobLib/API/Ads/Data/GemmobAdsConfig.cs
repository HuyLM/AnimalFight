using UnityEngine;

namespace Gemmob.API.Ads {
	public class GemmobAdsConfig : ScriptableObject {

        public const string ResourceName = "GemmobAdsConfig";

        private static GemmobAdsConfig instance;
        public static GemmobAdsConfig Instance {
            get {
                if (instance == null) {
                    instance = Resources.Load<GemmobAdsConfig>(ResourceName);
                }
                return instance;
            }
        }

		public bool enableAndroid;
		public string androidApiLink;

		public bool enableIos;
		public string iosApiLink;

        public string testDeviceId;

		public bool useBannerTop;
        public bool useBannerBottom;
        public bool useInterstitial;
		public bool useRewardVideo;

        public bool enableTest;
	}
}