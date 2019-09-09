using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacker : MonoBehaviour
{
    protected Character character;

    public float TimeDelayAttack = 1;
    protected float time;

    protected virtual void OnEnable()
    {
        if (character == null)
        {
            character = GetComponent<Character>();
        }

        if (character != null)
        {
            character.OnAttack += OnAttack;
        }
    }

    protected virtual void OnDisable()
    {
        if (character != null)
        {
            character.OnAttack -= OnAttack;
        }
    }

    protected virtual void OnAttack(Damage damage)
    {
        if (time < Time.time)
        {
            Attack(damage);
            time = Time.time + TimeDelayAttack;
        }
    }

    protected virtual void Attack(Damage damage)
    {
        Character target = character.target;
        target.TakeHit(damage);
    }
}
