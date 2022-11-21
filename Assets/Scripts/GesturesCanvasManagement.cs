using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GesturesCanvasManagement : MonoBehaviour
{
    public GestureDetection gestureDetection;
    private List<Gesture> gestureList;

    public GameObject[] GestureFrames;

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(DelayRoutine(1));
    }

    // Update is called once per frame

    public IEnumerator DelayRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        UpdateCanvas();
    }

    public void UpdateCanvas()
    {
        gestureList = gestureDetection.gestures;
        if (gestureList.Count != 0)
        {
            for (int i = 0; i < GestureFrames.Length; i++)
            {
                GestureFrames[i].SetActive(false);
            }

            for (int i = 0; i < gestureList.Count; i++)
            {
                GestureFrames[i].SetActive(true);
                GestureFrames[i].GetComponentInChildren<TextMeshProUGUI>().text = gestureList[i].name;
                GetComponentInChildren<ButtonVR>().onRelease.AddListener(delegate { DeleteGesture(gestureList[i].name); });
            }
        } 
    }

    public void DeleteGesture(string name)
    {
        for (int i = 0; i < gestureList.Count; i++)
        {
            if(gestureList[i].name == name)
            {
                gestureDetection.gestures.RemoveAt(i);
                break;
            }
        }
        GestureList gestureClass = new GestureList();
        gestureClass.gestures = gestureDetection.gestures;
        gestureDetection.dataHandler.Save(gestureClass);
        UpdateCanvas();
    }
}
