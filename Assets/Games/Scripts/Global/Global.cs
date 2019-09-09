using UnityEngine;

public class Global : SingletonFreeAlive<Global> {
    public AppState AppState { get; private set; }

    public GameData GameData { get; private set; }

    protected override void OnAwake() {
        Application.targetFrameRate = 60;
        GameData = new GameData();
        GameResource.Instance.Initialize(); // ato
    }

    #region Load Scene
    const float loadSceneDelayTime = 0.3f;
	public void GoHomeScene(System.Action callback = null, System.Action exitCallback = null) {
		if (AppState == AppState.OnHome) return;
        AppState = AppState.OnHome;
        Transition.Instance.StartTransition(() => {
                Scenes.Instance.LoadAsync(SceneDefined.Index.Home, null, callback, loadSceneDelayTime);
            }, exitCallback
            );
    }

	public void GoGameScene(bool replay = false) {
        if (AppState == AppState.OnIngame) {
            if (replay) Scenes.Instance.Reload();
            return;
        }
        AppState = AppState.OnIngame;
        Transition.Instance.StartTransition(() => {
            Scenes.Instance.LoadAsync(SceneDefined.Index.GamePlay, null, null, loadSceneDelayTime);
        });
    }

    #endregion

    #region Application state
    public void SaveGameData() {
        PrefData.Instance.SaveGameData(GameData);
    }

    public void Pause() {
		Time.timeScale = 0f;
        EventDispatcher.Instance.Dispatch(EventKey.OnApplicationPause);
	}

	public void Resume() {
		Time.timeScale = 1f;
        EventDispatcher.Instance.Dispatch(EventKey.OnApplicationResume);
	}

    public void Quit() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    protected override void OnDestroy() {
        SaveGameData();
        GameData = null;
        base.OnDestroy();
    }

    public void OnApplicationPause(bool pause) {
        SaveGameData();
    }

    public void OnApplicationFocus(bool focus) {
        SaveGameData();
    }

    public void OnApplicationQuit() {
    }
    #endregion

}
