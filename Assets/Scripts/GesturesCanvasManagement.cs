using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GesturesCanvasManagement : MonoBehaviour
{
    public GestureDetection gestureDetection;
    private List<Gesture> gestureList;

    public GameObject[] GestureFrames;

    public TMP_InputField debugLog, debugLog2;

    private int k = 0, j = 0;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DelayRoutine(1));
    }

    // Update is called once per frame

    public IEnumerator DelayRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        UpdateCanvas(gestureDetection.gestures);
    }

    public void UpdateCanvas(List<Gesture> gestureList)
    {
        j++;
        if (gestureList.Count != 0)
        {
            k++;
            for (int i = 0; i < GestureFrames.Length; i++)
            {
                GestureFrames[i].SetActive(false);   
            }
            
            for (int i = 0; i < gestureList.Count; i++)
            {
                GestureFrames[i].SetActive(true);
                GestureFrames[i].GetComponentInChildren<TextMeshProUGUI>().text = gestureList[i].name;
                GestureFrames[i].GetComponentInChildren<ButtonsGesturesCanvas>().buttonName = gestureList[i].name;
            }
            
        }
        debugLog.text = "j = " + j.ToString();
        debugLog2.text = " k = " + k.ToString();
    }

    public void DeleteGesture(string name)
    {
        gestureList = gestureDetection.gestures;

        for (int i = 0; i < gestureList.Count; i++)
        {
            if(gestureList[i].name == name)
            {
                gestureDetection.gestures.RemoveAt(i);
                break;
            }
        }

        // save the new gesture list 
        GestureList gestureClass = new GestureList();
        gestureClass.gestures = gestureDetection.gestures;
        gestureDetection.dataHandler.Save(gestureClass);

        // update the canvas with the new gesture list
        UpdateCanvas(gestureDetection.gestures);
    }

    public void PrintName(string name)
    {
        debugLog.text = name;
    }
}
