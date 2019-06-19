using UnityEngine;
using System.Collections;

public enum AchievementEvent {
	COLLECT_GEM
}

public class BitMaskAttribute : PropertyAttribute
{
    public System.Type propType;

    public BitMaskAttribute(System.Type type)
    {
        propType = type;
    }
}