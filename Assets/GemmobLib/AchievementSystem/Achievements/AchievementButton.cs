using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RippleButton))]
public class AchievementButton : MonoBehaviour
{
	[SerializeField] private Text coinText;
	[SerializeField] private Color goldEnableColor, goldDisableColor;

	AchievementViewData data;
	public void UpdateView (AchievementViewData data)
	{
		this.data = data;
		switch (data.status) {
		case AchievementViewData.Status.PROGRESSING:
				ShowCollectBuy(false, data.Bonus.FormattedString);
			break;
		case AchievementViewData.Status.PENDING_COLLECT:
				ShowCollectBuy(true, data.Bonus.FormattedString);
			break;
		case AchievementViewData.Status.PASSED:
			SetComplete ();
			break;
		default:
			break;
		}
	}

	private void SetComplete() {
        coinText.text = "Done!";
	}

	private void ShowCollectBuy(bool isShow, string numberGold) {
		coinText.text = "+" + numberGold;

		if (isShow) {
			coinText.color = goldEnableColor;
		} else {
			coinText.color = goldDisableColor;
		}
	}
}

