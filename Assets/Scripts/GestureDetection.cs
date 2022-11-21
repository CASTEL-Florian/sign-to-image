using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

[System.Serializable]
public class Gesture
{
    public string name;
    public List<Vector3> fingerDatas;


}

// necessaire car JsonUtility.ToJson fait chier et veut pas écrire juste une putain de liste
[System.Serializable]
public class GestureList
{
    public List<Gesture> gestures;
}


public class GestureDetection : MonoBehaviour
{
    // save & load system
    [SerializeField] private string FileName;
    public FileDataHandler dataHandler;
    public TMP_InputField dataInputField;


    public float threshold;
    public OVRSkeleton skeleton;
    public List<Gesture> gestures;
    private List<OVRBone> fingerBones;
    private Gesture previousGesture;

    public TMP_InputField inputField;
    public TMP_InputField gestureRecognitionInputField;
    public TMP_InputField debugLog, debugLog2, debugLog3, debugLog4, phraseField, URLInputField;

    private bool hasStarted = false;

    private bool _isTesting = true;
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
        fingerBones = new List<OVRBone>(skeleton.Bones);
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

    public void Save()
    {
        if(hasStarted)
        {
            Gesture g = new Gesture();
            g.name = "new gesture";
            List<Vector3> data = new List<Vector3>();
            foreach (var bone in fingerBones)
            {
                // finger position relative to root
                data.Add(skeleton.transform.InverseTransformPoint(bone.Transform.position));
            }
            
            g.name = inputField.text;
            g.fingerDatas = data;
            gestures.Add(g);

            // save data in the saveFile
            GestureList gestureList = new GestureList();
            gestureList.gestures = gestures;
            dataHandler.Save(gestureList);
            //gesturesCanvasManagement.UpdateCanvas();
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
            for(int i = 0; i < fingerBones.Count; i++)
            {
                Vector3 currentData = skeleton.transform.InverseTransformPoint(fingerBones[i].Transform.position);
                float distance = Vector3.Distance(currentData,gesture.fingerDatas[i]);
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
