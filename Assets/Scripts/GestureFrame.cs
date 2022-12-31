using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GestureFrame : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private ButtonsGesturesCanvas deleteButton;
    [SerializeField] private ButtonsGesturesCanvas labelButton;
    [SerializeField] private ButtonsGesturesCanvas showGestureButton;

    public void SetName(string name)
    {
        deleteButton.buttonName = name;
        labelButton.buttonName = name;
        showGestureButton.buttonName = name;
        text.text = name;
    }

    public void SetLabel(string label)
    {
        labelButton.SetText(label);
    }
}
