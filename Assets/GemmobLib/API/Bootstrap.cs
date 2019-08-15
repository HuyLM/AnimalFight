using UnityEngine;

public class Bootstrap : MonoBehaviour {
    [SerializeField] private bool preloadFirebase = true;
    [SerializeField] private bool preloadAds = true;

    private void Start() {
        if (preloadFirebase) {

        }

        if (preloadAds) {
            var m = Mediation.Instance;
        }
    }
}
