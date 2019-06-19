using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomPropertyDrawer(typeof(BitMaskAttribute))]
public class EnumBitMaskPropertyDrawer : PropertyDrawer
{
	public override void OnGUI (Rect position, SerializedProperty prop, GUIContent label)
	{
		var typeAttr = attribute as BitMaskAttribute;
		label.text = label.text + " ("+prop.longValue+")";
		prop.longValue = DrawBitMaskField(position, prop.longValue, typeAttr.propType, label);
	}

	public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
	{
		var type = attribute as BitMaskAttribute;
		var names = System.Enum.GetNames (type.propType);
		return base.GetPropertyHeight (property, label) * (Mathf.CeilToInt(names.Length / 2.0f) + 1);
	}

	public static long DrawBitMaskField (Rect position, long value, System.Type type, GUIContent label)
	{
		var names = System.Enum.GetNames (type);
		var values = System.Enum.GetValues (type) as int[];

		int rows = Mathf.CeilToInt(names.Length / 2.0f) + 1;
		float dy = position.height / rows;
		var rect = position;
		rect.height = dy;
		long newValue = 0;

		EditorGUI.LabelField (rect, label);

		rect.width /= 2;
		EditorGUI.indentLevel++;
		for (int i = 0; i < names.Length; i++) {
			rect.position = new Vector2(position.position.x + (i % 2) * rect.width, position.position.y + (i/2 + 1) * dy);
			var v = EditorGUI.Toggle (rect, names [i], (value & (1 << values[i])) != 0);

			if (v) {
				newValue |= 1 << values [i];
			}
		}
		EditorGUI.indentLevel--;

		return newValue;
	}
}

public static class EditorExtension
{
	public static int DrawBitMaskField (Rect aPosition, int aMask, System.Type aType, GUIContent aLabel)
	{
		var itemNames = System.Enum.GetNames(aType);
		var itemValues = System.Enum.GetValues(aType) as int[];

		int val = aMask;
		int maskVal = 0;
		for(int i = 0; i < itemValues.Length; i++)
		{
			if (itemValues[i] != 0)
			{
				if ((val & itemValues[i]) == itemValues[i])
					maskVal |= 1 << i;
			}
			else if (val == 0)
				maskVal |= 1 << i;
		}
		int newMaskVal = EditorGUI.MaskField(aPosition, aLabel, maskVal, itemNames);
		int changes = maskVal ^ newMaskVal;

		for(int i = 0; i < itemValues.Length; i++)
		{
			if ((changes & (1 << i)) != 0)            // has this list item changed?
			{
				if ((newMaskVal & (1 << i)) != 0)     // has it been set?
				{
					if (itemValues[i] == 0)           // special case: if "0" is set, just set the val to 0
					{
						val = 0;
						break;
					}
					else
						val |= itemValues[i];
				}
				else                                  // it has been reset
				{
					val &= ~itemValues[i];
				}
			}
		}
		return val;
	}
}