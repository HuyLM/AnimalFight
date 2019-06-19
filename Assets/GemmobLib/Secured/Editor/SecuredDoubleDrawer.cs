using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer (typeof(SecuredDouble))]
public class SecuredDoubleDrawer : PropertyDrawer
{

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		// Using BeginProperty / EndProperty on the parent property means that
		// prefab override logic works on the entire property.
		EditorGUI.BeginProperty (position, label, property);

		// Draw label
		position = EditorGUI.PrefixLabel (position, GUIUtility.GetControlID (FocusType.Passive), label);

		// Don't make child fields be indented
		int indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		Rect valueRect = new Rect (position.x, position.y, position.width * 0.7f, position.height);
		Rect formatRect = new Rect (position.x + position.width * 0.7f, position.y, position.width * 0.3f, position.height);

		var valueProp = property.FindPropertyRelative ("masked");
		var keyProp = property.FindPropertyRelative ("key");
		var key = keyProp.longValue;
		var value = valueProp.doubleValue;
		var v = EditorGUI.DoubleField (valueRect, SecuredDouble.Encrypt (key, value));

		if (GUI.Button (formatRect, "↑key↓")) {
			key = SecuredDouble.RandomKey ();
			keyProp.longValue = key;
		}

		valueProp.doubleValue = SecuredDouble.Encrypt (key, v);
		// Set indent back to what it was
		EditorGUI.indentLevel = indent;

		EditorGUI.EndProperty ();
	}
}
