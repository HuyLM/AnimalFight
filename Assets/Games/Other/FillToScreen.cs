using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FillToScreen : MonoBehaviour {

    public SpriteRenderer sprite;
    public Camera camera;

    private void OnEnable()
    {
        if (sprite == null) sprite = GetComponent<SpriteRenderer>();
        if (camera == null) camera = Camera.main;
        ResizeSpriteToScreen(sprite);
    }

    void ResizeSpriteToScreen(SpriteRenderer sr)
    {
        if (sr == null) return;

        sr.transform.localScale = new Vector3(1, 1, 1);

        var width = sr.sprite.bounds.size.x;
        var height = sr.sprite.bounds.size.y;

        var worldScreenHeight = camera.orthographicSize * 2.0f;
        var worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        float x = worldScreenWidth / width;
        float y = worldScreenHeight / height;

        sr.transform.localScale = new Vector2(x, y);
    }
}
