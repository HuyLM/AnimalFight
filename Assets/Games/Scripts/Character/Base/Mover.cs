using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{

    protected Character character;
    protected Vector3 localScale;

    protected bool Flip
    {
        get
        {
            return character.Flip;
        }
    }

    protected virtual void OnEnable()
    {
        localScale = transform.localScale;
        if (character == null)
        {
            character = GetComponent<Character>();
        }

        if (character != null)
        {
            character.OnMove += OnMove;
        }
    }

    protected virtual void OnDisable()
    {
        if (character != null)
        {
            character.OnMove -= OnMove;
        }
    }

    protected virtual void OnMove(float moveSpeed)
    {
        float direction = Flip ? -1 : 1;
        float offset = moveSpeed * direction * Time.deltaTime;

        transform.position = new Vector3(transform.position.x + offset, transform.position.y, transform.position.z);
        transform.localScale = new Vector3(localScale.x * -direction, localScale.y, localScale.z);
    }
}
