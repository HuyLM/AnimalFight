using Gemmob.Common.Data;
using System;
using System.Collections.Generic;

public partial class PrefData {
    private partial class Key {
        public const string CurrentCoin = "CurrentCoin";
        public const string CurrentGem = "CurrentGem";

        public static readonly string MYALLIE = "MYALLIE";
        public static readonly string MYHERO = "MYHERO";
        public static readonly string CURRENT_LEVEL = "CURRENT_LEVEL";
        public static readonly string MAX_LEVEL = "LEVEL_MAX";
    }

    public ulong CurrentCoin {
        get { return PersitenData.GetULong(Key.CurrentCoin, 0); }
        private set {
            if (value < 0) value = 0;
            PersitenData.SetULong(Key.CurrentCoin, value);
        }
    }

    public void UpdateCoin(ulong value, bool dispatchEvent = true) {
        CurrentCoin += value;
        if (dispatchEvent) EventDispatcher.Instance.Dispatch(EventKey.OnCoinChanged, value);
    }

    public ulong CurrentGem {
        get { return PersitenData.GetULong(Key.CurrentGem, 0); }
        private set {
            if (value < 0) value = 0;
            PersitenData.SetULong(Key.CurrentGem, value);
        }
    }

    public void UpdateGem(ulong value, bool dispatchEvent = true) {
        CurrentGem += value;
        if (dispatchEvent) EventDispatcher.Instance.Dispatch(EventKey.OnGemChanged, value);
    }


    public static List<HeroData> Heros {
        get {
            return IPlayerPrefs.Get<List<HeroData>>(Key.MYHERO, new List<HeroData>());
        }
    }

    public static List<AllieData> Allies {
        get {
            List<AllieData> allieDatas = IPlayerPrefs.Get<List<AllieData>>(Key.MYALLIE, new List<AllieData>());
            return allieDatas;
        }
    }

    public static int LevelMax {
        get {
            return IPlayerPrefs.GetInt(Key.MAX_LEVEL);
        }

        set {
            IPlayerPrefs.SetInt(Key.MAX_LEVEL, value);
        }
    }

    public static int CurrentLevel {
        get {
            return IPlayerPrefs.GetInt(Key.CURRENT_LEVEL);
        }

        set {
            IPlayerPrefs.SetInt(Key.CURRENT_LEVEL, value);
            if(LevelMax < CurrentLevel) {
                LevelMax = CurrentLevel;
            }
        }
    }

    public static HeroData CollectHero(CharacterData character, int level = 0) {
        List<HeroData> heros = Heros;

        string id = Guid.NewGuid().ToString();
        HeroData heroData = new HeroData(id, character.species, level);

        heros.Add(heroData);
        IPlayerPrefs.Set(Key.MYHERO, heros);
        return heroData;
    }
}
