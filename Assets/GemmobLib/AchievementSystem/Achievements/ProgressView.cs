using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressView : MonoBehaviour {

    public Text process;

	public void SetProgress(float percent) {
        process.text = percent * 100 + " %";
	}
}
