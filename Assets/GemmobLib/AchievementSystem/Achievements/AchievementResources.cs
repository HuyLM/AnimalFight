using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName="Achievement Database", fileName="AchievementDatabase")]
public class AchievementResources : ScriptableObject {
	public Achievement[] achievements;

	static AchievementResources instance;

	public static AchievementResources Instance {
		get {
			if (instance == null) {
				instance = Resources.Load<AchievementResources> ("AchievementResources");
			}
			return instance;
		}
	}


	public static void ReleaseInstance () {
		if (instance != null) {
			foreach (var item in instance.achievements) {
				Resources.UnloadAsset (item);
			}
			Resources.UnloadAsset (instance);
		}
		instance = null;
	}

}
