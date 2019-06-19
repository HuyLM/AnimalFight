using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AchievementSystem
{
	public const string BADGE_CHANGED_EVENT = "AchievementSystem.BadgeChanged";

	List<Achievement> achievements = new List<Achievement> ();
	Dictionary<AchievementEvent, List<Achievement>> eventMap = new Dictionary<AchievementEvent, List<Achievement>> ();
	AchievementResources resources;
	AchievementStorage storage;
	Achievement currentAchievement;

	static AchievementSystem instance;

	public static AchievementSystem Instance {
		get { 
			if (instance == null) {
				instance = new AchievementSystem ();
				instance.LoadAchievements ();
			}

			return instance;
		}
	}

	public static void UnloadInstance ()
	{
		instance = null;
		AchievementResources.ReleaseInstance ();
	}

	public List<Achievement> GetAllAchievement ()
	{
		return achievements;
	}

	public int BadgeCount {
		get {
			int count = 0;
			foreach (Achievement achi in achievements) {
				count += achi.PendingCollectStarCount;
			}
			return count;
		}
	}

	#if UNITY_EDITOR
	public static void ClearData ()
	{
		if (instance != null) {
			instance.Release ();
			instance = null;
		}

		AchievementResources.ReleaseInstance ();
		AchievementStorage.ClearData ();
	}

	public static void ReleaseInstance ()
	{
		instance = null;
		AchievementResources.ReleaseInstance ();
		AchievementStorage.ReleaseInstance ();
	}
	#endif

	public AchievementSystem ()
	{
		resources = AchievementResources.Instance;
		storage = AchievementStorage.Instance;
	}

	private void LoadAchievements ()
	{
		achievements.Clear ();
		achievements.AddRange (resources.achievements);
		int i = 0;
		foreach (var a in achievements) {
			a.WakeUp ();

			if (string.IsNullOrEmpty (a.id)) {
				Debug.LogError ("ID is null " + a.DescriptionForm);	
			}
			var data = storage [a.id];
			if (data != null) {
				a.Bind (data);
			}
			if (!a.Passed) {
				ActivateAchievement (a);
			}
			a.Index = ++i;
		}
	}

	public void ActivateAchievement (Achievement achievement)
	{
		var events = achievement.Events;
		foreach (var e in events) {
			List<Achievement> acs;
			if (!eventMap.TryGetValue (e, out acs)) {
				acs = new List<Achievement> ();
				eventMap [e] = acs;
			}
			acs.Add (achievement);
			achievement.Active = true;
		}

//		Logger.Debug ("Activated: " + achievement.Description);
	}

	public void DeactivateAchievement (Achievement achievement)
	{
		var events = achievement.Events;
		foreach (var e in events) {
			List<Achievement> acs;
			if (eventMap.TryGetValue (e, out acs)) {
				acs.Remove (achievement);	
			}
		}
		achievement.Active = false;

//		Logger.Debug ("Deactivated: " + achievement.Description);
	}

	public Achievement CurrentAchievement {
		get { 
			return currentAchievement;
		}
	}

	public int Count {
		get { return achievements.Count; }
	}

	void OnAchievementPassed (Achievement achievement)
	{
//		Logger.Debug ("Passed: " + achievement.Description);
	}

	public void TriggerEvent (AchievementEvent eventType, SecuredDouble data)
	{
		List<Achievement> acs;
		var tobeRemoved = new List<Achievement> ();

		if (eventMap.TryGetValue (eventType, out acs)) {
			foreach (var a in acs) {
				var ret = a.ProcessEvent (eventType, data);

				if (ret == AchievementProcessResult.PASSED || ret == AchievementProcessResult.FAILED) {
					tobeRemoved.Add (a);
				}

				storage [a.id] = a.Data ();
//				Globals.hud.trophiesView.achievementsView.UpdateAchievement(a);
			}

			foreach (var a in tobeRemoved) {
				DeactivateAchievement (a);
				if (a.Passed)
					OnAchievementPassed (a);
			}
		}
			
	}

	public void OnGameStart ()
	{
		if (currentAchievement == null)
			return;
		ActivateAchievement (currentAchievement);
	}

	public void OnGameOver ()
	{
		if (currentAchievement == null)
			return;
		DeactivateAchievement (currentAchievement);
	}

	public void Save ()
	{
		for (int i = 0; i < resources.achievements.Length; i++) {
			storage [resources.achievements [i].id] = resources.achievements [i].Data ();
		}
		storage.Save ();
	}

	public void Release ()
	{
		storage.Close ();
	}

	//	public void Log () {
	//		int total = achievements.Count;
	//		foreach (var a in achievements) {
	//			var p = (a is IProgress) ? ((IProgress)a).ProgressString : "--";
	//			Debug.LogFormat ("[{0}/{1} {2}] Passed=[{3}] Progress=[{4}] Desc=[{5}]",
	//				a.Index, total, a.id, a.Passed, p, a.description);
	//		}
	//
	//		if (currentAchievement != null) {
	//			Debug.Log ("Current Index = " + currentAchievement.Index);
	//		}
	//	}
}


