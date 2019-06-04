using System;
using Gemmob.Common;

public partial class EventDispatcher : Singleton<EventDispatcher> {

    #region Params Generic Key
    /**<summary>ParamsEventObserver listener</summary> */
    public void Dispatch<T>() where T : EventParams {
        ParamsEventObserver.Instance.Dispatch<T>();
    }

    /**<summary>ParamsEventObserver listener</summary> */
    public void Dispatch<T>(T eventParams) where T : EventParams {
        ParamsEventObserver.Instance.Dispatch<T>(eventParams);
    }

    /**<summary>ParamsEventObserver listener</summary> */
    public void AddListener<T>(Action<T> eventCallback, bool showDebugLog = false) where T : EventParams {
        ParamsEventObserver.Instance.AddListener<T>(eventCallback);
    }

    /**<summary>ParamsEventObserver listener</summary> */
    public void AddListener<T>(Action eventCallback, bool showDebugLog = false) where T : EventParams {
        ParamsEventObserver.Instance.AddListener<T>(eventCallback);
    }

    /**<summary>ParamsEventObserver listener</summary> */
    public void RemoveListener<T>(Action<T> eventCallback, bool showDebugLog = false) where T : EventParams {
        ParamsEventObserver.Instance.RemoveListener(eventCallback);
    }

    /**<summary>ParamsEventObserver listener</summary> */
    public void RemoveListener<T>(Action eventCallback, bool showDebugLog = false) where T : EventParams {
        ParamsEventObserver.Instance.RemoveListener<T>(eventCallback);
    }

    /** <summary> Remove ALL callback listener by `key`. Be carefully, use at OnDestroy only to free RAM. </summary> */
    public void RemoveListener<T>() {
        ParamsEventObserver.Instance.RemoveListener<T>();
    }
    #endregion

    #region String Key
    /**<summary>StringEventObserver listener</summary> */
    public void Dispatch(string key, object value = null, bool showDebugLog = false) {
        StringEventObserver.Instance.Dispatch(key, value, showDebugLog);
    }

    /**<summary>StringEventObserver listener</summary> */
    public void AddListener(string key, Action<object> eventCallback, bool showDebugLog = false) {
        StringEventObserver.Instance.AddListener(key, eventCallback, showDebugLog);
    }

    /**<summary>StringEventObserver listener</summary> */
    public void AddListener(string key, Action eventCallback, bool showDebugLog = false) {
        StringEventObserver.Instance.AddListener(key, eventCallback, showDebugLog);
    }

    /**<summary>StringEventObserver listener</summary> */
    public void RemoveListener(string key, Action<object> eventCallback, bool showDebugLog = false) {
        StringEventObserver.Instance.RemoveListener(key, eventCallback, showDebugLog);
    }

    /**<summary>StringEventObserver listener</summary> */
    public void RemoveListener(string key, Action eventCallback, bool showDebugLog = false) {
        StringEventObserver.Instance.RemoveListener(key, eventCallback, showDebugLog);
    }

    /** <summary> Remove ALL callback listener by `key`. Be carefully, use at OnDestroy only to free RAM. </summary> */
    public void RemoveListener(string key) {
        StringEventObserver.Instance.RemoveListener(key);
    }
    #endregion


    #region int Key
    /**<summary>IntEventObserver listener</summary> */
    public void Dispatch(int key, object value = null, bool showDebugLog = false) {
        IntEventObserver.Instance.Dispatch(key, value, showDebugLog);
    }

    /**<summary>IntEventObserver listener</summary> */
    public void AddListener(int key, Action<object> eventCallback, bool showDebugLog = false) {
        IntEventObserver.Instance.AddListener(key, eventCallback, showDebugLog);
    }

    /**<summary>IntEventObserver listener</summary> */
    public void AddListener(int key, Action eventCallback, bool showDebugLog = false) {
        IntEventObserver.Instance.AddListener(key, eventCallback, showDebugLog);
    }

    /**<summary>IntEventObserver listener</summary> */
    public void RemoveListener(int key, Action<object> eventCallback, bool showDebugLog = false) {
        IntEventObserver.Instance.RemoveListener(key, eventCallback, showDebugLog);
    }

    /**<summary>IntEventObserver listener</summary> */
    public void RemoveListener(int key, Action eventCallback, bool showDebugLog = false) {
        IntEventObserver.Instance.RemoveListener(key, eventCallback, showDebugLog);
    }

    /** <summary> Remove ALL callback listener by `key`. Be carefully, use at OnDestroy only to free RAM. </summary> */
    public void RemoveListener(int key) {
        IntEventObserver.Instance.RemoveListener(key);
    }
    #endregion

    #region EventKey enum
    /**<summary>EventKey enum - IntEventObserver listener</summary> */
    public void Dispatch(EventKey key, object value = null) {
        IntEventObserver.Instance.Dispatch((int)key, value, false);
    }

    /**<summary>EventKey enum - IntEventObserver listener</summary> */
    public void AddListener(EventKey key, Action<object> eventCallback, bool showDebugLog = false) {
        IntEventObserver.Instance.AddListener((int)key, eventCallback, showDebugLog);
    }

    /**<summary>EventKey enum - IntEventObserver listener</summary> */
    public void AddListener(EventKey key, Action eventCallback, bool showDebugLog = false) {
        IntEventObserver.Instance.AddListener((int)key, eventCallback, showDebugLog);
    }

    /**<summary>EventKey enum - IntEventObserver listener</summary> */
    public void RemoveListener(EventKey key, Action<object> eventCallback, bool showDebugLog = false) {
        IntEventObserver.Instance.RemoveListener((int)key, eventCallback, showDebugLog);
    }

    /**<summary>EventKey enum - IntEventObserver listener</summary> */
    public void RemoveListener(EventKey key, Action eventCallback, bool showDebugLog = false) {
        IntEventObserver.Instance.RemoveListener((int)key, eventCallback, showDebugLog);
    }

    /** <summary> Remove ALL callback listener by `key`. Be carefully, use at OnDestroy only to free RAM. </summary> */
    public void RemoveListener(EventKey key) {
        IntEventObserver.Instance.RemoveListener((int)key);
    }
    #endregion
}