namespace Gemmob.Common {
	public class EvenDisableListener : EventListenerBase {
		private void OnEnable() {
			if (listener != null) listener(true);
		}

		private void OnDisable() {
			if (listener != null) listener(false);
		}
	}
}