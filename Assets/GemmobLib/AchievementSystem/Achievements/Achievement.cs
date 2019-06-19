using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public abstract class Achievement : ScriptableObject
{
	public System.Action<Achievement> OnProgressChanged = delegate { };
	public System.Action<Achievement> OnStarChanged = delegate { };

	#region Public Serialized Fields
	public string id;

	public Sprite sprite;
	public double minBonus;
	[BitMask(typeof(AchievementEvent))] 
	public long eventMasks;
	public Star[] stars = new Star[5];
	#endregion

	#region Private 
	private int collectedCount;
	protected SecuredDouble current = new SecuredDouble(0);
	private List<AchievementEvent> events = new List<AchievementEvent>();
	private AchievementViewData viewData = new AchievementViewData (); 
	#endregion

	public virtual void WakeUp ()
	{
		Events = Extensions.GetEnumValues<AchievementEvent> (eventMasks);	
		collectedCount = 0;
		UpdateCurrentStar ();
	}

    public string DescriptionForm;

	public static SecuredDouble BaseBonus {
		get {
			return 0;
		}
	}

	public AchievementViewData CurrentViewData {
		get {
			Star star;
			star = PendingCollectStar;
			if (star !=  null) {
				return viewData.Update (
					status: AchievementViewData.Status.PENDING_COLLECT,
					description: star.Description (current, DescriptionForm),
					progressString: star.Progress(current),
					bonusFactor: star.bonusFactor,
					minBonus: minBonus,
					stars: StarsPassed,
					progress: 1);
			}

			star = CurrentStar;
			if (star != null) {
				return viewData.Update (
					status: AchievementViewData.Status.PROGRESSING,
					description: star.Description (current, DescriptionForm),
					progressString: star.Progress(current),
					bonusFactor: star.bonusFactor,
					minBonus: minBonus,
					stars: StarsPassed,
					progress: (float)(current / star.target));
			}

			star = stars [stars.Length - 1];
			return viewData.Update (
				status: AchievementViewData.Status.PASSED,
				description: star.Description (current, DescriptionForm),
				progressString: star.Progress(current),
				bonusFactor: star.bonusFactor,
				minBonus: minBonus,
				stars: StarsPassed,
				progress: 1);
		}
	}

	public SecuredDouble Total
	{
		get
		{
			return stars[stars.Length - 1].target;
		}
	}

	public List<AchievementEvent> Events
	{
		get { return events; }
		private set { events = value; }
	}

	public int StarsPassed
	{ 
		get
		{
			int total = 0;
			for (int i = 0; i < stars.Length; i++)
			{
				if (stars[i].target >= current)
				{
					total++;
				}
			}
			return total;
		}
	}

	public int TotalStarCollected
	{
		get { return collectedCount; }
	}

	public bool Passed
	{
		get { return current >= Total; }
	}

	/**
	 * Return the current processing star, null if all passed 
	 **/
	public Star CurrentStar {
		get;
		private set;
	}

	public bool Active
	{
		get;
		set;
	}

	public int Index
	{
		get;
		set;
	}

	private int FirstPendingCollectStarIndex {
		get {
			if (collectedCount < stars.Length && current >= stars[collectedCount].target) {
				return collectedCount;
			}
			return -1;
		}
	}

	public int PendingCollectStarCount {
		get {
			int t = 0;
			for (int i = collectedCount; i < stars.Length; i++) {
				if (stars [i].target <= current)
					t++;
			}
			return t;
		}
	}

	public Star PendingCollectStar {
		get {
			var i = FirstPendingCollectStarIndex;
			return i >= 0 ? stars[i] : null;
		}
	}

	private void UpdateCurrentStar ()
	{
		int i = 0;
		while (i < stars.Length && stars[i].target <= current) {
			i++;
		}

		if (i < stars.Length)
			CurrentStar = stars [i];
		else
			CurrentStar = null;
	}

	public SecuredDouble Collect ()
	{
		var index = FirstPendingCollectStarIndex;
		if (index < 0)
		{
			return 0;
		}

//		results [index].collected = true;
		collectedCount++;
		//EventDispatcher.Instance.Dispatch(EventName.ACHIEVEMENT_BADGE_COUNT_CHANGED, null);
        return  stars[index].bonusFactor;
	}

	public void SetEvent(params AchievementEvent[] events)
	{
		this.events.Clear();
		this.events.AddRange(events);
	}

	public AchievementProcessResult ProcessEvent(AchievementEvent eventType, SecuredDouble data)
	{
		var oldStar = CurrentStar;
		var vr = OnEvent(eventType, data);
		UpdateCurrentStar ();

		if (oldStar != CurrentStar) {
			OnStarChanged.Invoke (this);
			//EventDispatcher.Instance.Dispatch (EventName.ACHIEVEMENT_BADGE_COUNT_CHANGED, null);
		} else {
			OnProgressChanged.Invoke(this);	
		}

		return vr;
	}

	protected abstract AchievementProcessResult OnEvent(AchievementEvent eventType, SecuredDouble data);

	public PersistentData Data()
	{
		var data = new PersistentData();
		data.current = current;
		data.collected = collectedCount;
		return data;
	}

	public void Bind(Achievement.PersistentData data)
	{
		var thisData = (PersistentData)data;
		this.current = thisData.current;
		int passedCount = 0;
		for (int i = 0; i < stars.Length; i++) {
			if (stars[i].target <= current) {
				passedCount++;
			} else {
				break;
			}
		}

		this.collectedCount = Mathf.Min (passedCount, data.collected);
		UpdateCurrentStar ();
	}

	[System.Serializable]
	public class PersistentData
	{
		public SecuredDouble current;
		public int collected;
	}

	[System.Serializable]
	public class Star
	{
		public SecuredDouble target;
		public SecuredDouble bonusFactor = 1;

		public Star(SecuredDouble value, float bonus)
		{
			this.target = value;
			this.bonusFactor = bonus;
		}

		public string Description (SecuredDouble current, string descriptionForm)
		{
			return descriptionForm.Replace("{X}", target.ToString());
		}

		public string Progress (SecuredDouble current)
		{
			return SecuredDouble.Min(current, target).ToString() + "/" + target.ToString();
		}
	}
}

public enum AchievementProcessResult
{
	PROGRESSING,
	PASSED,
	FAILED,
}

public class AchievementViewData
{
	public Status status = Status.PROGRESSING;
	public string description = "";
	public string progressString = "";
	public SecuredDouble bonusFactor = 0;
	public SecuredDouble minBonus = 0;
	public int stars = 0;
	public float progress = 0;

	public AchievementViewData ()
	{
		
	}

	public AchievementViewData (Status status, string description, string progressString, SecuredDouble bonusFactor, SecuredDouble minBonus, int stars, float progress)
	{
		this.status = status;
		this.description = description;
		this.progressString = progressString;
		this.bonusFactor = bonusFactor;
		this.minBonus = minBonus;
		this.stars = stars;
		this.progress = progress;
	}

	public AchievementViewData Update (Status status, string description, string progressString, SecuredDouble bonusFactor, SecuredDouble minBonus, int stars, float progress)
	{
		this.status = status;
		this.description = description;
		this.progressString = progressString;
		this.bonusFactor = bonusFactor;
		this.minBonus = minBonus;
		this.stars = stars;
		this.progress = progress;

		return this;
	}

	public SecuredDouble Bonus
	{
        get { return bonusFactor; }
	}

	public enum Status {
		PROGRESSING,
		PENDING_COLLECT,
		PASSED
	}
}