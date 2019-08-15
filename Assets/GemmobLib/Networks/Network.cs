using UnityEngine;

public static class Network {
    public static bool IsInternetAvaiable {
        get {
            return Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork ||
                    Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
        }
    }
}