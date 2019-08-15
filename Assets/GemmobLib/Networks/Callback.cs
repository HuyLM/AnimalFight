using UnityEngine;
using UniRx;
using System;

namespace Gemmob.Networks {
    public class Callback {

        public static void Call(Action callback, float delayTime = 0) {
            if (callback != null) {
                Scheduler.MainThreadIgnoreTimeScale.Schedule(TimeSpan.FromSeconds(delayTime), time => {
                    callback.Invoke();
                });
            }
        }
    }
}