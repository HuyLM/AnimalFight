using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gemmob.Common {
    /** <summary> Send multi params to dispatcher </summary>*/
    public class EventParams {
    }
    
    internal sealed class ParamsEventObserver : EventObserver<ParamsEventObserver, Type, EventParams> {
        public void AddListener<T>(Action<T> eventCallback, bool showDebugLog = false) where T : EventParams {
            Action<EventParams> action = (param) => { eventCallback.Invoke((T)param); };
            base.AddListener(typeof(T), action, showDebugLog);
        }

        public void RemoveListener<T>(Action<T> eventCallback, bool showDebugLog = false) where T : EventParams {
            Action<EventParams> action = (param) => { eventCallback.Invoke((T)param); };
            base.RemoveListener(typeof(T), action, showDebugLog);
        }

        public void AddListener<T>(Action eventCallback, bool showDebugLog = false) where T : EventParams {
            Action<EventParams> action = (param) => { eventCallback.Invoke(); };
            base.AddListener(typeof(T), action, showDebugLog);
        }

        public void RemoveListener<T>(Action eventCallback, bool showDebugLog = false) where T : EventParams {
            Action<EventParams> action = (param) => { eventCallback.Invoke(); };
            base.RemoveListener(typeof(T), action, showDebugLog);
        }

        public void RemoveListener<T>() {
            base.RemoveListener(typeof(T));
        }

        public void Dispatch<T>(EventParams param = null, bool showDebugLog = false) {
            base.Dispatch(typeof(T), param, showDebugLog);
        }
    }

    internal sealed class IntEventObserver : EventObserver<IntEventObserver, int, object> {
    }

    internal sealed class StringEventObserver : EventObserver<StringEventObserver, string, object> {
    }

    internal class EventObserver<T, K, V> where T : new(){
        private static T instance;
        public static T Instance {
            get {
                if (instance != null) return instance;
                instance = new T();
                return instance;
            }
        }

        private readonly Dictionary<K, Dictionary<int, Action<V>>> observerDictionary = new Dictionary<K, Dictionary<int, Action<V>>>();
        
        public void AddListener(K key, Action<V> eventCallback, bool showDebugLog = false) {
            var hashCode = eventCallback.GetHashCode();
            Action<V> action = eventCallback.Invoke;
            AddListener(key, hashCode, action, showDebugLog);
        }

        public void AddListener(K key, Action eventCallback, bool showDebugLog = false) {
            var hashCode = eventCallback.GetHashCode();
            Action<V> action = _object => { eventCallback.Invoke(); };
            AddListener(key, hashCode, action, showDebugLog);
        }

        public void RemoveListener(K key, Action<V> eventCallback, bool showDebugLog = false) {
            RemoveByHashCode(key, eventCallback.GetHashCode(), showDebugLog);
        }

        public void RemoveListener(K key, Action eventCallback, bool showDebugLog = false) {
            RemoveByHashCode(key, eventCallback.GetHashCode(), showDebugLog);
        }

        /** <summary> Remove ALL callback listener by `key`. Be carefully, use at OnDestroy only to free RAM. </summary> */
        public void RemoveListener(K key) {
            RemoveByKey(key);
        }

        private void AddListener(K key, int hashCode, Action<V> action, bool showDebugLog = false) {
            if (showDebugLog) {
                Logs.LogFormat("[EventDispatcher({1})] Register Key: {0}", key, typeof(K));
            }

            Dictionary<int, Action<V>> value;
            if (!observerDictionary.TryGetValue(key, out value)) {
                value = new Dictionary<int, Action<V>>();
                observerDictionary[key] = value;
            }

            value[hashCode] = action;
        }


        private void RemoveByHashCode(K key, int hashCode, bool showDebugLog = false) {
            if (showDebugLog) {
                Logs.LogFormat("[EventDispatcher({1})] UnRegister Key: {0}", key, typeof(K));
            }

            Dictionary<int, Action<V>> value;
            if (observerDictionary.TryGetValue(key, out value)) {
                value.Remove(hashCode);
            }
        }

        private void RemoveByKey(K key, bool showDebugLog = false) {
            if (showDebugLog) {
                Logs.LogFormat("[EventDispatcher({1})] UnRegister All of Key: {0}", key, typeof(K));
            }
            observerDictionary.Remove(key);
        }

        public void Dispatch(K key, V obj = default(V), bool showDebugLog = false) {
            Dictionary<int, Action<V>> value;
            if (observerDictionary.TryGetValue(key, out value)) {
                var valueCollection = value.Values;
#if !PRODUCTION_BUILD
                if (showDebugLog) {
                    Logs.LogFormat("[EventDispatcher({3})] Dispatch total_events={2}, key={0}, action=[{1}])", key, obj, valueCollection.Count, typeof(K));
                }
#endif
                foreach (var caller in valueCollection) {
                    caller(obj);
                }
            }
            else {
                if (showDebugLog) {
                    Debug.LogFormat("[EventDispatcher{2}] No dispatch to send: (key:{0}, action:{1})", key, obj, typeof(K));
                }
            }
        }
    }
}