using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SoundManager))]
public class SoundManagerEditor : Editor {

	public override void OnInspectorGUI () {
		DrawDefaultInspector ();

        if (!Application.isPlaying) return;
        
		SoundManager sfx = (SoundManager)target;
		if (GUILayout.Button(sfx.BackgroundMusicEnable ? "Set Bg Music Disable" : "Set Bg Music Enable")) {
            sfx.BackgroundMusicEnable = !sfx.BackgroundMusicEnable;
		}

        if (GUILayout.Button(sfx.SoundEffectEnable ? "Set Sound Disable" : "Set Sound Enable")) {
            sfx.SoundEffectEnable = !sfx.SoundEffectEnable;
        }
        
		if (GUILayout.Button("Play Background Music")) {
			sfx.PlayBackgroundMusic ();
		}

		if (GUILayout.Button("Stop Background Music")) {
			sfx.StopBackgroundMusic ();
		}
	}
}