using UnityEngine;
using UnityEngine.UI;

namespace Gemmob.Common {
	public class DisableWhen : MonoBehaviour {
		[SerializeField] private bool iapEnable;
		[SerializeField] private bool iapDisable;

		[SerializeField] private bool adsEnable;
		[SerializeField] private bool adsDisable;


		private void OnEnable() {
			if (iapEnable && Build.IsIapEnable) {
				gameObject.SetActive(false);
			} else if (iapDisable && !Build.IsIapEnable) {
				gameObject.SetActive(false);
			} else if (adsEnable && Build.IsAdsEnable) {
				gameObject.SetActive(false);
			} else if (adsDisable && !Build.IsAdsEnable) {
				gameObject.SetActive(false);
			}
		}
	}
}