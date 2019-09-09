using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Unity.Collections;
#if UNITY_EDITOR
using UnityEditor;

[CustomPropertyDrawer(typeof(CharacterType))]
public class CharacterTypeDrawer : PropertyDrawer
{
    private int h = 50;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // The 6 comes from extra spacing between the fields (2px each)
        return EditorGUIUtility.singleLineHeight * 6 + 6;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.indentLevel++;

        var speciesRect = new Rect(position.x, position.y, position.width, 16);
        var rankTypeRect = new Rect(position.x, position.y + 18, position.width, 16);
        var levelRect = new Rect(position.x, position.y + 36, position.width, 16);
        var countRect = new Rect(position.x, position.y + 54, position.width, 16);
        var lineRect = new Rect(position.x, position.y + 64, position.width, 16);

        if (Event.current.type == EventType.Repaint)
        {
            SerializedProperty sp = property.FindPropertyRelative("species");
            Species species = (Species)sp.enumValueIndex;
            CharacterData characterData = GameResource.Instance.GetCharacter(species);

            if (characterData != null)
            {
                Texture texture = AssetPreview.GetAssetPreview(characterData.Icon);
                var icon = new Rect(position.x, position.y + position.height / 2 - h / 2, h, h);
                GUI.Label(icon, texture);
            }
        }

        EditorGUI.PropertyField(speciesRect, property.FindPropertyRelative("species"));
        EditorGUI.PropertyField(rankTypeRect, property.FindPropertyRelative("rankType"));
        EditorGUI.PropertyField(levelRect, property.FindPropertyRelative("level"));
        EditorGUI.PropertyField(countRect, property.FindPropertyRelative("count"));

        GUI.Label(lineRect, "____________________________________________________________________________________________________________________________");

        EditorGUI.indentLevel--;

        EditorGUI.EndProperty();
    }

    public override bool CanCacheInspectorGUI(SerializedProperty property)
    {
        return false;
    }
}

#endif

[CreateAssetMenu(menuName = "Level", fileName = "Level")]
public class Level : ScriptableObject
{
    public string id;
   
    public int Index
    {
        get; set;
    }


    public SecuredDouble mana;
    public Bonus bonus;
    public LostBonus lostBonus;
    public List<Wave> waves = new List<Wave>();

    public int EnemyCount
    {
        get
        {
            int count = 0;
            foreach (var w in waves)
            {
                foreach (var item in w.characterTypes)
                {
                    count += item.count;
                }
            }
            return count;
        }
    }

#if UNITY_EDITOR
    public void Unload()
    {

    }
#endif

    public void Initialize()
    {

    }

    public bool IsCurrentLevel
    {
        get
        {
            return Index == PrefData.CurrentLevel;
        }
    }

    public bool LevelUnlocked
    {
        get
        {
            return (Index <= PrefData.LevelMax);
        }
    }

    public bool Passed
    {
        get
        {
            return Index < PrefData.LevelMax;
        }
    }

    [ContextMenu("Generate ID (if needed)")]
    public void GenerateID()
    {
        if (string.IsNullOrEmpty(id))
        {
            id = Guid.NewGuid().ToString();
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }
    }
}

[Serializable]
public class Wave
{
    public List<CharacterType> characterTypes = new List<CharacterType>();
    public float timeDelay;
	public float SpawnDistance;
}

[Serializable]
public class CharacterType
{
    public Species species;
    public RankType rankType;
    public int level;
    public int count;

    public CharacterData characterData
    {
        get
        {
            return GameResource.Instance.GetAllie(species);
        }
    }
}

[Serializable]
public class Bonus
{
    public int coin;
    public int gem;
    public int fish;
    public List<CharacterType> allies = new List<CharacterType>();

    public void Collect()
    {
        foreach (var item in allies)
        {
            for (int i = 0; i < item.count; i++)
            {
               // PrefData.CollectAllie(item.characterData, item.rankType, item.level);
            }
        }

        //Screw.Settings.AddCoin(coin);
       // Screw.Settings.AddFish(fish);
       // Screw.Settings.AddGem(gem);

        Debug.Log("Collect Bonus!");
    }

    public Bonus(int gem, int fish, int coin)
    {
        this.coin = coin;
        this.gem = gem;
        this.fish = fish;
    }
}
[Serializable]
public class LostBonus
{
    public int coin;
    public int fish;

    public void Collect()
    {
       // Screw.Settings.AddCoin(coin);
      /// PrefData.AddFish(fish);

        Debug.Log("Collect Lost Bonus!");
    }
}