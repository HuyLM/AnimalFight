
public class GemInfo : CurrencyInfo {
	protected override ulong CurrentValue { get { return PrefData.Instance.CurrentGem; } }

	protected override void SetUpKeyEvent() {
		KeyEvent = EventKey.OnGemChanged;
	}

    protected override void OnClickAddButton() {

    }
}
