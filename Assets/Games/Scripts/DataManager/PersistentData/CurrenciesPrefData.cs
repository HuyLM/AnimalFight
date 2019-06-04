using Gemmob.Common.Data;

public partial class PrefData {
    private partial class Key {
        public const string CurrentCoin = "CurrentCoin";
        public const string CurrentGem = "CurrentGem";
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

}
