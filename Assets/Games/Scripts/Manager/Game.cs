using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : SubManager {

    public GameLoader gameLoader;

    private static Game instance;
    public static Game Instance
    {
        get
        {
            if (instance == null) {
                instance = FindObjectOfType<Game>();
            }
            return instance;
        }
    }

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
    }
}
