using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SoundManager))]
public class SoundManagerEditor : Editor {

	public override void OnInspectorGUI () {
		DrawDefaultInspector ();

        if (!Application.isPlaying) return;
        
		SoundManager sfx = (SoundManager)target;
		if (GUILayout.Toggle(sfx.BackgroundMusicEnable, "Bg Music Enable")) {
            sfx.BackgroundMusicEnable = !sfx.BackgroundMusicEnable;
		}

        if (GUILayout.Toggle(sfx.SoundEffectEnable, "Sound Efefct Enable")) {
            sfx.SoundEffectEnable = !sfx.SoundEffectEnable;
        }
        
		if (GUILayout.Button("Play Background Music")) {
			sfx.PlayBackgroundMusic ();
		}

		if (GUILayout.Button("Stop Background Music")) {
			sfx.StopBackgroundMusic ();
		}

        //if (GUILayout.Button("Fake Start Watch Ads")) {
        //    EventDispatcher.Dispatch(SoundManager.OnAdsAudioStart, null);
        //}

        //if (GUILayout.Button("Fake Finish Watch Ads")) {
        //    EventDispatcher.Dispatch(SoundManager.OnAdsAudioFinish, null);
        //}
	}
}