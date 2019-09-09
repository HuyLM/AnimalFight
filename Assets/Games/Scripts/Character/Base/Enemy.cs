using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    private bool isFlagHolder;
    private int waveIndex;
   // private AnimationBase anim;
    [HideInInspector] public float moveSpeed;

    private void Awake()
    {
       // anim = transform.GetComponent<AnimationBase>();
    }

    public bool IsFlagHolder
    {
        get { return isFlagHolder; }
    }

    public int WaveIndex
    {
        get { return waveIndex; }
    }

    public void SetFlagHolder(bool isFlagHolder, Transform flagHolder)
    {
        this.isFlagHolder = isFlagHolder;
        flagHolder = transform;
    }

    public void SetWaveIndex(int index)
    {
        waveIndex = index;
    }

    public override void Move()
    {
        //if (anim.animator.IsMove)
        //{
        //    moveSpeed = characterData.MoveSpeed();
        //}
        //else moveSpeed = 0;

        //if (OnMove != null)
        //{
        //    OnMove.Invoke(moveSpeed);
        //}
    }

    protected override void Die()
    {
        if (this.characterData.species != Species.Asen)
        {
            StartCoroutine(PlayDieAnim());
        }
        else
        {
            Game.Instance.gameLoader.RemoveCharacter(this);
            gameObject.SetActive(false);
        }
    }
    IEnumerator PlayDieAnim()
    {
        //anim.animator.Die(false);
        //yield return new WaitForSeconds(anim.animator.timeDie);
        yield return null;
        Game.Instance.gameLoader.RemoveCharacter(this);
        gameObject.SetActive(false);
    }


}
