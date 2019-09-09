using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterData : ScriptableObject
{
    public string id;
    public Species species;

    public Sprite Icon;
     
    public GameObject characterPrefab;

    [Header("NotUpgrade")]
    [SerializeField] protected float rangeAttack;
    [SerializeField] protected float moveSpeed;

    public virtual void Initialize()
    {
        
    }

    public virtual float RangeAttack()
    {
        return rangeAttack;
    }

    public virtual float MoveSpeed()
    {
        return moveSpeed;
    }

    public abstract SecuredDouble Power();

    [System.Serializable]
    public class UpgradeData
    {
        public SecuredDouble hpMax;
        public SecuredDouble damage;
        public SecuredDouble mana;
        public SecuredDouble coin;
        public SecuredDouble fish;

        public UpgradeData(SecuredDouble hpMax, SecuredDouble damage, SecuredDouble mana, SecuredDouble coin, SecuredDouble fish)
        {
            this.hpMax = hpMax;
            this.damage = damage;
            this.mana = mana;
            this.coin = coin;
            this.fish = fish;
        }

        public UpgradeData()
        {
        }
    }
}

[System.Serializable]
public class Rank
{
    public RankType rankType;
    public List<CharacterData.UpgradeData> upgrades;


    public bool MaxLevel(int level)
    {
        return level == upgrades.Count - 1;
    }

    public int UpgradeCount
    {
        get
        {
            return upgrades.Count;
        }
    }

    public Rank(RankType rankType)
    {
        this.rankType = rankType;
        upgrades = new List<CharacterData.UpgradeData>();
    }
}

public enum Species
{
  Argon, Asen, Ceri, Dysprosi, Erbi, Gado, Lutexi, Scandi, Terbi, Tuli, Wyck, Ytter, Ytri, Holmium, Copen, BraveCat// AxeCat, BirdCat, BoogieCat, BraveCat,Cat, CowCat, CrazedCowCat, CrazedSexyLegsCat, DragonCat, EraserCat, FishCat,GatoAmigoCat, GiraffeCat, GrossCat, LizardCat, ManchoCat, MowHawkCat, r,s,u,v,x,z
}

public enum RankType { A = 3, B = 2, C = 1, D = 0, S = 4 }
public enum RankFillter { All = 5, A = 3, B = 2, C = 1, D = 0, S = 4 }