using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class Scenes : SingletonFreeAlive<Scenes> {

    string currentSceneName = "Logo";
    private double loadStartTime;
    private double loadFinishTime;

    public void Reload() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Load(string name) {
        currentSceneName = name;
        SceneManager.LoadScene(name);
    }

    public void Load(SceneDefined.Index index) {
        currentSceneName = index.ToString();
        Load((int)index);
    }

    public void Load(int index) {
        SceneManager.LoadScene(index);
    }

    public AsyncOperation LoadAsync(string name, Action onLoadding = null, Action onFinish = null, float delayTime = 0) {
        currentSceneName = name;
        AsyncOperation async = LoadAsync(name);
        StartCoroutine(IEWaitForDone(async, onLoadding, onFinish, delayTime));
        return async;
    }

    public AsyncOperation LoadAsync(SceneDefined.Index index, Action onLoadding, Action onFinish, float delayTime = 0) {
        currentSceneName = index.ToString();
        return LoadAsync((int)index, onLoadding, onFinish, delayTime);
    }

    public AsyncOperation LoadAsync(int index, Action onLoadding, Action onFinish, float delayTime = 0) {
        AsyncOperation async = LoadAsync(index);
        StartCoroutine(IEWaitForDone(async, onLoadding, onFinish, delayTime));
        return async;
    }

    public AsyncOperation LoadAsync(string name) {
        return SceneManager.LoadSceneAsync(name);
    }

    public AsyncOperation LoadAsync(SceneDefined.Index index) {
        return SceneManager.LoadSceneAsync((int)index);
    }

    public AsyncOperation LoadAsync(int index) {
        return SceneManager.LoadSceneAsync(index);
    }

    #region IEnumerator
    private IEnumerator IEWaitForDone(AsyncOperation async, Action onLoadding = null, Action onFinish = null, float delayTime = 0) {
        if (delayTime > 0) async.allowSceneActivation = false;
        while (delayTime > 0) {
            delayTime -= Time.deltaTime;
            if (onLoadding != null) onLoadding.Invoke();
            yield return null;
        }
        async.allowSceneActivation = true;
		while(!async.isDone) {
			yield return null;
		}
        if (onFinish != null) onFinish.Invoke();
    }
    #endregion
}