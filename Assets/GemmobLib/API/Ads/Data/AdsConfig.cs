using System;

namespace Gemmob.API.Ads {
	public class AdsConfig {
		// Class

		[Serializable]
		public class ApiInfo {
			// Thông tin game
			public GameInfo info;

			// Thông tin quảng cáo
			public AdsInfo ads;
		}

		[Serializable]
		public class GameInfo {
			// android or ios
			public string type;

			// Tên ứng dụng
			public string name;

			// Bundle ứng dụng
			public string package;

			// Version api (version ứng ụng)
			public string version_name;

			// Version code
			public string version_code;

			// Id GA
			public string tracking_id;

			//OneSignal id
			public string onesignal_code;

			// Facebook name
			public string facebook_name;

			// Facebook id
			public string facebook_app_id;

			// Developer name => show more games link
			public string developer;
		}

		[Serializable]
		public class AdmobInfo {
			// Id banner
			public string banner;

			public string admob_id;

			// Id full
			public string interstitial;

			// Id video reward
			public string video;

            public AdmobInfo(string admob_id, string banner, string interstitial, string video) {
                this.admob_id = admob_id;
                this.banner = banner;
                this.interstitial = interstitial;
                this.video = video;
            }
		}

		[Serializable]
		public class BackupInfo {
			//banner id (starapp)
			public string startapp;

			//video id (unity)
			public string unityads;
		}

		[Serializable]
		public class ControlInfo {
			//intertitial ads(admob or backup)
			public string interstitial;

			//video ads(admob or backup)
			public string video;
		}

		[Serializable]
		public class ShowInfo {
			public int x1;
			public int x2;
			public int x3;
			public int x4;
			public int x5;
		}

		[Serializable]
		public class AdsInfo {
			public AdmobInfo admob;
			public BackupInfo backup;
			public ControlInfo control;
			public ShowInfo show;
		}
	}
}