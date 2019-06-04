
public class CoinInfo : CurrencyInfo {
	protected override ulong CurrentValue { get { return PrefData.Instance.CurrentCoin; } }

	protected override void SetUpKeyEvent() {
		KeyEvent = EventKey.OnCoinChanged;
	}
    
    protected override void OnClickAddButton() {

    }
}
