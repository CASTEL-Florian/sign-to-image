using UnityEngine;
[System.Serializable]

[CreateAssetMenu(fileName = "SignData", menuName = "Data/SignData", order = 0)]
public class SignsData : ScriptableObject
{
    // Start is called before the first frame update
    public Sprite SignSprite;
    public string SignNameText;
    public string SignDescriptionText;
}
