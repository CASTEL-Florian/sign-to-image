using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Painting")]
public class PaintingData : ScriptableObject
{

    public int id;
    public string name;
    public string style;
    public string subject;
    public Sprite imageSprite;

    public List<Painting> images;
    public void AddImage(Painting img)
    {
        images.Add(img);
    }
}
