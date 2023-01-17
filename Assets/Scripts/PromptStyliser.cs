using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PromptStyliser : MonoBehaviour
{
    [System.Serializable]
    public class StyleColor
    {
        public Style style;
        public Color color;
    }

    public enum Style { NoStyle, Painting, Sketch, Drawing, DigitalArt, Abstract, Watercolor, Photo, Cubism }
    
    
    [SerializeField] private List<StyleColor> styleColors;
    [SerializeField] private List<ColorChange> colorChanges;
    private int currentIndex;

    public string Stylise(string prompt, Style style, bool addQualityTags = true)
    {
        if (style == Style.NoStyle)
            return prompt + (addQualityTags ? ", masterpiece, intricate, high quality, 8k" : "");
        string newPrompt = prompt;
        if (style == Style.Painting)
            newPrompt = "Painting of " + newPrompt;
        if (style == Style.Sketch)
            newPrompt = "Professional pencil sketch of " + newPrompt;
        if (style == Style.Drawing)
            newPrompt = "Colored pencil drawing of " + newPrompt;
        if (style == Style.DigitalArt)
            newPrompt = newPrompt + ", artstation";
        if (style == Style.Abstract)
            newPrompt = "Abstract art of " + prompt;
        if (style == Style.Watercolor)
            newPrompt = "Watercolor painting of " + prompt;
        if (style == Style.Photo)
            newPrompt = "Photo of " + prompt;
        if (style == Style.Cubism)
            newPrompt = prompt + " in the style of cubism";
        return newPrompt + (addQualityTags ? ", masterpiece, intricate, high quality, 8k" : "");
    }

    public string ApplyCurrentStyle(string prompt)
    {
        return Stylise(prompt, styleColors[currentIndex].style);
    }

    private void Start()
    {
        foreach (ColorChange colorChange in colorChanges)
        {
            colorChange.ChangeColor(styleColors[0].color);
        }
    }

    public void ChangeStyle()
    {
        currentIndex = (currentIndex + 1) % styleColors.Count;
        foreach (ColorChange colorChange in colorChanges)
        {
            colorChange.ChangeColor(styleColors[currentIndex].color);
        }
    }
    public void ChangeStyle(int styleId)
    {
        currentIndex = styleId % styleColors.Count;
        foreach (ColorChange colorChange in colorChanges)
        {
            colorChange.ChangeColor(styleColors[currentIndex].color);
        }
    }
}
