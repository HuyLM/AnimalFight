using System.Collections;
using System.Collections.Generic;
using System;

public interface ICharacter
{
    void Move();
    void Attack();
    void TakeHit(Damage damage);
}

[Serializable]
public class Damage
{
    public string id;
    public DamageType damageType;
    public Action OnAttacked;
    public Character owner;
    public Character target;
    public SecuredDouble damage;

    public SecuredDouble value
    {
        get
        {
            return damage;
        }
    }

    public Damage(DamageType damageType, Character owner, Character target, SecuredDouble damage)
    {
        this.damageType = damageType;
        this.owner = owner;
        this.target = target;
        this.damage = damage;
    }

    public Damage(DamageType damageType, Character owner, Character target, SecuredDouble damage, Action OnAttacked)
    {
        this.damageType = damageType;
        this.owner = owner;
        this.target = target;
        this.damage = damage;
        this.OnAttacked = OnAttacked;
    }

    public void Active()
    {
        if (OnAttacked != null)
        {
            OnAttacked.Invoke();
        }
    }
}

public enum DamageType
{
    None,
    Slow,
    Freeze,
}