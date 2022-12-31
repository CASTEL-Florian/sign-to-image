using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonsGesturesCanvas : MonoBehaviour
{
    public string buttonName;
    public GesturesCanvasManagement gesturesCanvasManagement;
    [SerializeField] private TextMeshProUGUI text;

    public void DeleteGesture()
    {
        gesturesCanvasManagement.DeleteGesture(buttonName);
    }

    public void ShowButton()
    {
        gesturesCanvasManagement.showGesture(buttonName);
    }

    public void LabelButton()
    {
        gesturesCanvasManagement.ChangeLabel(buttonName);
    }

    public void SetText(string txt)
    {
        text.text = txt;
    }
}
