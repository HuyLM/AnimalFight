using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class TestGameController : MonoBehaviour {

    public List<Character> heros;
    public List<Character> enemys;

    public static TestGameController instance;
    public static TestGameController Instance {
        get {
            if(instance == null) {
                instance = FindObjectOfType<TestGameController>();
                return instance;
            }
            return instance;
        }
    }

    protected void Awake() {
        //GameObject[] arrHero = GameObject.FindGameObjectsWithTag("Hero");
        //heros = new List<Character>();
        //for(int i = 0; i < arrHero.Length; ++i) {
        //    heros.Add(arrHero[i].GetComponent<Character>());
        //}
        //GameObject[] arrEnemy = GameObject.FindGameObjectsWithTag("Enemy");
        //enemys = new List<Character>();
        //for(int i = 0; i < arrHero.Length; ++i) {
        //    enemys.Add(arrEnemy[i].GetComponent<Character>());
        //}
    }

    public Character TargetOf(Character character) {
        List<Character> characters = new List<Character>();

        switch(character.group) {
            case Character.Group.Hero:
                characters = enemys;
                characters.Sort((Character a, Character b) => Vector3.Distance(character.transform.position, a.transform.position).CompareTo(Vector3.Distance(character.transform.position, b.transform.position)));

                break;
            case Character.Group.Enemy:
                characters = heros;
                characters.Sort((Character a, Character b) => Vector3.Distance(character.transform.position, a.transform.position).CompareTo(Vector3.Distance(character.transform.position, b.transform.position)));

                break;
            default:
                break;
        }

        if(characters.Count > 0) {
            return characters[0];
        }

        return null;
    }

}
