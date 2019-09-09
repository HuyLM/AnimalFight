using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

[Serializable]
public class HeroData
{
    public string allieID;
    public Species species;
    public int currentLevel;

    public HeroData(string allieID, Species species, int currentLevel)
    {
        this.allieID = allieID;
        this.species = species;
        this.currentLevel = currentLevel;
    }

    public int CurrentLevel
    {
        get
        {
            return currentLevel;
        }
    }

    public HeroInfoData HeroInfoData
    {
        get
        {
            return (HeroInfoData)GameResource.Instance.GetHero(species);
        }
    }

    public SecuredDouble Damage
    {
        get
        {
            return HeroInfoData.Damage(CurrentLevel);
        }
    }

    public SecuredDouble Hp
    {
        get
        {
            return HeroInfoData.HPMax(CurrentLevel);
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
            return HeroInfoData.Mana(CurrentLevel);
        }
    }

    public CharacterData.UpgradeData NextUpgrade()
    {

        return HeroInfoData.NextUpgrade(currentLevel);
    }

    public SecuredDouble NextCoin
    {
        get
        {
            return NextUpgrade().coin;
        }
    }

    public SecuredDouble NextFish
    {
        get
        {
            return ((HeroInfoData.HeroUpgradeData)NextUpgrade()).fish;
        }
    }

   /*public Image Avatar
    {
        get
        {
            return skin
        }
    }*/

    public bool MaxUpgraded
    {
        get
        {
            return HeroInfoData.MaxLevel(currentLevel);
        }
    }

    public int LevelMax
    {
        get
        {
            return HeroInfoData.UpgradeCount - 1;
        }
    }

    public bool Selected;

    public bool CanUpgrade(SecuredDouble currentCoin, SecuredDouble currentFish)
    {
        if (!MaxUpgraded && currentCoin >= NextCoin && currentFish >= NextFish)
        {
            return true;
        }
        return false;
    }

    public void Upgrade()
    {
        if (currentLevel >= LevelMax)
            return;

        currentLevel++;
       // EventDispatcher.Instance.Dispatch(EventName.HERO_UPGRADED, this);
    }
}
