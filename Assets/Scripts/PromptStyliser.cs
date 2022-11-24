using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NewBehaviourScript : MonoBehaviour
{
    public enum Style { NoStyle, Painting, Sketch, Drawing, DigitalArt, Abstract, Watercolor, Photo, Cubism }
    
    public string Stylise(string prompt, Style style)
    {
        if (style == Style.NoStyle)
            return prompt;
        string newPrompt = prompt;
        if (style == Style.Painting)
            newPrompt = "Oil painting of " + newPrompt;
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
        return newPrompt;
    }
}
