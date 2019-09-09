using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoader : MonoBehaviour {

    #region CharacterManager

    public Dictionary<Character.Type, List<Character>> enemies = new Dictionary<Character.Type, List<Character>>();
    public Dictionary<Character.Type, List<Character>> heros = new Dictionary<Character.Type, List<Character>>();

    public List<Dictionary<Character.Type, List<Character>>> allCharacter = new List<Dictionary<Character.Type, List<Character>>>();

    public Character MainHero;

    public List<Character> AllEnemy {
        get {
            List<Character> characters = new List<Character>();
            foreach(KeyValuePair<Character.Type, List<Character>> entry in enemies) {
                characters.AddRange(entry.Value);
            }

            return characters;
        }
    }

    public List<Character> AllHero {
        get {
            List<Character> characters = new List<Character>();
            foreach(KeyValuePair<Character.Type, List<Character>> entry in heros) {
                characters.AddRange(entry.Value);
            }

            return characters;
        }
    }

    public List<Character> EnemysDie = new List<Character>();
    public List<Character> HerosDie = new List<Character>();

    public Character FirstHero {
        get {
            List<Character> characters = AllHero;

            characters.Sort((Character a, Character b) => b.transform.position.x.CompareTo(a.transform.position.x));
            if(characters.Count > 0) {
                return characters[0];
            }
            return null;
        }
    }

    public int EnemiesCount {
        get {
            return AllEnemy.Count;
        }
    }

    public int HerosCount {
        get {
            return AllHero.Count;
        }
    }

    public void AddCharacter(Character character) {
        switch(character.group) {
            case Character.Group.Enemy:
                AddItem(enemies, character);
                break;

            case Character.Group.Hero:
                AddItem(heros, character);
                break;
        }
    }

    public void RemoveCharacter(Character character) {
        switch(character.group) {
            case Character.Group.Enemy:
                RemoveItem(enemies, character);
                EnemysDie.Add(character);
                Enemy enemy = (Enemy)character;
                GenWaveImmediately(CurrentLevel.waves[enemy.WaveIndex], enemy.WaveIndex);
                //EventDispatcher.Instance.Dispatch(EventName.ENEMY_DIE, character);
                break;

            case Character.Group.Hero:
                RemoveItem(heros, character);
                HerosDie.Add(character);
                break;
        }

        game.CheckGameState();
    }

    private void AddItem(Dictionary<Character.Type, List<Character>> dictionary, Character character) {
        List<Character> characters = new List<Character>();
        if(HasKey(dictionary, character.type)) {
            characters = dictionary[character.type];
        }
        characters.Add(character);
        dictionary[character.type] = characters;
    }

    private void RemoveItem(Dictionary<Character.Type, List<Character>> dictionary, Character character) {
        List<Character> characters = new List<Character>();
        if(HasKey(dictionary, character.type)) {
            characters = dictionary[character.type];
        }
        characters.Remove(character);
        dictionary[character.type] = characters;
    }

    private void SortItem(Dictionary<Character.Type, List<Character>> dictionary, bool flip = false) {
        foreach(KeyValuePair<Character.Type, List<Character>> entry in dictionary) {
            List<Character> characters = new List<Character>();
            if(HasKey(dictionary, entry.Key)) {
                characters = dictionary[entry.Key];

                if(!flip) {
                    characters.Sort((Character a, Character b) => a.transform.position.x.CompareTo(b.transform.position.x));
                }
                else {
                    characters.Sort((Character a, Character b) => b.transform.position.x.CompareTo(a.transform.position.x));
                }
            }
        }
    }

    private bool HasKey(Dictionary<Character.Type, List<Character>> dictionary, Character.Type key) {
        if(dictionary == null || key == null) {
            return false;
        }

        if(dictionary != null) {
            if(dictionary.ContainsKey(key)) {
                return true;
            }
        }

        return false;
    }

    public void UpdateSort() {
        SortItem(heros, true);
        SortItem(enemies);
    }

    public Character TargetOf(Character character) {
        List<Character> characters = new List<Character>();

        switch(character.group) {
            case Character.Group.Hero:
                foreach(var item in character.targetTypes) {
                    if(HasKey(enemies, item) && enemies[item].Count > 0) {
                        characters.Add(enemies[item][0]);
                    }
                }
                characters.Sort((Character a, Character b) => a.transform.position.x.CompareTo(b.transform.position.x));

                break;
            case Character.Group.Enemy:
                foreach(var item in character.targetTypes) {
                    if(HasKey(heros, item) && heros[item].Count > 0) {
                        characters.Add(heros[item][0]);
                    }
                }
                characters.Sort((Character a, Character b) => b.transform.position.x.CompareTo(a.transform.position.x));

                break;
            default:
                break;
        }

        if(characters.Count > 0) {
            return characters[0];
        }

        return null;
    }

    public List<Character> TargetsOf(Character character) {
        List<Character> characters = new List<Character>();

        switch(character.group) {
            case Character.Group.Hero:
                foreach(var item in character.targetTypes) {
                    if(HasKey(enemies, item)) {
                        characters.AddRange(enemies[item]);
                    }
                }

                break;
            case Character.Group.Enemy:
                foreach(var item in character.targetTypes) {
                    if(HasKey(heros, item)) {
                        characters.AddRange(heros[item]);
                    }
                }
                break;
            default:
                break;
        }

        return characters;
    }

    #endregion

    private Game game;
    private Wave currentWave;
    [HideInInspector] public Level CurrentLevel;

    public Area area;

    public float flyPoint;
    public float walkPoint;
    public float waterPoint;
    public float towerPoint;


    [SerializeField] private float offsetGenCharacter = 3f;
    [SerializeField] private float offsetGenEnemy = 10f;

    private float genEnemyPoint {
        get {
            if(FirstHero != null) {
                return FirstHero.transform.position.x + offsetGenEnemy;
            }
            return 0;

        }
    }

    private float genHeroPoint {
        get {
            return area.BottonLeft.x + offsetGenCharacter;
        }
    }

    public void Initialize(Game game) {
        this.game = game;
        allCharacter.Add(enemies);
        allCharacter.Add(heros);
    }

    public void Load(Level level) {
        this.CurrentLevel = level;
        StopAllCoroutines();

        for(int i = 0; i < level.waves.Count; i++) {
            StartCoroutine(GenWaveDelay(level.waves[i], i));
        }

        List<AllieData> allieDatas = GameResource.Instance.AllieDatas.FindAll(a => a.Selected);
        foreach(var item in allieDatas) {
            item.SpendMana();
            GenCharacter(item.CharacterData, Character.Group.Hero, item.CharacterData.GetUpgrade(item.rankType, item.CurrentLevel));
        }

        foreach(var item in GameResource.Instance.HeroDatas) {
            MainHero = GenCharacter(item.HeroInfoData, Character.Group.Hero, item.HeroInfoData.GetUpgrade(item.CurrentLevel));
        }
    }

    void GenWave(Wave wave, int waveIndex) {
        currentWave = wave;
        bool gotFlagHolder = false;
        Enemy flagHolder = null;
        foreach(var c in wave.characterTypes) {
            for(int i = 0; i < c.count; i++) {
                CharacterInfoData characterInfoData = (CharacterInfoData)c.characterData;
                Enemy enemy = (Enemy)GenCharacter(c.characterData, Character.Group.Enemy,
                    characterInfoData.GetUpgrade(c.rankType, c.level));
                enemy.SetWaveIndex(waveIndex);
                if(!gotFlagHolder) {
                    flagHolder = enemy;
                    enemy.SetFlagHolder(true, flagHolder.transform);
                }
                gotFlagHolder = true;
            }
        }
        //EventDispatcher.Instance.Dispatch(EventName.GEN_WAVE, flagHolder);
    }

    private IEnumerator GenWaveDelay(Wave wave, int waveIndex) {
        float timeDelay = wave.timeDelay;
        yield return new WaitForSeconds(timeDelay);
        GenWave(wave, waveIndex);
    }

    void GenWaveImmediately(Wave wave, int waveIndex) {
        if(GetEnemiesInWave(waveIndex).Count != 0)
            return;
        if(IsLastWave(waveIndex))
            return;
        StopCoroutine(GenWaveDelay(wave, waveIndex + 1));
        //GetWave(wave, waveIndex);
    }

    private Character GenCharacter(CharacterData characterData, Character.Group group, CharacterData.UpgradeData upgrade) {
        GameObject prefab = characterData.characterPrefab;
        GameObject character = Instantiate(prefab, game.content.transform);
        Character cc = character.GetComponent<Character>();
        cc.SetGroup(group);

        cc.Initialize(characterData, upgrade);

        character.transform.position = GenPoint(cc);

        return cc;
    }

    private Vector3 GenPoint(Character character) {
        float offset = 0.5f;
        float x = 0;
        float y = 0;

        switch(character.group) {
            case Character.Group.Hero:
                x = genHeroPoint;
                break;
            case Character.Group.Enemy:
                x = genEnemyPoint;
                break;
            default:
                break;
        }

        switch(character.type) {
            case Character.Type.Fly:
                y = flyPoint;
                break;
            case Character.Type.Swim:
                y = waterPoint;
                break;
            case Character.Type.Walk:
                y = walkPoint;
                break;
            case Character.Type.Tower:
                y = towerPoint;
                return new Vector3(x, y, 0);
                break;
            default:
                break;
        }

        //return new Vector3(x + Random.RandomRange(-offset, offset), y + Random.RandomRange(-offset, offset), 0);
        float characterX = character.group == Character.Group.Hero ? genHeroPoint : currentWave.SpawnDistance;
        return new Vector3(characterX + Random.Range(-offset, offset), y + Random.Range(-offset, offset), 0);
    }

    public List<Enemy> GetEnemiesInWave(int waveIndex) {
        List<Character> enemies = AllEnemy;
        List<Enemy> enemiesInWave = new List<Enemy>();
        foreach(var character in enemies) {
            var enemy = (Enemy)character;
            if(enemy.WaveIndex == waveIndex) {
                enemiesInWave.Add(enemy);
            }
        }

        return enemiesInWave;
    }

    bool IsLastWave(int waveIndex) {
        return CurrentLevel.waves.Count - 1 == waveIndex;
    }

    public void Reset() {
        foreach(var entry in AllEnemy) {
            Destroy(entry.gameObject);
        }

        foreach(var entry in AllHero) {
            Destroy(entry.gameObject);
        }

        heros.Clear();
        enemies.Clear();

        foreach(var item in EnemysDie) {
            Destroy(item.gameObject);
        }
        EnemysDie = new List<Character>();

        foreach(var item in HerosDie) {
            Destroy(item.gameObject);
        }
        HerosDie = new List<Character>();
    }

    private void OnDrawGizmos() {
        Gizmos.DrawLine(area.BottonLeft, new Vector3(area.BottonLeft.x, area.TopRight.y, 0));
        Gizmos.DrawLine(new Vector3(area.BottonLeft.x, area.TopRight.y, 0), area.TopRight);
        Gizmos.DrawLine(area.TopRight, new Vector3(area.TopRight.x, area.BottonLeft.y, 0));
        Gizmos.DrawLine(new Vector3(area.TopRight.x, area.BottonLeft.y, 0), area.BottonLeft);
    }
}

[System.Serializable]
public class Area {
    public Vector3 BottonLeft;
    public Vector3 TopRight;
}
