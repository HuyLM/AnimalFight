using System.Reflection;
using UnityEngine;

/** <summary> MonoBehavior - Singleton class which is not nullable 'Instance' </summary> */
public class MonoSingleton<T> : MonoBehaviour where T : Component {
    private static T instance;

    public static T Instance {
        get {
            if (instance != null) return instance;
            Initialize();
            return instance;
        }
    }

    private static void Initialize() {
        if (instance != null) return;
        instance = (T)FindObjectOfType(typeof(T));
        if (instance == null) instance = new GameObject(typeof(T).ToString()).AddComponent<T>();
    }

    protected virtual void Awake() {
        if (instance == null) instance = this as T;
        else if (this != instance) {
            Debug.LogWarningFormat("[MonoSingleton] Class {0} is initialized multiple times", typeof(T).FullName);
            DestroyImmediate(gameObject);
            return;
        }
        
        OnAwake();
    }

    protected virtual void OnAwake() { }

    public virtual void PreLoad() { }

    protected virtual void OnDestroy() {
        instance = null;
    }
}

/** <summary> MonoBehavior - Singleton class with DonDestroyOnLoad </summary> */
public class MonoSingletonKeepAlive<T> : MonoSingleton<T> where T : MonoBehaviour {
    protected override void Awake() {
        base.Awake();
        if (gameObject) DontDestroyOnLoad(gameObject);
    }

    protected override void OnDestroy() {

    }
}

/** <summary> Normal singleton which is not MonoBehavior </summary> */
public class Singleton<T> where T: new(){
    private static T instance;
    
    public static T Instance {
        get {
            if (instance != null) return instance;
            instance = new T();
            return instance;
        }
    }

}