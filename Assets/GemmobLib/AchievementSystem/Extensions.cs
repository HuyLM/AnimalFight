using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System;
using TMPro;

public static class Extensions {

	public static Vector3 AverageNormal (this Collision collison) {
		Vector3 normal = Vector3.zero;
		foreach (var i in collison.contacts) {
			normal += i.normal;
		}
		if (normal.sqrMagnitude > Mathf.Epsilon) {
			normal.Normalize ();
		}
		return normal;
	}

	public static Vector3 Multiply (this Vector3 a, Vector3 b) {
		return new Vector3 (a.x * b.x, a.y * b.y, a.z * b.z);
	}

	public static string GetResponseBody (this UnityWebRequest rq) 
	{
		if (rq.downloadHandler == null || rq.downloadHandler.data == null)
			return null;
		
		return System.Text.Encoding.UTF8.GetString (rq.downloadHandler.data);
	}

	public static List<T> GetEnumValues<T> (long mask, bool bitmask=false) where T : struct
	{
		Type type = typeof(T);
		if (!type.IsEnum) {
			Debug.LogErrorFormat ("{0} is not an enum", type.ToString());
			return null;
		}

		var values = Enum.GetValues (type) as int[];
		var ret = new List<T> ();
		for (int i = 0; i < values.Length; i++) {
			var v = values [i];
			if (
				(bitmask && (mask & v) != 0) ||
				(!bitmask && (mask & (1 << v)) != 0)
			) {
				ret.Add ((T)(object)v);
			}
		}
		return ret;
	}

    public static float GetTextSize(this TextMesh textMesh)
    {
        float width = 0;
        foreach (char symbol in textMesh.text)
        {
            CharacterInfo info;
            if (textMesh.font.GetCharacterInfo(symbol, out info, textMesh.fontSize, textMesh.fontStyle))
            {
                width += info.advance;
            }
        }
        float finalWidth = width * textMesh.characterSize * textMesh.transform.lossyScale.x * 0.1f;
        return finalWidth;
    }

    public static float GetTextSize(this TextMeshPro textMesh)
    {
        return textMesh.preferredWidth;
    }


    public static void SetSizeX(this Camera camera, float width, float offset)
    {
        float size = 0;
        size = (width * Screen.height / Screen.width * 0.5f) + offset;
        camera.orthographicSize = size;
    }

#if false
	public static Dictionary<string, object> GetJsonResponse (this UnityWebRequest request)
	{
		var body = request.GetResponseBody ();
		if (body == null) {
			return null;
		}

		try {
			var json = MiniJSONV.Json.Deserialize (body);
			return json as Dictionary<string, object>;	
		} catch (System.Exception e) {
			Debug.LogError (e.Message);
			return null;
		}
	}
#endif
}
