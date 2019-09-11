using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    protected Character character;
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;

    public float TimeDelayAttack = 2;
    protected float time;

    private void OnEnable()
    {
        animator = GetComponent<Animator>();
        character = GetComponentInParent<Character>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (character != null)
        {
            character.OnMove += OnMove;
            character.OnAttack += OnAttack;
            character.OnTakeHit += OnTakeHit;
        }
    }

    protected virtual void OnAttack(Damage damage)
    {
        if (time < Time.time)
        {
            animator.SetBool("move", false);
            animator.SetTrigger("attack");
            time = Time.time + TimeDelayAttack;
        }
    }

    protected virtual void OnMove(float moveSpeed)
    {
        animator.SetBool("move", true);

        spriteRenderer.sortingOrder = (int)(-transform.position.y * 10);
    }

    protected virtual void OnTakeHit(Damage damage) {
        animator.SetTrigger("takehit");
    }

    public void PlayIdle() {
        animator.Play("BraveIdle");
    }
    protected virtual void OnDisable()
    {
        if (character != null)
        {
            character.OnMove -= OnMove;
            character.OnAttack -= OnAttack;
        }
    }
}
