using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

	public void OnClick()
    {
		IDebug.Instance.Log("Null sdasdasdasdasd");
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
           // StackUI.Instance.Show<WinUI>();
        }

        if (Input.GetKeyDown(KeyCode.B)) {
            StackUI.Instance.Show<LoseUI>();
        }

        if (Input.GetKeyDown(KeyCode.C)) {
            StackUI.Instance.HideAll();
        }
    }
}
