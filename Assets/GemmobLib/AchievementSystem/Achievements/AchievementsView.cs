using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementsView : MonoBehaviour
{
	[SerializeField] private GameObject achievementViewPrefab;

	void Start ()
	{
		achievementViewPrefab.SetActive (false);
	}

	void OnEnable()
	{
		LoadAllAchievement();
	}

	void OnDisable()
	{
		for (int i = transform.childCount - 1; i >= 0; i--)
		{
			if (transform.GetChild(i).gameObject != achievementViewPrefab)
				Destroy(transform.GetChild(i).gameObject);
		}
	}

	public void LoadAllAchievement() {
		LoadData(AchievementSystem.Instance.GetAllAchievement());
	}

	public void LoadData(List<Achievement> data)
	{
		bool showBG = false;
		foreach (Achievement achi in data)
		{
			GameObject g = Instantiate(achievementViewPrefab, transform) as GameObject;
			g.SetActive (true);
			g.transform.localScale = new Vector3(1, 1, 1);
			g.transform.localPosition = Vector3.zero;
			g.GetComponent<AchievementView>().UpdateView(achi);
		}
	}
}