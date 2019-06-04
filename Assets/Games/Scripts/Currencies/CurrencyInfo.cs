using UnityEngine;
using TMPro;
using Gemmob.Common.Data;
using System.Collections;

/** <summary> Base currency class which can automatic update value if changed. Add this to currency group. </summary> */
public abstract class CurrencyInfo : MonoBehaviour {
    [SerializeField] protected ButtonBase mainButton;
	[SerializeField] protected TextMeshProUGUI valueText;
    [SerializeField] protected float valueCounterDuration = 0.5f;

    private Coroutine coroutine;

	protected EventKey KeyEvent;

	protected abstract ulong CurrentValue { get; }
	protected abstract void SetUpKeyEvent();
    protected abstract void OnClickAddButton();

    protected virtual void Awake() {
        SetUpKeyEvent();
    }

	private void OnEnable() {
		OnValueChange(0);
		EventDispatcher.Instance.AddListener(KeyEvent, OnValueChange);
	}

	private void OnDisable() {
		EventDispatcher.Instance.RemoveListener(KeyEvent, OnValueChange);
	}

    protected virtual void Start() {
        if (mainButton == null) {
            mainButton = GetComponent<ButtonBase>();
            if (mainButton == null) mainButton = GetComponentInChildren<ButtonBase>();
        }

        if (mainButton != null) {
            mainButton.onClick.AddListener(OnClickAddButton);
        }
    }

    public virtual void RemoveEvent() {
		EventDispatcher.Instance.RemoveListener(KeyEvent, OnValueChange);
	}

	public virtual void ListenEvent() {
		EventDispatcher.Instance.AddListener(KeyEvent, OnValueChange);
	}

	private void OnValueChange(object value) {
        if (valueText == null) return;
        if (value == null) return;

        ulong changedValue = PersitenData.ToULong(value);
        if (changedValue < 2) {
            valueText.text = CurrentValue.ToString();
        }
        else {
            if (coroutine != null) {
                StopCoroutine(coroutine);
                coroutine = null;
            }
            coroutine = StartCoroutine(IETextCounter(valueText, CurrentValue - changedValue, CurrentValue));
        }
	}

	IEnumerator IETextCounter(TextMeshProUGUI text, ulong current, ulong target) {
        if (text == null) yield break;

		ulong start = current;
		for (float timer = 0; timer < valueCounterDuration; timer += Time.deltaTime) {
			float progress = timer / valueCounterDuration;
			current = PersitenData.ToULong(Mathf.Lerp(start, target, progress));
			text.text = current.ToString();
			yield return null;
		}
		text.text = target.ToString();
	}

}