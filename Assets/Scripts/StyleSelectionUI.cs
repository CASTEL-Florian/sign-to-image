using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StyleSelectionUI : MonoBehaviour
{
    [SerializeField] private PromptStyliser promptStyliser;
    [SerializeField] private AudioHandler audioHandler;

    [SerializeField] private Image styleImage;
    [SerializeField] private List<Texture> textures;
    private int styleIndex = 0;
    




    public void NextStyle()
    {
        styleIndex = (styleIndex + 1) % textures.Count;
        styleImage.material.SetTexture("_BaseMap", textures[styleIndex]);
        styleImage.material.SetTexture("_EmissionMap", textures[styleIndex]);
        audioHandler.PlaySwitchStyleSound();
        promptStyliser.ChangeStyle(styleIndex);
    }
}
