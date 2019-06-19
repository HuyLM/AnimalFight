using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="AchievementReach", menuName="Achievements/Reach")]
public class ReachAchievement : Achievement
{
	protected override AchievementProcessResult OnEvent (AchievementEvent eventType, SecuredDouble data)
	{
		current = SecuredDouble.Min(SecuredDouble.Max(current, data), Total);

		return Passed ? AchievementProcessResult.PASSED : AchievementProcessResult.PROGRESSING;
	}
}
