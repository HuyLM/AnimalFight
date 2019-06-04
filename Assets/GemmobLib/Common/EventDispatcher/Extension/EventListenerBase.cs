using System;
using UnityEngine;

namespace Gemmob.Common {
    public class EventListenerBase : MonoBehaviour {
        protected Action<bool> listener;

        public void SetListener<T>(Action<T> Action) where T : EventParams {
            listener = active => {
                if (active)
                    EventDispatcher.Instance.AddListener(Action);
                else
                    EventDispatcher.Instance.RemoveListener(Action);
            };

            listener(true);
        }

        public void SetListener<T>(Action Action) where T : EventParams {
            listener = active => {
                if (active)
                    EventDispatcher.Instance.AddListener<T>(Action);
                else
                    EventDispatcher.Instance.RemoveListener<T>(Action);
            };

            listener(true);
        }

        public void SetListener(string key, Action Action) {
            listener = active => {
                if (active)
                    EventDispatcher.Instance.AddListener(key, Action);
                else
                    EventDispatcher.Instance.RemoveListener(key, Action);
            };

            listener(true);
        }

        public void SetListener(string key, Action<object> Action) {
            listener = active => {
                if (active)
                    EventDispatcher.Instance.AddListener(key, Action);
                else
                    EventDispatcher.Instance.RemoveListener(key, Action);
            };

            listener(true);
        }

        public void SetListener(int key, Action Action) {
            listener = active => {
                if (active)
                    EventDispatcher.Instance.AddListener(key, Action);
                else
                    EventDispatcher.Instance.RemoveListener(key, Action);
            };

            listener(true);
        }

        public void SetListener(int key, Action<object> Action) {
            listener = active => {
                if (active)
                    EventDispatcher.Instance.AddListener(key, Action);
                else
                    EventDispatcher.Instance.RemoveListener(key, Action);
            };

            listener(true);
        }

        public void SetListener(EventKey eventkey, Action<object> Action) {
            listener = active => {
                if (active)
                    EventDispatcher.Instance.AddListener(eventkey, Action);
                else
                    EventDispatcher.Instance.RemoveListener(eventkey, Action);
            };

            listener(true);
        }

        public void SetListener(EventKey eventkey, Action action) {
            listener = active => {
                if (active)
                    EventDispatcher.Instance.AddListener(eventkey, action);
                else
                    EventDispatcher.Instance.RemoveListener(eventkey, action);
            };

            listener(true);
        }

    }
}