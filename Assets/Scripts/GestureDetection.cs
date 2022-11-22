using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

[System.Serializable]
public class Gesture
{
    public string name;
    public List<Vector3> rightFingerDatas;
    public List<Vector3> leftFingerDatas;
}

// necessaire car JsonUtility.ToJson fait chier et veut pas écrire juste une putain de liste
[System.Serializable]
public class GestureList
{
    public List<Gesture> gestures;

    public GestureList()
    {
        gestures = new List<Gesture>();
    }
}


public class GestureDetection : MonoBehaviour
{
    // save & load system
    [SerializeField] private string FileName;
    public FileDataHandler dataHandler;
    public TMP_InputField dataInputField;


    public float threshold;
    public OVRSkeleton rightSkeleton;
    public OVRSkeleton leftSkeleton;
    public List<Gesture> gestures;
    private List<OVRBone> rightFingerBones;
    private List<OVRBone> leftFingerBones;
    private Gesture previousGesture;

    public TMP_InputField inputField;
    public TMP_InputField gestureRecognitionInputField;
    public TMP_InputField debugLog, debugLog2, debugLog3, debugLog4, phraseField, URLInputField;

    // canvas to show cooldown when saving a new gesture
    public GameObject cooldownCanvas;
    public TMP_InputField cooldownInputField;
    private bool _isSaving;


    private bool hasStarted = false;

    int i = 0;

    private string start = "sos";
    private bool _isPhrase = false;
    private string phrase = "";

    public GameObject frame;
    public GesturesCanvasManagement gesturesCanvasManagement;


    // Start is called before the first frame update
    void Start()
    {
        // load data from the saveFile
        dataHandler = new FileDataHandler(Application.persistentDataPath, FileName);
        dataHandler.dataInputField = dataInputField;
        gestures = dataHandler.Load().gestures;

        previousGesture = new Gesture();
        StartCoroutine(DelayRoutine(2.5f));
    }

    public IEnumerator DelayRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        rightFingerBones = new List<OVRBone>(rightSkeleton.Bones);
        leftFingerBones = new List<OVRBone>(leftSkeleton.Bones);
        hasStarted = true;
        URLInputField.text = frame.GetComponent<Request>().url;
    }

    // Update is called once per frame
    void Update()
    {
        if(hasStarted)
        {
            Gesture currentGesture = Recognize();
            bool hasRecognized = !currentGesture.Equals(new Gesture());

            // check if its a new gesture
            if(hasRecognized && !currentGesture.Equals(previousGesture))
            {
                previousGesture = currentGesture;
                //currentGesture.onRecognized.Invoke();
                gestureRecognitionInputField.text = currentGesture.name;
                if(currentGesture.name == "sos" || i == 2)
                {
                    if(_isPhrase)
                    {
                        //frame.GetComponent<Request>().SetPrompt(phrase);
                        _isPhrase = false;
                    }    
                    else
                    {
                        _isPhrase = true;
                        i = 0;
                    }
                }
                else
                {
                    if(_isPhrase)
                    {
                        if(i == 0)
                        {
                            debugLog3.text = "sujet :" + currentGesture.name;
                            phrase += " " + currentGesture.name;
                        }
                        else
                        {
                            debugLog4.text = "lieu :" + currentGesture.name;
                            phrase += " in " + currentGesture.name; 
                        }
                        i ++;
                    }        
                }
                debugLog2.text = "_isPhrase = " + _isPhrase.ToString();
                phraseField.text = phrase;
            }
            else if(!hasRecognized)
            {
                gestureRecognitionInputField.text = " ";
            }
        }
    }

    public void SaveGestureButton()
    {
        if(!_isSaving)
        {
            StartCoroutine(SaveGesture());
        }
    }
    public IEnumerator SaveGesture()
    {
        _isSaving = true;
        cooldownCanvas.SetActive(true);
        cooldownInputField.text = "3";
        yield return new WaitForSeconds(1f);
        cooldownInputField.text = "2";
        yield return new WaitForSeconds(1f);
        cooldownInputField.text = "1";
        yield return new WaitForSeconds(1f);
        cooldownInputField.text = "KABOOOM";
        Save();
        yield return new WaitForSeconds(1f);
        cooldownCanvas.SetActive(false);
        _isSaving = false;
    }
    public void Save()
    {
        if(hasStarted)
        {
            Gesture g = new Gesture();
            List<Vector3> rightData = new List<Vector3>();
            List<Vector3> leftData = new List<Vector3>();
            foreach (var bone in rightFingerBones)
            {
                // finger position relative to root
                rightData.Add(rightSkeleton.transform.InverseTransformPoint(bone.Transform.position));
            }

            foreach (var bone in leftFingerBones)
            {
                leftData.Add(leftSkeleton.transform.InverseTransformPoint(bone.Transform.position));
            }
            
            g.name = inputField.text;
            g.rightFingerDatas = rightData;
            g.leftFingerDatas = leftData;
            gestures.Add(g);

            // save data in the saveFile
            GestureList gestureList = new GestureList();
            gestureList.gestures = gestures;
            dataHandler.Save(gestureList);
            inputField.text = "";
            gesturesCanvasManagement.UpdateCanvas(gestures);
        }
        else
        {
            gestureRecognitionInputField.text = "attend un peu boloss";
        }
    }

    Gesture Recognize()
    {
        Gesture currentgesture = new Gesture();
        float currentMin = Mathf.Infinity;

        foreach (var gesture in gestures)
        {
            float sumDistance = 0;
            bool isDiscarded = false;
            for(int i = 0; i < rightFingerBones.Count; i++)
            {
                Vector3 currentRightData = rightSkeleton.transform.InverseTransformPoint(rightFingerBones[i].Transform.position);
                Vector3 currentLeftData = leftSkeleton.transform.InverseTransformPoint(leftFingerBones[i].Transform.position);

                float rightDistance = Vector3.Distance(currentRightData,gesture.rightFingerDatas[i]);
                float leftDistance = Vector3.Distance(currentLeftData, gesture.leftFingerDatas[i]);
                float distance = Mathf.Max(rightDistance, leftDistance);
                debugLog.text = "Dist = " + distance.ToString();
                //debugLog2.text = (distance > 0.05f).ToString();
                if (distance > threshold)
                {
                    isDiscarded = true;
                    break;
                }

                sumDistance += distance;
                
            }
            if(!isDiscarded && sumDistance < currentMin)
            {
                currentMin = sumDistance;
                currentgesture = gesture;
            }
        }
        return (currentgesture);
    }

    public void Reset()
    {
        debugLog3.text = " ";
        debugLog4.text = " ";
        phraseField.text = " ";
        phrase = "";
    }

    public void Generate()
    {
        frame.GetComponent<Request>().SetPrompt(phrase);
        frame.GetComponent<Request>().Generate();
    }

    public void SetURL()
    {
        frame.GetComponent<Request>().url = URLInputField.text;
        PlayerPrefs.SetString("url",URLInputField.text);
    }
}
