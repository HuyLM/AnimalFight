using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Character : MonoBehaviour, ICharacter
{
    public enum Type { Fly, Swim, Walk, Tower }
    public enum State { None, Moving, Attack }
    public enum Group { Hero, Enemy }

    protected State state;

    public Group group;
    public Type type;
    public List<Type> targetTypes;

    public Action<float> OnMove;
    public Action<Damage> OnAttack;
    public Action<Damage> OnTakeHit;
    public Action OnReset;

    protected bool immortal;
    [SerializeField] protected SpriteRenderer hpView;

    protected CharacterData characterData;
    protected CharacterData.UpgradeData upgrade;

    public CharacterData CharacterData
    {
        get
        {
            return characterData;
        }
    }

    public CharacterData.UpgradeData Upgrade
    {
        get
        {
            return upgrade;
        }
    }

    public Character target
    {
        get
        {
            //return Game.Instance.gameLoader.TargetOf(this);
            return TestGameController.Instance.TargetOf(this);
        }
    }

    protected SecuredDouble currentHp = 1000;
    public SecuredDouble CurrentHp
    {
        get
        {
            return currentHp;
        }
        set
        {
            currentHp = value;
            if (hpView != null)
            {
                hpView.material.SetFloat("_Percent", (float)(currentHp / HPMax));
            }
        }
    }

    public SecuredDouble HPMax
    {
        get
        {
            //return upgrade.hpMax;
            return 1000;
        }
    }

    private Dictionary<string, float> buff = new Dictionary<string, float>();
    public SecuredDouble Damage
    {
        get
        {
            if (upgrade == null)
            {
                return 100;
            }
            if (buff.Count == 0)
            {
                return upgrade.damage;
            }

            if (buff.Count > 0)
            {
                SecuredDouble buffValue = 0;

                foreach (KeyValuePair<string, float> entry in buff)
                {
                    buffValue += (entry.Value - 1) * upgrade.damage;
                }

                return upgrade.damage + buffValue;
            }
            return upgrade.damage;
        }
    }

    public virtual bool Flip
    {
        get
        {
            if (target == null && group == Group.Enemy)
            {
                return true;
            }
            if (target == null && group == Group.Hero)
            {
                return false;
            }
            return transform.position.x - target.transform.position.x > 0;
        }
    }

    public virtual bool IsDie
    {
        get
        {
            return currentHp <= 0;
        }
    }

    private void OnEnable()
    {
        SetState(State.Moving);
    }

    protected virtual void Update()
    {

        if (!GameManager.IsState(GameState.Playing))
        {
            return;
        }

        if (IsState(State.None))
        {

        }

        if (target == null)
        {
            if (Game.Instance.gameLoader.AllEnemy.Count == 0)
            {
                SetState(State.Moving);
            }

            else
            {
                SetState(State.None);
            }
        }
        if (target != null)
        {
            if (Mathf.Abs(transform.position.x - target.transform.position.x) > 1f)// ato hard fix
            {
                SetState(State.Moving);
            }
            else
            {
                Debug.Log("attack");
                SetState(State.Attack);
            }
        }


    }

    public virtual void AddBuff(string id, float ratio)
    {
        buff.Add(id, ratio);
    }

    public virtual void RemoveBuff(string id)
    {
        buff.Remove(id);
    }

    public void SetState(State state)
    {
        this.state = state;
        switch (state)
        {
            case State.Attack:
                Attack();
                break;
            case State.Moving:
                Move();
                break;
            case State.None:
                break;
        }
    }

    public bool IsState(State state)
    {
        return this.state == state;
    }

    public virtual void SetGroup(Group group)
    {
        this.group = group;
    }

    public virtual void SetImmortal(bool value)
    {
        immortal = value;
    }

    public virtual void Initialize(CharacterData characterData, CharacterData.UpgradeData upgrade)
    {
        this.upgrade = upgrade;
        this.characterData = characterData;
        Game.Instance.gameLoader.AddCharacter(this);
        CurrentHp = HPMax;
    }

    public virtual void Attack()
    {
        if (OnAttack != null && IsDie==false)
        {
            OnAttack.Invoke(new Damage(DamageType.None, this, target, Damage));
        }
    }

    public virtual void Move()
    {
        if (OnMove != null)
        {
            //OnMove.Invoke(characterData.MoveSpeed());
            OnMove.Invoke(2);
        }
    }
   
    public virtual void TakeHit(Damage damage)
    {
        if (IsDie)
        {
            return;
        }

        if (immortal)
        {
            return;
        }

        damage.Active();

        SecuredDouble hit = damage.value;
        CurrentHp -= hit;
        if (CurrentHp <= 0)
        {
            CurrentHp = 0;
            Die();
        }

        if (OnTakeHit != null)
        {
            OnTakeHit.Invoke(damage);
        }
    }

    public virtual void Reset()
    {
        SetImmortal(false);
        if (OnReset != null)
        {
            OnReset.Invoke();
        }
    }

    protected virtual void Die()
    {
        Game.Instance.gameLoader.RemoveCharacter(this);
        gameObject.SetActive(false);
    }

    public void ChangeGroup(Group group)
    {
        Game.Instance.gameLoader.RemoveCharacter(this);
        SetGroup(group);
        Game.Instance.gameLoader.AddCharacter(this);
    }

    private void OnDrawGizmos()
    {
        //Character character = Game.Instance.gameLoader.TargetOf(this);
        //if (character != null)
        //{
        //    if (transform.position.x > character.transform.position.x)
        //    {
        //        Gizmos.color = Color.red;
        //    }
        //    else
        //    {
        //        Gizmos.color = Color.white;
        //    }

        //    Gizmos.DrawLine(transform.position, character.transform.position);
        //}

        //UnityEditor.Handles.Label(transform.position, string.Format("HP: {0} Dame: {1} Armor: {2}", CurrentHp, Damage, Armor));
    }
}