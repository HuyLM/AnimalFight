using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AdsTest : MonoBehaviour
{
    public void ShowInterstital() {
        Mediation.Instance.ShowInterstitial("test");
    }

    public void ShowRewardVideo() {
        Mediation.Instance.ShowRewardVideo("test");
    }

    public void ShowBannerBottom() {
        Mediation.Instance.ShowBannerBottom("test");
    }

    public void ShowBannerTop() {
        Mediation.Instance.ShowBannerTop("test");
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(AdsTest))]
public class AdsTestEditor : Editor {
    public override void OnInspectorGUI() {
        if (!Application.isPlaying) return;

        DrawDefaultInspector();

        var test = target as AdsTest;
        if (GUILayout.Button("Show Fake Interstial")) {
            test.ShowInterstital();
        }

        if (GUILayout.Button("Show Fake Reward Video")) {
            test.ShowRewardVideo();
        }

        if (GUILayout.Button("Show Fake Banner Bottom")) {
            test.ShowBannerBottom();
        }

        if (GUILayout.Button("Show Fake Banner Top")) {
            test.ShowBannerTop();
        }
    }

}
#endif