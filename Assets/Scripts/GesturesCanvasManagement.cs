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


    public GameObject rightHand;
    public GameObject leftHand;

    public List<GameObject> rightFingerBones;
    public List<GameObject> leftFingerBones;

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
                GestureFrames[i].GetComponentInChildren<ButtonsGesturesCanvas>().buttonName = gestureList[i].name;
            }
            
        }
    }

    public void DeleteGesture(string name)
    {
        gestureList = gestureDetection.gestures;

        for (int i = 0; i < gestureList.Count; i++)
        {
            debugLog2.text = name;
            debugLog.text = gestureList[i].name;
            if (gestureList[i].name == name)
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

    public void showGesture(string name)
    {
        debugLog.text = "bouton show bien appuy�";
        gestureList = gestureDetection.gestures;

        for (int i = 0; i < gestureList.Count; i++)
        {
            debugLog2.text = name;
            debugLog.text = gestureList[i].name;
            if (gestureList[i].name == name)
            {
                leftHand.SetActive(true);
                rightHand.SetActive(true);
                for (int k = 0; k < gestureList[i].leftFingerDatas.Count; k++)
                {
                    debugLog2.text = "IN THE LOOP";
                    rightFingerBones[k].transform.position = gestureList[i].rightFingerDatas[k];
                    leftFingerBones[k].transform.position = gestureList[i].leftFingerDatas[k];
                }
                rightHand.transform.position = new Vector3(0.25f, 1.5f, 0.4f);
                rightHand.transform.rotation = Quaternion.Euler(0, 90f, -90f);
                leftHand.transform.position = new Vector3(-0.25f, 1.5f, 0.4f);
                leftHand.transform.rotation = Quaternion.Euler(0, 90f, 90f);
                StartCoroutine(DesactivateHands(10f));
                break;
            }
        }
       

    }

    public void PrintName(string name)
    {
        debugLog.text = name;
    }

    public IEnumerator DesactivateHands(float delay)
    {
        yield return new WaitForSeconds(delay);
        rightHand.transform.position = new Vector3(0, 0, 0);
        rightHand.transform.rotation = Quaternion.Euler(0, 0, 0);
        leftHand.transform.position = new Vector3(0, 0, 0);
        leftHand.transform.rotation = Quaternion.Euler(0, 0, 0);
        leftHand.SetActive(false);
        rightHand.SetActive(false);
    }
}
