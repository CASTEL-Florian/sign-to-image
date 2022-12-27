using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

[System.Serializable]
public class Gesture
{
    public string name;
    ///-------------used for grammar------------
    public string type;

    public List<Vector3> rightFingerDatas;
    public List<Vector3> leftFingerDatas;

    public List<Quaternion> rightFingerRotations;
    public List<Quaternion> leftFingerRotations;

    public Gesture()
    {
        name = "";
        type = "";
        rightFingerDatas = new List<Vector3>();
        leftFingerDatas = new List<Vector3>();
    }
}

// necessaire car JsonUtility.ToJson fait chier et veut pas �crire juste une putain de liste
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

    ///-------------used for grammar------------
    private string subject = "";
    private string place = "";
    private string other = "";

    public Request frame;
    public GesturesCanvasManagement gesturesCanvasManagement;

    // player movement
    public PlayerMovement playerMovement;
    public bool _playerCanMove = true;

    //particules system to show feedbacks
    public ParticleSystem particleLeftManager, particleRightManager;

    public ColorChange colorChangeLeft, colorChangeRight;

    public ParticleSystem sentenceParticlesLeft, sentenceParticlesRight;

    public PromptStyliser promptStyliser;
    // Start is called before the first frame update
    void Start()
    {
        // load data from the saveFile
        dataHandler = new FileDataHandler(Application.persistentDataPath, FileName);
        if(dataInputField)
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
        URLInputField.text = frame.url;
    }

    // Update is called once per frame
    void Update()
    {
        if(hasStarted)
        {
            Gesture currentGesture = Recognize();
            bool hasRecognized = !currentGesture.Equals(new Gesture());
            // check if its a new gesture

            if (hasRecognized && !currentGesture.Equals(previousGesture) && currentGesture.name != "")
            {
                previousGesture = currentGesture;
                gestureRecognitionInputField.text = currentGesture.name;
                if(currentGesture.name == "sos")
                {
                    if(_isPhrase)
                    {
                        _isPhrase = false;
                        sentenceParticlesLeft.Stop();
                        sentenceParticlesRight.Stop();
                        i = 0;
                        Generate();
                        Reset();
                    }    
                    else
                    {
                        _isPhrase = true;
                        for (int i = 0; i < phraseElements.Length; i++)
                        {
                            phraseElements[i] = "";
                        }
                        sentenceParticlesLeft.Play();
                        sentenceParticlesRight.Play();
                        i = 0;
                    }
                }
                else if (currentGesture.name == "move" && _playerCanMove)
                {
                    movePlayer();
                    previousGesture = null;
                }
                else if (currentGesture.name == "change" && _playerCanMove)
                {
                    promptStyliser.ChangeStyle();
                }
                else
                {
                    if(_isPhrase && currentGesture.name != "")
                    {
                        if(i == 0)
                        {
                            phrase += " " + currentGesture.name;
                        }
                        else
                        {
                            phrase += " in " + currentGesture.name; 
                        }
                        i ++;

                        ///-------------used for grammar------------
                        /*
                        if (currentGesture.type == "subject")
                        {
                            subject = currentGesture.name;
                        }
                        else if(currentGesture.type == "place")
                        {
                            place = " in " + currentGesture.name;
                        }
                        else if (currentGesture.type == "other")
                        {
                            other += " " + currentGesture.name;
                        }*/

                        i++;
                    }
                    particleLeftManager.Play();
                    particleRightManager.Play();
                    colorChangeLeft.Pulse();
                    colorChangeRight.Pulse();
                }
                
                ///-------------used for grammar------------
                //phrase = subject + place + other;
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
        cooldownInputField.text = "CLIC";
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
            List<Quaternion> rotationRightData = new List<Quaternion>();
            List<Quaternion> rotationLeftData = new List<Quaternion>();
            foreach (var bone in rightFingerBones)
            {
                // finger position relative to root
                rightData.Add(rightSkeleton.transform.InverseTransformPoint(bone.Transform.position));
                rotationRightData.Add(Quaternion.Inverse(rightSkeleton.transform.rotation) * bone.Transform.rotation);

            }

            foreach (var bone in leftFingerBones)
            {
                leftData.Add(leftSkeleton.transform.InverseTransformPoint(bone.Transform.position));
                rotationLeftData.Add(Quaternion.Inverse(leftSkeleton.transform.rotation) * bone.Transform.rotation);
            }
            
            g.name = inputField.text;
            g.rightFingerDatas = rightData;
            g.leftFingerDatas = leftData;
            g.rightFingerRotations = rotationRightData;
            g.leftFingerRotations = rotationLeftData;
            gestures.Add(g);

            // save data in the saveFile
            GestureList gestureList = new GestureList();
            gestureList.gestures = gestures;
            dataHandler.Save(gestureList);
            inputField.text = "";
            if (gesturesCanvasManagement)
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
        phraseField.text = " ";
        phrase = "";
        for (int i = 0; i < phraseElements.Length; i++)
        {
            phraseElements[i] = "";
        }
    }

    public void Generate()
    {
        frame.SetPrompt(promptStyliser.ApplyCurrentStyle(phrase));
        frame.Generate();
    }

    public void SetURL()
    {
        frame.url = URLInputField.text;
        PlayerPrefs.SetString("url",URLInputField.text);
    }

    public void movePlayer()
    {
        Vector3 startBone = rightFingerBones[6].Transform.position;
        Vector3 endBone = rightFingerBones[8].Transform.position;

        Vector3 direction = new Vector3 (startBone.x - endBone.x, startBone.y - endBone.y, startBone.z - endBone.z);
        direction = direction.normalized;
        direction.y = 0;
        debugLog.text = "x = " + direction.x.ToString();
        debugLog2.text = "z = " + direction.z.ToString();
        playerMovement.PlayerMove(-direction);
    }
}
