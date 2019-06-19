using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

public class AchievementStorage {
	private BinaryFormatter formatter;
	private Dictionary<string, Achievement.PersistentData> data;

    private static string FilePath
    {
        get
        {
            return Application.persistentDataPath + "/achievements.dat";
        }
    }


    static AchievementStorage instance;

	public static AchievementStorage Instance {
		get {
			if (instance == null) {
				instance = new AchievementStorage ("achievements.dat");
			}

			return instance;
		}
	}

	#if UNITY_EDITOR
	public static void ClearData () {
		if (instance != null) {
			instance.Close ();
		}
		instance = null;
		File.Delete (FilePath);
		Debug.Log ("Deleted " + FilePath);
	}

	public static void ReleaseInstance () {
		if (instance != null) {
			instance.Close ();
		}
		instance = null;
	}
	#endif

	public AchievementStorage (string filename) {
		formatter = new BinaryFormatter ();
		Path = FilePath;

		Reload ();
	}

	public Dictionary<string, Achievement.PersistentData> Data {
		get { return data; }
	}

	public string Path {
		get;
		private set;
	}

	public void Save () {
		using (var stream = new FileStream (Path, FileMode.Create)) {
			stream.SetLength (0);
			formatter.Serialize (stream, data);	
			stream.Flush ();
		}

	}

	public void Save (Dictionary<string, Achievement.PersistentData> data) {
		if (data == null) {
			throw new SystemException ("Cannot save Achievement Data");
		}
		this.data = data;
		Save ();
	}

	public void Reload () {
		try {
			using (var stream = new FileStream (Path, FileMode.OpenOrCreate)) {
				data = formatter.Deserialize (stream) as Dictionary<string, Achievement.PersistentData>;	
			}
		} catch (Exception e) {
			Debug.Log ("Exception " + e.ToString());
		}

		if (data == null) 
			data = new Dictionary<string, Achievement.PersistentData> ();
	}

	public void Close () {
//		stream.Close ();
	}

	public Achievement.PersistentData this[string key] {
		get { Achievement.PersistentData pd;
			if (data.TryGetValue(key, out pd)) {
				return pd;		
			} 

			return null;
		}
		set { 
			if (value == null) {
				data.Remove (key);	
			} else {
				data [key] = value;	
			}
		}
	}
}
