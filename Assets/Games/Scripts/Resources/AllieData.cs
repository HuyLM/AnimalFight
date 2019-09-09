using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class AllieData
{
    public string allieID;
    public Species species;
    public RankType rankType;
    public int currentLevel;

    public AllieData(string allieID, Species species, RankType rankType, int currentLevel)
    {
        this.allieID = allieID;
        this.species = species;
        this.rankType = rankType;
        this.currentLevel = currentLevel;
    }

    public int CurrentLevel
    {
        get
        {
            return currentLevel;
        }
    }

    public CharacterInfoData CharacterData
    {
        get
        {
            return (CharacterInfoData)GameResource.Instance.GetAllie(species);
        }
    }

    public SecuredDouble Damage
    {
        get
        {
            return CharacterData.Damage(rankType, CurrentLevel);
        }
    }

    public SecuredDouble Hp
    {
        get
        {
            return CharacterData.HPMax(rankType, CurrentLevel);
        }
    }

    public SecuredDouble Power
    {
        get
        {
            return 0;
        }
    }

    public SecuredDouble Mana
    {
        get
        {
            return CharacterData.Mana(rankType, CurrentLevel);
        }
    }

    public CharacterData.UpgradeData NextUpgrade()
    {

        return CharacterData.NextUpgrade(rankType, currentLevel);
    }

    public SecuredDouble NextCost
    {
        get {
            // return NextUpgrade().coin;
            return NextUpgrade().fish;
        }
    }

    public bool MaxUpgraded
    {
        get
        {
            return CharacterData.GetRank(rankType).MaxLevel(currentLevel);
        }
    }

    public int LevelMax
    {
        get
        {
            return CharacterData.GetRank(rankType).UpgradeCount - 1;
        }
    }

    public SecuredDouble CurrentMana;
    public bool Selected;
    public bool CanUse
    {
        get
        {
            return Mana >= GameResource.Instance.CurrentLevel.mana;
        }
    }

    public void SetSelect(bool value)
    {
        if (!CanUse)
        {
            return;
        }

        Selected = value;
        //EventDispatcher.Instance.Dispatch(EventName.ALLIE_SELECTED, this);
    }

    public void FillMana()
    {
        CurrentMana = Mana;
    }

    public void SpendMana()
    {
        CurrentMana -= 5;
    }

    public bool CanUpgrade(SecuredDouble currentCoin)
    {
        if (!MaxUpgraded && currentCoin >= NextCost)
        {
            return true;
        }
        return false;
    }

    public SecuredDouble Upgrade()
    {
        if (currentLevel >= LevelMax)
            return 0;

        currentLevel++;
        var cost = NextCost;
        return cost;
    }
}