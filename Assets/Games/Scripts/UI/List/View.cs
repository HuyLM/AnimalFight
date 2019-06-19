using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class View : MonoBehaviour
{

    public GameObject itemPrefab;
    public Transform parent;
    protected List<Item> items = new List<Item>();
    public List<Item> Items
    {
        get { return items; }
    }

    protected abstract List<object> GetData();

    private void OnEnable()
    {
        Generate();
    }

    protected virtual void Generate()
    {
        Reset();

        if (GetData() != null)
        {
            for (int i = 0; i < GetData().Count; i++)
            {
                GameObject obj = Instantiate(itemPrefab, parent);
                Item itemComponent = obj.GetComponent<Item>();
                itemComponent.UpdateUI(this, GetData()[i]);
                obj.SetActive(true);
                items.Add(itemComponent);
            }
        }
    }

    protected virtual void Reset()
    {
        foreach (var item in items)
        {
            Destroy(item.gameObject);
        }

        items = new List<Item>();
    }
}
