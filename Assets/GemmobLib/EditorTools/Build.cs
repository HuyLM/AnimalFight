using UnityEngine;

namespace Gemmob.Common {
	public class Build {
		public static bool IsEditor {
			get {
#if UNITY_EDITOR
				return true;
#else
                return false;
	#endif
			}
		}

		public static bool IsDebug {
			get {
#if PRODUCTION_BUILD
				return false;
#else
				return true;
#endif
			}
		}

		public static bool IsRelease {
			get {
#if PRODUCTION_BUILD
				return true;

#else
				return false;
#endif
			}
		}

		public static bool IsAdsEnable {
			get {
#if ADS_ENABLE
				return true;
#else
                return false;
#endif
			}
		}

		public static bool IsIapEnable {
			get {
#if IAP_ENABLE
                return true;
#else
				return false;
#endif
			}
		}

		public static bool IsFirebaseEnable {
			get {
#if FIREBASE_ENABLE
				return true;
#else
                return false;
#endif
			}
		}
	}
}