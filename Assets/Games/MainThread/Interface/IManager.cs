using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IManager {
    /// <summary>
    /// Init param
    /// </summary>
    void Init();
    /// <summary>
    /// Play Game
    /// </summary>
    void StartGame();
    /// <summary>
    /// RePlay when you lose
    /// </summary>
    void RePlay();
    void Next();
    /// <summary>
    /// Call when player lose
    /// </summary>
    void GameOver();
    /// <summary>
    /// Call when player win
    /// </summary>
    void GameWin();
}
