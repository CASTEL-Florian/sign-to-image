using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Frame : MonoBehaviour
{
    [SerializeField] private Image img;
    [SerializeField] private TextMeshProUGUI text;
    private Painting painting;

    public void SetPainting(Painting paint, Texture2D tex)
    {
        painting = paint;
        img.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2());
        img.material.SetTexture("_BaseMap", tex);
        if (text)
            text.text = painting.name;
    }

    public void Reset()
    {
        img.sprite = Sprite.Create(new Texture2D(2, 2), new Rect(0, 0, 2, 2), new Vector2());
        if (text)
            text.text = "";
    }
}
