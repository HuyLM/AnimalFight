using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "GameResource", fileName = "GameResource")]
public class GameResource : ScriptableObject
{
    private static GameResource instance;
    public static GameResource Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<GameResource>("GameResource");
            }
            return instance;
        }
    }

    public List<CharacterData> characterDatas = new List<CharacterData>();
    public List<Level> levels = new List<Level>();



    public CharacterData RandomAllie
    {
        get {
            return AllieInfoDatas[Random.Range(0, AllieInfoDatas.Count)];
        }
    }

    public List<HeroData> HeroDatas
    {
        get
        {
            return PrefData.Heros;
        }
    }

    public List<CharacterData> HeroInfoDatas
    {
        get
        {
            return characterDatas.FindAll(a => a is HeroInfoData);
        }
    }

    public List<AllieData> AllieDatas
    {
        get
        {
            return PrefData.Allies;
        }
    }

    public List<CharacterData> AllieInfoDatas
    {
        get
        {
            return characterDatas.FindAll(a => a is CharacterInfoData);
        }
    }

    public HeroData MainHero
    {
        get
        {
            if (HeroDatas.Count != 0)
            {
                return HeroDatas[0];
            }
            return null;
        }
    }

    public CharacterData GetCharacter(Species species)
    {
        return characterDatas.Find(a => a.species.Equals(species));
    }

    public CharacterData GetAllie(Species species)
    {
        return characterDatas.Find(a => a.species.Equals(species) && a is CharacterInfoData);
    }

    public CharacterData GetHero(Species species)
    {
        return characterDatas.Find(a => a.species.Equals(species) && a is HeroInfoData);
    }

    public void Initialize()
    {

        for (int i = 0; i < levels.Count; i++)
        {
            levels[i].Index = i;
        }

        for (int i = 0; i < characterDatas.Count; i++)
        {
            characterDatas[i].Initialize();
        }

        if (HeroDatas.Count == 0)
        {
            if (HeroInfoDatas.Count != 0)
            {
                CharacterData characterData = HeroInfoDatas[0];
                PrefData.CollectHero(characterData);
            }
        }
        MainHero.HeroInfoData.OnWakeUp();
    }

#if UNITY_EDITOR
    public static void Unload()
    {
        //foreach (var item in Instance.stages)
        //{
        //    item.Unload();
        //    Resources.UnloadAsset(item);
        //}
        //Resources.UnloadAsset(instance);
    }
#endif

   

    public Level CurrentLevel
    {
        get
        {
            int currentLevel = PrefData.CurrentLevel;
            currentLevel = Mathf.Clamp(currentLevel, 0, levels.Count);
            Level level = levels[currentLevel];
            return level;
        }
    }

    public Level LevelNext(Level level)
    {

        if (level.Index < levels.Count - 1)
        {
            return levels[level.Index + 1];
        }

        if (level.Index == levels.Count - 1)
        {
            return level;
        }

       
        Level levelEnd = levels[levels.Count - 1];
        return levelEnd;
    }
    // test

    //public Wave CurrentWave
    //{
    //    get
    //    {
    //        int currentStage = Screw.Settings.CurrentStage;
    //        currentStage = Mathf.Clamp(currentStage, 0, StageCount - 1);
    //        Stage stage = this[currentStage];

    //        int currentLevel = Screw.Settings.CurrentLevel;
    //        currentLevel = Mathf.Clamp(currentLevel, 0, stage.LevelCount - 1);
    //        Level level = stage[currentLevel];

    //        int currentWave = Screw.Settings.CurrentWave;
    //        currentWave = Mathf.Clamp(currentWave, 0, level.WaveCount - 1);
    //        return level[currentWave];
    //    }
    //}

    //public Wave waveNext(Wave wave) {
    //    Level level = wave.level;
    //    Stage stage = level.stage;

    //    if (wave.Index < level.WaveCount - 1)
    //    {
    //        return level[wave.Index + 1];
    //    }

    //    if (wave.Index == level.WaveCount - 1)
    //    {
    //        return wave;
    //    }
    //    return wave;
    //}
    // end test

    [ContextMenu("Generate Stage ID (if needed)")]
    public void GenerateID()
    {
        foreach (var i in levels)
        {
            if (i.id.Equals(""))
            {
                i.GenerateID();
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GameResource))]
public class GameResourceEditor : Editor {
    private GameResource myTarget;
    private void OnEnable()
    {
        myTarget = (GameResource)target;
    }

    private List<CharacterInfoData> GetAllCharacterData()
    {
        return Resources.LoadAll<CharacterInfoData>("CharacterData/CharacterInfoData/Edition").ToList();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Update Characters Data"))
        {
            var charsData = GetAllCharacterData().OrderBy(x => int.Parse(x.id.Remove(0,1))).ToList();
            myTarget.characterDatas.Clear();
            foreach (var item in charsData)
            {
                myTarget.characterDatas.Add((CharacterData)item);
            }
        }
    }
}
#endif
