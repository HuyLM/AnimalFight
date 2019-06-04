using System;
using UnityEngine;

namespace Gemmob.Common {
    public static class EventDispatcherExtension {
        public static void RegisterListener(this MonoBehaviour mono, string key, Action<object> action, bool untilDisable = true) {
            if (untilDisable) GetComponent<EvenDisableListener>(mono, key, action);
            else GetComponent<EventDestroyListener>(mono, key, action);
        }

        public static void RegisterListener(this MonoBehaviour mono, string key, Action action, bool untilDisable = true) {
            if (untilDisable) GetComponent<EvenDisableListener>(mono, key, action);
            else GetComponent<EventDestroyListener>(mono, key, action);
        }

        public static void RegisterListener(this MonoBehaviour mono, int key, Action<object> action, bool untilDisable = true) {
            if (untilDisable) GetComponent<EvenDisableListener>(mono, key, action);
            else GetComponent<EventDestroyListener>(mono, key, action);
        }

        public static void RegisterListener(this MonoBehaviour mono, int key, Action action, bool untilDisable = true) {
            if (untilDisable) GetComponent<EvenDisableListener>(mono, key, action);
            else GetComponent<EventDestroyListener>(mono, key, action);
        }

        public static void RegisterListener(this MonoBehaviour mono, EventKey key, Action<object> action, bool untilDisable = true) {
            if (untilDisable) GetComponent<EvenDisableListener>(mono, key, action);
            else GetComponent<EventDestroyListener>(mono, key, action);
        }

        public static void RegisterListener(this MonoBehaviour mono, EventKey key, Action action, bool untilDisable = true) {
            if (untilDisable) GetComponent<EvenDisableListener>(mono, key, action);
            else GetComponent<EventDestroyListener>(mono, key, action);
        }

        public static void RegisterListener<T>(this MonoBehaviour mono, Action<T> action, bool untilDisable = true) where T : EventParams {
            if (untilDisable) GetComponent<T, EvenDisableListener>(mono, action);
            else GetComponent<T, EventDestroyListener>(mono, action);
		}

		public static void RegisterListener<T>(this MonoBehaviour mono, Action action, bool untilDisable = true) where T : EventParams {
            if (untilDisable) GetComponent<T, EvenDisableListener>(mono, action);
            else GetComponent<T, EventDestroyListener>(mono, action);
        }

        #region Base
        private static void GetComponent<TS>(MonoBehaviour mono, string key, Action<object> action) where TS : EventListenerBase {
            GetComponent<TS>(mono).SetListener(key, action);
        }

        private static void GetComponent<TS>(MonoBehaviour mono, string key, Action action) where TS : EventListenerBase {
            GetComponent<TS>(mono).SetListener(key, action);
        }

        private static void GetComponent<TS>(MonoBehaviour mono, int key, Action<object> action) where TS : EventListenerBase {
            GetComponent<TS>(mono).SetListener(key, action);
        }

        private static void GetComponent<TS>(MonoBehaviour mono, int key, Action action) where TS : EventListenerBase {
            GetComponent<TS>(mono).SetListener(key, action);
        }

        private static void GetComponent<TS>(MonoBehaviour mono, EventKey key, Action<object> action) where TS : EventListenerBase {
            GetComponent<TS>(mono).SetListener(key, action);
        }

        private static void GetComponent<TS>(MonoBehaviour mono, EventKey key, Action action) where TS : EventListenerBase {
            GetComponent<TS>(mono).SetListener(key, action);
        }

        private static void GetComponent<T, TS>(MonoBehaviour mono, Action<T> action) where TS : EventListenerBase where T : EventParams {
            GetComponent<TS>(mono).SetListener(action);
		}

		private static void GetComponent<T, TS>(MonoBehaviour mono, Action action) where TS : EventListenerBase where T : EventParams {
            GetComponent<TS>(mono).SetListener<T>(action);
		}

        private static T GetComponent<T>(MonoBehaviour mono) where T : EventListenerBase {
            var component = mono.GetComponent<T>();
            if (null == component) component = mono.gameObject.AddComponent<T>();
            return component;
        }
        #endregion
    }
}