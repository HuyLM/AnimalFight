using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : SubManager {

    public GameLoader gameLoader;
    private int countWin;
    public static Game instance;
    public static Game Instance {
        get {
            if(instance == null) {
                instance = FindObjectOfType<Game>();
                instance.Init();
                return instance;
            }
            return instance;
        }
    }

    public GameObject content;

    public Level CurrentLevel {
        get {
            return gameLoader.CurrentLevel;
        }
    }

    protected override void Awake() {
        if(instance == null) {
            instance = this;
        }

        base.Awake();
    }

    private void Start() {
        GameManager.Instance.StartGame();
    }

    public override void Init() {
        gameLoader.Initialize(this);
    }

    public override void StartGame() {
        //Load(GameResource.Instance.CurrentLevel);
    }

    public override void Next() {
        Level levelNext = GameResource.Instance.LevelNext(CurrentLevel);
        Load(levelNext);
    }

    public override void RePlay() {
        Load(gameLoader.CurrentLevel);
    }

    public override void GameWin() {
        CurrentLevel.bonus.Collect();
        Level levelNext = GameResource.Instance.LevelNext(CurrentLevel);
         PrefData.CurrentLevel = levelNext.Index;
    }
    public override void GameOver() {
        CurrentLevel.lostBonus.Collect();
    }
    public void Load(Level level) {
        Reset();
        gameLoader.Load(level);
        gameLoader.UpdateSort();
        //EventDispatcher.Instance.Dispatch(EventName.LOAD_LEVEL, level);
    }

    public void CheckGameState() {
        if(!GameManager.IsState(GameState.Playing)) {
            return;
        }

        if(gameLoader.EnemysDie.Count >= CurrentLevel.EnemyCount) {
            GameManager.Instance.GameWin();
        }

        if(gameLoader.AllHero.Count <= 0) {

            GameManager.Instance.GameOver();
        }
    }

    private void Reset() {
        gameLoader.Reset();
    }

    private void Update() {
        gameLoader.UpdateSort();

        if(Input.GetKeyDown(KeyCode.Space)) {
            Level levelNext = GameResource.Instance.LevelNext(CurrentLevel);
            PrefData.CurrentLevel = levelNext.Index;
            Load(levelNext);
        }
    }

}
