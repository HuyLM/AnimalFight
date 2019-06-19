using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IDebug : MonoBehaviour {

	private Text log;
	private static IDebug instance;

	public bool showLog;
	public static IDebug Instance{
		get {
			if (instance == null)
			{
				instance = FindObjectOfType<IDebug>();
				if (instance == null) instance = new IDebug();
			}
			return instance;
		}
	}

	private void Awake()
	{
		Init();
	}

	private void OnEnable()
	{
		Init();
	}

	void Init(){
		instance = this;
        if (log == null)
            log = GetComponent<Text>();
        log.enabled = showLog;
	}

	public void Log(string text){
		if (log != null) log.text += text + "\n";
	}
}
