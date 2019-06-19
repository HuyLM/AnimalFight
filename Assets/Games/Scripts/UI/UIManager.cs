using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : SubManager {

    public override void GameOver() {
        StackUI.Instance.Show<LoseUI>();
    }

    public override void RePlay() {
        StackUI.Instance.HideAll();
    }

    public override void GameWin() {
        StackUI.Instance.Show<WinUI>();
    }

    public void Back() {
        
    }

}
