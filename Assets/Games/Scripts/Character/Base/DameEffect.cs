using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class DameEffect : MonoBehaviour
{
    [SerializeField] private GameObject objectDame;
    private TextMeshPro txtDame;
    private Character character;

    private void Awake()
    {
        character = this.GetComponent<Character>();
        character.OnTakeHit += CreateDamEffect;
    }

    private void OnDestroy()
    {
        character.OnTakeHit -= CreateDamEffect;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    void CreateDamEffect(Damage dam)
    {
        var rd = UnityEngine.Random.Range(-0.5f, 0.5f);
        var randomDame = 1;
        var txtDameClone = PoolManager.Spawn(objectDame, transform.parent, new Vector3(transform.position.x + rd, transform.position.y + rd+1f, 0), Quaternion.identity);
        txtDame = txtDameClone.GetComponent<TextMeshPro>();
        txtDameClone.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.05f).OnComplete(() =>
        {
            txtDameClone.transform.DOMove(new Vector3(transform.position.x + rd, transform.position.y + 1.5f + rd, transform.position.z), 0.5f);
            txtDameClone.transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f);
        });
        int intDame = (int)(dam.damage * 1000 * randomDame);
        txtDame.SetText(intDame.ToString());
        txtDameClone.GetComponent<MonoBehaviour>().StartCoroutine(RecycleObject(txtDameClone));
    }

    private IEnumerator RecycleObject(GameObject c)
    {
        yield return new WaitForSeconds(0.5f);
        PoolManager.Recycle(c);
    }
}
