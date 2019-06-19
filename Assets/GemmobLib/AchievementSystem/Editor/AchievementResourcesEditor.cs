using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using System.IO;
using System.Linq;

[CustomEditor(typeof(AchievementResources))]
public class AchievementResourcesEditor : Editor {
	
	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		AchievementResources cb = (AchievementResources)target;

		EditorGUILayout.BeginHorizontal ();

		if (GUILayout.Button("Load From Folder")) {
			var path = EditorUtility.OpenFolderPanel ("Select Folder", Application.dataPath + "/Resources/Achievements", "");

			if (string.IsNullOrEmpty (path))
				return;

			var achievements = new List<Achievement> ();
			var files = Directory.GetFiles (path);

			foreach (var f in files) {
				if (f.EndsWith("asset")) {
					var relPath = FileUtil.GetProjectRelativePath (Path.Combine (path, f));
					var a = AssetDatabase.LoadAssetAtPath<Achievement> (relPath);
					if (a) {
						if (string.IsNullOrEmpty(a.id)) {
							a.id = System.Guid.NewGuid ().ToString();
						}
						EditorUtility.SetDirty (a);
						achievements.Add (a);
					}
				}
			}

			cb.achievements = achievements.ToArray ();

			var ids = new HashSet<string> ();
			foreach (var a in cb.achievements) {
				ids.Add (a.id);
			}

			if (ids.Count != cb.achievements.Length) {
				Debug.LogError ("Duplicate ID");
			}

			EditorUtility.SetDirty (cb);
		}

		if (GUILayout.Button ("Ensure Unique ID")) {
			foreach (var a in cb.achievements) {
				if (string.IsNullOrEmpty(a.id)) {
					a.id = System.Guid.NewGuid ().ToString();
				}
				EditorUtility.SetDirty (a);
			}

			AssetDatabase.SaveAssets ();

			var ids = new HashSet<string> ();
			foreach (var a in cb.achievements) {
				ids.Add (a.id);
			}

			if (ids.Count != cb.achievements.Length) {
				Debug.LogError ("Duplicate ID");
			}
		}

		EditorGUILayout.EndHorizontal ();

		if (GUILayout.Button("Unload")) {
			foreach (var item in AchievementResources.Instance.achievements) {
				Resources.UnloadAsset (item);
			}

			Resources.UnloadAsset (AchievementResources.Instance);
		}
		if (GUILayout.Button("Auto Setup Parameter(for test)")) {
			foreach (Achievement achi in cb.achievements) {
				if (achi.DescriptionForm == "") {
//					achi.descriptionForm = achi.name;
				}

				for (int i = 0; i < achi.stars.Length; i++)
				{
					if (achi.stars[i].target.Value == 0 && achi.stars[i].bonusFactor == 0)
						achi.stars[i] = new Achievement.Star(5 * (i + 1), i + 1);
				}
				EditorUtility.SetDirty (achi);
			}
		}
	}

//	[MenuItem("Screw/Achievements/Unload Assets")]
	public static void UnloadAchievementAssets () {
		AssetDatabase.SaveAssets ();

		AchievementSystem.ReleaseInstance ();

		var path = Application.dataPath + "/Resources/Achievements/AchievementResources.asset";
		var relPath = FileUtil.GetProjectRelativePath (path);
		Debug.Log ("REL = " + relPath);
		var asset = AssetDatabase.LoadAssetAtPath<AchievementResources> (relPath);
		Debug.Log ("Asset = " + asset);
		if (asset != null) {
			Resources.UnloadAsset (asset);
			Debug.Log ("Unload " + asset);
		}

		path = Application.dataPath + "/Resources/Achievements";

		var files = Directory.GetFiles (path);

		foreach (var f in files) {
			if (f.EndsWith("asset")) {
				relPath = FileUtil.GetProjectRelativePath (Path.Combine (path, f));
				var a = AssetDatabase.LoadAssetAtPath<Achievement> (relPath);
				if (a) {
					Resources.UnloadAsset (a);
					Debug.Log ("Unlooad " + a);
				}
			}
		}
	}
}