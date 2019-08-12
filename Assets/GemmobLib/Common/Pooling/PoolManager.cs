using UnityEngine;
using System.Collections.Generic;

public sealed class PoolManager : SingletonFreeAlive<PoolManager> {
    public enum StartupPoolMode { Awake, Start, CallManually };
    public StartupPoolMode startupPoolMode;
    public List<StartupPool> startupPools;

    [System.Serializable]
    public class StartupPool {
        public int size;
        public GameObject prefab;
    }

    static List<GameObject> tempList = new List<GameObject>();

    Dictionary<GameObject, List<GameObject>> pooledObjects = new Dictionary<GameObject, List<GameObject>>();
    Dictionary<GameObject, GameObject> spawnedObjects = new Dictionary<GameObject, GameObject>();

    bool startupPoolsCreated;

    protected override void Awake() {
        if (instance != null) {
            if (startupPools != null && startupPools.Count > 0) {
                if (startupPoolMode == StartupPoolMode.Awake) {
                    CreateStartupPools(this);
                }
                else {
                    Instance.startupPools.AddRange(startupPools);
                }
            }
            DestroyImmediate(gameObject);
            return;
        }

        base.Awake();
        if (startupPoolMode == StartupPoolMode.Awake)
            CreateStartupPools(Instance);
    }

    void Start() {
        if (startupPoolMode == StartupPoolMode.Start)
            CreateStartupPools(Instance);
    }

    public static void CreateStartupPools(PoolManager pooling) {
        if (!pooling.startupPoolsCreated) {
            pooling.startupPoolsCreated = true;
            var pools = Instance.startupPools;
            if (pools != null && pools.Count > 0)
                for (int i = 0; i < pools.Count; ++i)
                    CreatePool(pools[i].prefab, pools[i].size);
        }
    }

    public static void CreatePool<T>(T prefab, int initialPoolSize) where T : Component {
        CreatePool(prefab.gameObject, initialPoolSize);
    }
    public static void CreatePool(GameObject prefab, int initialPoolSize) {
        if (prefab != null && !Instance.pooledObjects.ContainsKey(prefab)) {
            var list = new List<GameObject>();
            Instance.pooledObjects.Add(prefab, list);

            if (initialPoolSize > 0) {
                bool active = prefab.activeSelf;
                prefab.SetActive(false);
                Transform parent = Instance.transform;
                while (list.Count < initialPoolSize) {
                    var obj = Instantiate(prefab);
                    obj.transform.SetParent(parent);
                    list.Add(obj);
                }
                prefab.SetActive(active);
            }
        }
    }

    public static T Spawn<T>(T prefab, Transform parent, Vector3 position, Quaternion rotation) where T : Component {
        return Spawn(prefab.gameObject, parent, position, rotation).GetComponent<T>();
    }
    public static T Spawn<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component {
        return Spawn(prefab.gameObject, null, position, rotation).GetComponent<T>();
    }
    public static T Spawn<T>(T prefab, Transform parent, Vector3 position) where T : Component {
        return Spawn(prefab.gameObject, parent, position, Quaternion.identity).GetComponent<T>();
    }
    public static T Spawn<T>(T prefab, Vector3 position) where T : Component {
        return Spawn(prefab.gameObject, null, position, Quaternion.identity).GetComponent<T>();
    }
    public static T Spawn<T>(T prefab, Transform parent) where T : Component {
        return Spawn(prefab.gameObject, parent, Vector3.zero, Quaternion.identity).GetComponent<T>();
    }
    public static T Spawn<T>(T prefab) where T : Component {
        return Spawn(prefab.gameObject, null, Vector3.zero, Quaternion.identity).GetComponent<T>();
    }
    public static GameObject Spawn(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation) {
        List<GameObject> list;
        Transform trans;
        GameObject obj;
        if (Instance.pooledObjects.TryGetValue(prefab, out list)) {
            obj = null;
            if (list.Count > 0) {
                while (obj == null && list.Count > 0) {
                    obj = list[0];
                    list.RemoveAt(0);
                }
                if (obj != null) {
                    trans = obj.transform;
                    trans.SetParent(parent);
                    trans.localPosition = position;
                    trans.localRotation = rotation;
                    obj.SetActive(true);
                    Instance.spawnedObjects.Add(obj, prefab);
                    return obj;
                }
            }
            obj = Instantiate(prefab);
            trans = obj.transform;
            trans.SetParent(parent);
            trans.localPosition = position;
            trans.localRotation = rotation;
            Instance.spawnedObjects.Add(obj, prefab);
            return obj;
        }
        else {
            obj = Instantiate(prefab);
            trans = obj.GetComponent<Transform>();
            trans.SetParent(parent);
            trans.localPosition = position;
            trans.localRotation = rotation;
            return obj;
        }
    }
    public static GameObject Spawn(GameObject prefab, Transform parent, Vector3 position) {
        return Spawn(prefab, parent, position, Quaternion.identity);
    }
    public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation) {
        return Spawn(prefab, null, position, rotation);
    }
    public static GameObject Spawn(GameObject prefab, Transform parent) {
        return Spawn(prefab, parent, Vector3.zero, Quaternion.identity);
    }
    public static GameObject Spawn(GameObject prefab, Vector3 position) {
        return Spawn(prefab, null, position, Quaternion.identity);
    }
    public static GameObject Spawn(GameObject prefab) {
        return Spawn(prefab, null, Vector3.zero, Quaternion.identity);
    }

    public static void Recycle<T>(T obj) where T : Component {
        Recycle(obj.gameObject);
    }
    public static void Recycle(GameObject obj) {
        GameObject prefab;
        if (Instance.spawnedObjects.TryGetValue(obj, out prefab))
            Recycle(obj, prefab);
        else
            Destroy(obj);
    }
    static void Recycle(GameObject obj, GameObject prefab) {
        Instance.pooledObjects[prefab].Add(obj);
        Instance.spawnedObjects.Remove(obj);
        obj.transform.SetParent(Instance.transform);
        obj.SetActive(false);
    }

    public static void RecycleAll<T>(T prefab) where T : Component {
        RecycleAll(prefab.gameObject);
    }
    public static void RecycleAll(GameObject prefab) {
        foreach (var item in Instance.spawnedObjects)
            if (item.Value == prefab)
                tempList.Add(item.Key);
        for (int i = 0; i < tempList.Count; ++i)
            Recycle(tempList[i]);
        tempList.Clear();
    }
    public static void RecycleAll() {
        tempList.AddRange(Instance.spawnedObjects.Keys);
        for (int i = 0; i < tempList.Count; ++i)
            Recycle(tempList[i]);
        tempList.Clear();
    }

    public static bool IsSpawned(GameObject obj) {
        return Instance.spawnedObjects.ContainsKey(obj);
    }

    public static int CountPooled<T>(T prefab) where T : Component {
        return CountPooled(prefab.gameObject);
    }
    public static int CountPooled(GameObject prefab) {
        List<GameObject> list;
        if (Instance.pooledObjects.TryGetValue(prefab, out list))
            return list.Count;
        return 0;
    }

    public static int CountSpawned<T>(T prefab) where T : Component {
        return CountSpawned(prefab.gameObject);
    }
    public static int CountSpawned(GameObject prefab) {
        int count = 0;
        foreach (var instancePrefab in Instance.spawnedObjects.Values)
            if (prefab == instancePrefab)
                ++count;
        return count;
    }

    public static int CountAllPooled() {
        int count = 0;
        foreach (var list in Instance.pooledObjects.Values)
            count += list.Count;
        return count;
    }

    public static List<GameObject> GetPooled(GameObject prefab, List<GameObject> list, bool appendList) {
        if (list == null)
            list = new List<GameObject>();
        if (!appendList)
            list.Clear();
        List<GameObject> pooled;
        if (Instance.pooledObjects.TryGetValue(prefab, out pooled))
            list.AddRange(pooled);
        return list;
    }
    public static List<T> GetPooled<T>(T prefab, List<T> list, bool appendList) where T : Component {
        if (list == null)
            list = new List<T>();
        if (!appendList)
            list.Clear();
        List<GameObject> pooled;
        if (Instance.pooledObjects.TryGetValue(prefab.gameObject, out pooled))
            for (int i = 0; i < pooled.Count; ++i)
                list.Add(pooled[i].GetComponent<T>());
        return list;
    }

    public static List<GameObject> GetSpawned(GameObject prefab, List<GameObject> list, bool appendList) {
        if (list == null)
            list = new List<GameObject>();
        if (!appendList)
            list.Clear();
        foreach (var item in Instance.spawnedObjects)
            if (item.Value == prefab)
                list.Add(item.Key);
        return list;
    }
    public static List<T> GetSpawned<T>(T prefab, List<T> list, bool appendList) where T : Component {
        if (list == null)
            list = new List<T>();
        if (!appendList)
            list.Clear();
        var prefabObj = prefab.gameObject;
        foreach (var item in Instance.spawnedObjects)
            if (item.Value == prefabObj)
                list.Add(item.Key.GetComponent<T>());
        return list;
    }

    public static void DestroyPooled(GameObject prefab) {
        List<GameObject> pooled;
        if (Instance.pooledObjects.TryGetValue(prefab, out pooled)) {
            for (int i = 0; i < pooled.Count; ++i)
                Destroy(pooled[i]);
            pooled.Clear();
        }
    }
    public static void DestroyPooled<T>(T prefab) where T : Component {
        DestroyPooled(prefab.gameObject);
    }

    public static void DestroyAll(GameObject prefab) {
        RecycleAll(prefab);
        DestroyPooled(prefab);
    }
    public static void DestroyAll<T>(T prefab) where T : Component {
        DestroyAll(prefab.gameObject);
    }
}

public static class ObjectPoolExtensions {
    public static void CreatePool<T>(this T prefab) where T : Component {
        PoolManager.CreatePool(prefab, 0);
    }
    public static void CreatePool<T>(this T prefab, int initialPoolSize) where T : Component {
        PoolManager.CreatePool(prefab, initialPoolSize);
    }
    public static void CreatePool(this GameObject prefab) {
        PoolManager.CreatePool(prefab, 0);
    }
    public static void CreatePool(this GameObject prefab, int initialPoolSize) {
        PoolManager.CreatePool(prefab, initialPoolSize);
    }

    public static T Spawn<T>(this T prefab, Transform parent, Vector3 position, Quaternion rotation) where T : Component {
        return PoolManager.Spawn(prefab, parent, position, rotation);
    }
    public static T Spawn<T>(this T prefab, Vector3 position, Quaternion rotation) where T : Component {
        return PoolManager.Spawn(prefab, null, position, rotation);
    }
    public static T Spawn<T>(this T prefab, Transform parent, Vector3 position) where T : Component {
        return PoolManager.Spawn(prefab, parent, position, Quaternion.identity);
    }
    public static T Spawn<T>(this T prefab, Vector3 position) where T : Component {
        return PoolManager.Spawn(prefab, null, position, Quaternion.identity);
    }
    public static T Spawn<T>(this T prefab, Transform parent) where T : Component {
        return PoolManager.Spawn(prefab, parent, Vector3.zero, Quaternion.identity);
    }
    public static T Spawn<T>(this T prefab) where T : Component {
        return PoolManager.Spawn(prefab, null, Vector3.zero, Quaternion.identity);
    }
    public static GameObject Spawn(this GameObject prefab, Transform parent, Vector3 position, Quaternion rotation) {
        return PoolManager.Spawn(prefab, parent, position, rotation);
    }
    public static GameObject Spawn(this GameObject prefab, Vector3 position, Quaternion rotation) {
        return PoolManager.Spawn(prefab, null, position, rotation);
    }
    public static GameObject Spawn(this GameObject prefab, Transform parent, Vector3 position) {
        return PoolManager.Spawn(prefab, parent, position, Quaternion.identity);
    }
    public static GameObject Spawn(this GameObject prefab, Vector3 position) {
        return PoolManager.Spawn(prefab, null, position, Quaternion.identity);
    }
    public static GameObject Spawn(this GameObject prefab, Transform parent) {
        return PoolManager.Spawn(prefab, parent, Vector3.zero, Quaternion.identity);
    }
    public static GameObject Spawn(this GameObject prefab) {
        return PoolManager.Spawn(prefab, null, Vector3.zero, Quaternion.identity);
    }

    public static void Recycle<T>(this T obj) where T : Component {
        PoolManager.Recycle(obj);
    }
    public static void Recycle(this GameObject obj) {
        PoolManager.Recycle(obj);
    }

    public static void RecycleAll<T>(this T prefab) where T : Component {
        PoolManager.RecycleAll(prefab);
    }
    public static void RecycleAll(this GameObject prefab) {
        PoolManager.RecycleAll(prefab);
    }

    public static int CountPooled<T>(this T prefab) where T : Component {
        return PoolManager.CountPooled(prefab);
    }
    public static int CountPooled(this GameObject prefab) {
        return PoolManager.CountPooled(prefab);
    }

    public static int CountSpawned<T>(this T prefab) where T : Component {
        return PoolManager.CountSpawned(prefab);
    }
    public static int CountSpawned(this GameObject prefab) {
        return PoolManager.CountSpawned(prefab);
    }

    public static List<GameObject> GetSpawned(this GameObject prefab, List<GameObject> list, bool appendList) {
        return PoolManager.GetSpawned(prefab, list, appendList);
    }
    public static List<GameObject> GetSpawned(this GameObject prefab, List<GameObject> list) {
        return PoolManager.GetSpawned(prefab, list, false);
    }
    public static List<GameObject> GetSpawned(this GameObject prefab) {
        return PoolManager.GetSpawned(prefab, null, false);
    }
    public static List<T> GetSpawned<T>(this T prefab, List<T> list, bool appendList) where T : Component {
        return PoolManager.GetSpawned(prefab, list, appendList);
    }
    public static List<T> GetSpawned<T>(this T prefab, List<T> list) where T : Component {
        return PoolManager.GetSpawned(prefab, list, false);
    }
    public static List<T> GetSpawned<T>(this T prefab) where T : Component {
        return PoolManager.GetSpawned(prefab, null, false);
    }

    public static List<GameObject> GetPooled(this GameObject prefab, List<GameObject> list, bool appendList) {
        return PoolManager.GetPooled(prefab, list, appendList);
    }
    public static List<GameObject> GetPooled(this GameObject prefab, List<GameObject> list) {
        return PoolManager.GetPooled(prefab, list, false);
    }
    public static List<GameObject> GetPooled(this GameObject prefab) {
        return PoolManager.GetPooled(prefab, null, false);
    }
    public static List<T> GetPooled<T>(this T prefab, List<T> list, bool appendList) where T : Component {
        return PoolManager.GetPooled(prefab, list, appendList);
    }
    public static List<T> GetPooled<T>(this T prefab, List<T> list) where T : Component {
        return PoolManager.GetPooled(prefab, list, false);
    }
    public static List<T> GetPooled<T>(this T prefab) where T : Component {
        return PoolManager.GetPooled(prefab, null, false);
    }

    public static void DestroyPooled(this GameObject prefab) {
        PoolManager.DestroyPooled(prefab);
    }
    public static void DestroyPooled<T>(this T prefab) where T : Component {
        PoolManager.DestroyPooled(prefab.gameObject);
    }

    public static void DestroyAll(this GameObject prefab) {
        PoolManager.DestroyAll(prefab);
    }
    public static void DestroyAll<T>(this T prefab) where T : Component {
        PoolManager.DestroyAll(prefab.gameObject);
    }
}
