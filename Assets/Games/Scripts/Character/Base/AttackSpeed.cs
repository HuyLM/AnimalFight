using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSpeed : MonoBehaviour
{
    float timeDelay;
    [SerializeField] float timeDelayMin=0.2f;
    [SerializeField] float deltaTime = 0.1f;
    [SerializeField] float timeAction = 2f;
    float timeDelayMax;
    float fistTime;

    private Attacker attacker;

    private void Awake()
    {
        attacker = GetComponent<Attacker>();

        timeDelay = attacker.TimeDelayAttack;
        timeDelayMax = timeDelay;
    }

    private void Update()
    {
        if (GameManager.IsState(GameState.Playing))
        {
            CheckTap();
            attacker.TimeDelayAttack = timeDelay;
        }
  
    }

    void CheckTap() {
        if (timeDelay > timeDelayMax) timeDelay = timeDelayMax;
        if (timeDelay < timeDelayMin) timeDelay = timeDelayMin;

        if (Input.GetMouseButtonUp(0)) {
            fistTime = Time.time;
            if (timeDelay >= timeDelayMin && timeDelay <= timeDelayMax)
            {
                timeDelay -= deltaTime;
            }
        }

        if (Time.time - fistTime > timeAction) {
            
            fistTime = Time.time;
            timeDelay += deltaTime;
        }
    }
}
