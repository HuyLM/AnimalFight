using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(HeroInfoData))]
public class HeroInfoDataEditor : Editor
{
    private HeroInfoData characterInfoData;

    public override void OnInspectorGUI()
    {
        characterInfoData = (HeroInfoData)target;
        var texture = AssetPreview.GetAssetPreview(characterInfoData.Icon);
        GUILayout.Label(texture);
        base.OnInspectorGUI();
    }

}
#endif

[CreateAssetMenu(menuName = "CharacterData/HeroInfoData", fileName = "HeroInfoData")]
public class HeroInfoData : CharacterData
{

    public const string SKIN_ID = "SKIN_ID";
    public string KEY = "";
   
    public List<HeroUpgradeData> upgrades = new List<HeroUpgradeData>();
   // public List<SkinsData> skinsData = new List<SkinsData>();
     

    public string CurrentSkinID
    {
        get
        {
            return IPlayerPrefs.GetString(SKIN_ID);
        }
    }

    public void OnWakeUp()
    {
        //if (CurrentSkinID == "")
        //{
        //    if (skinsData.Count > 0)
        //    {
        //        SetCurrentSkin(skinsData[0].id);
        //    }
        //}

        //foreach (var item in skinsData)
        //{   
        //    item.heroInfoData = this;
        //}
    }

    public void SetCurrentSkin(string id)
    {
        IPlayerPrefs.SetString(SKIN_ID, id);
    }

    public int UpgradeCount
    {
        get
        {
            return upgrades.Count;
        }
    }

    public UpgradeData GetUpgrade(int level)
    {
        return upgrades[level];
    }

    public UpgradeData NextUpgrade(int level)
    {
        if (!MaxLevel(level))
        {
            return GetUpgrade(level + 1);
        }

        return GetUpgrade(UpgradeCount - 1);
    }

    public bool MaxLevel(int level)
    {
        return level == UpgradeCount - 1;
    }

    public virtual SecuredDouble HPMax(int level)
    {
        return upgrades[level].hpMax;
    }

    public virtual SecuredDouble Damage(int level)
    {
        return upgrades[level].damage;
    }

    public virtual SecuredDouble Mana(int level)
    {
        return upgrades[level].mana;
    }

    public override SecuredDouble Power()
    {
        return 0;
    }

    [System.Serializable]
    public class HeroUpgradeData : UpgradeData {
        //public SecuredDouble fish;

        public HeroUpgradeData(SecuredDouble hpMax, SecuredDouble damage, SecuredDouble mana, SecuredDouble coin, SecuredDouble fish) : base(hpMax, damage, mana, coin, fish)
        {
            this.fish = fish;
        }
    }
    
}
