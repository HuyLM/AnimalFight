using UnityEngine;
using UnityEngine.UI;

namespace Gemmob.Common {
	public class DisableButtonWhen : MonoBehaviour {
		[SerializeField] private bool iapEnable;
		[SerializeField] private bool iapDisable;

		[SerializeField] private bool adsEnable;
		[SerializeField] private bool adsDisable;

		private Button button;

		private void Start() {
			button = GetComponent<Button>();
			if (button != null) {
				if (iapEnable && Build.IsIapEnable) {
					button.enabled = false;
				} else if (iapDisable && !Build.IsIapEnable) {
					button.enabled = false;
				} else if (adsEnable && Build.IsAdsEnable) {
					button.enabled = false;
				} else if (adsDisable && !Build.IsAdsEnable) {
					button.enabled = false;
				}
			}
		}
	}
}