using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubManager : Manager
{
    protected virtual void Awake() {
        if (GameManager.Instance != null) {
            if (!Initialized) {
                Init();
            }
            GameManager.Instance.AddThread(this);
        }
    }

    protected virtual void OnDestroy() {
        if (GameManager.Instance != null) {
            GameManager.Instance.RemoveThread(this);
        }
    }
}
