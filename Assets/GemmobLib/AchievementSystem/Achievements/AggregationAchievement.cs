using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="Aggregation", menuName="Achievements/AchievementAggregation")]
public class AggregationAchievement : Achievement
{

	protected override AchievementProcessResult OnEvent (AchievementEvent eventType, SecuredDouble data)
	{
		current += data;

		return Passed ? AchievementProcessResult.PASSED : AchievementProcessResult.PROGRESSING;
	}
}
