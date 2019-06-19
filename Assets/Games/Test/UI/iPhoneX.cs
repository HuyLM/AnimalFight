using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class iPhoneX : MonoBehaviour {
    public Image top;
    public Image bottom;

    void OnEnable() {
#if !UNITY_EDITOR
        gameObject.SetActive(false);
#else
        bottom.gameObject.SetActive(Device.iPhoneX);
        top.gameObject.SetActive(Device.iPhoneX);
#endif
    }
}
