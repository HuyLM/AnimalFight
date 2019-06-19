using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementView : MonoBehaviour
{
	public string id {
		get;
		private set;
	}

	[SerializeField] private Text nameView, progress;
	[SerializeField] private StarView starView;
	[SerializeField] private ProgressView progressView;
	[SerializeField] private AchievementButton achievementButton;

	private Achievement achievement;

	public void UpdateView(Achievement achievement)
	{
		UnsubscribeEvents ();

		this.achievement = achievement;
		this.id = achievement.id;

		var data = achievement.CurrentViewData;
		nameView.text = data.description;
		progress.text = data.progressString;

        starView.UpdateView(achievement.TotalStarCollected,achievement.stars.Length);
		progressView.SetProgress(data.progress);
		achievementButton.UpdateView(data);

		if (data.status == AchievementViewData.Status.PASSED) {
			SetComplete ();
		}

		if (data.status == AchievementViewData.Status.PROGRESSING) {
			achievement.OnProgressChanged += OnProgressChanged;
			achievement.OnStarChanged += OnStarChanged;
		}
	}

	void OnDisable ()
	{
		UnsubscribeEvents ();
	}

	void UnsubscribeEvents ()
	{
		if (achievement != null) {
			achievement.OnStarChanged -= OnStarChanged;
			achievement.OnProgressChanged -= OnProgressChanged;
		}
	}

	private void OnProgressChanged (Achievement achievement)
	{
		var data = achievement.CurrentViewData;
		progressView.SetProgress(data.progress);
		progress.text = data.progressString;
	}

	private void OnStarChanged (Achievement achievement)
	{
		UpdateView (achievement);
	}
		
	private void SetComplete() {
		progress.gameObject.SetActive (false);
		progressView.gameObject.SetActive (false);
	}

	public void Collect ()
	{
		var bonus = achievement.Collect ();
        // bonus with .....
        Debug.Log("Collect");

		UpdateView (achievement);
	}
}