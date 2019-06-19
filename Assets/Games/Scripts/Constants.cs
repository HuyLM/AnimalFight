using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class Constants 
{
    public static readonly int CoinFree = 50;

    public static Vector3 BottomLeft
    {
        get
        {
            return Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        }
    }

    public static Vector3 UpRight
    {
        get
        {
            return Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));
        }
    }

    public const float TimeShowPopup = 0.3f;

	public static void ValidateProperties ()
	{
		List<string> values = new List<string> ();
		var fieldInfos = typeof(Constants).GetFields (BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		foreach (var f in fieldInfos) {
			if (f.FieldType == typeof(string)) {
				string v = f.GetValue (null) as string;
				if (values.Contains (v)) {
					Debug.LogErrorFormat ("Duplicate value: field {0}, value {1}", f.Name, v);
				}
				values.Add (v);
			}
		}
	}
}