using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsGesturesCanvas : MonoBehaviour
{
    public string buttonName;
    public GesturesCanvasManagement gesturesCanvasManagement;

    public void DeleteGesture()
    {
        gesturesCanvasManagement.DeleteGesture(buttonName);
    }
}
