using System.Diagnostics;
using UnityEngine;

public class Bootstrap : MonoBehaviour {
    [SerializeField] private int targetFPS = 60;
    [SerializeField] private bool preloadFirebase = true;
    [SerializeField] private bool preloadAds = true;

    private void Awake() {
        Application.targetFrameRate = targetFPS;
    }

    private void Start() {
        if (preloadFirebase) {

        }

        if (preloadAds) {
            //var m = Mediation.Instance;
        }
    }

}
