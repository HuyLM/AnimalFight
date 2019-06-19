using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour,IManager {

    protected bool initialized = false;

    public bool Initialized {
        get {
            return initialized;
        }
    }

    public virtual void Init()
    {
        initialized = true;
    }

    public virtual void GameOver()
    {

    }

    public virtual void RePlay()
    {

    }

    public virtual void StartGame()
    {

    }

    public virtual void GameWin()
    {

    }

    public virtual void Next()
    {

    }


}
