using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

    protected View view;
    protected int index;

    public virtual void UpdateUI(View view, object data)
    {
        this.view = view;
        this.index = view.Items.Count;
    }
}
