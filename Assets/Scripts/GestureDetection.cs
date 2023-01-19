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
    /// string that act like a boolean 0 false 1 true
    public string unlock;

    public List<Vector3> rightFingerDatas;
    public List<Vector3> leftFingerDatas;

    public List<Quaternion> rightFingerRotations;
    public List<Quaternion> leftFingerRotations;

    public Gesture()
    {
        unlock = "0";
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

// used to save the distances of the real bones of the 2 hands
public class HandsDists
{
    public List<float> rightHandDists;
    public List<float> leftHandDists;

    public HandsDists()
    {
        rightHandDists = new List<float>();
        leftHandDists = new List<float>();
    }
}


public class GestureDetection : MonoBehaviour
{
    // save & load system
    [SerializeField] private string GesturesFileName;
    [SerializeField] private string HandsDatasFileName;
    public FileDataHandler gesturesDataHandler;
    public FileDataHandler handsDataHandler;
    public TMP_InputField dataInputField;


    public float threshold;
    public OVRPlayerController playerController;
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

    // feedback visuel en faisant apparaitre le nom du signe fait
    public FloatingImagesHandler floatingImagesHandler;

    // saved datas of the admin hands (for calibration)
    private HandsDists referenceHandsDists;
    private bool _isCalibrating;
    public GameObject CalibrationCanvas;

    // gestion des quêtes 
    public QuestManager questManager;
    private bool _hasToValidateQuest = false;

    public AudioHandler audioHandler;

    [SerializeField] private float minTimeBetweenSigns = 0.5f;

    [SerializeField] private float minSignHoldTime = 0.5f;
    private float currentTimeBetweenSigns = 0;
    private float currentSignHoldTime = 0;
    private bool currentGestureActivated = false;

    public BackgroundChanger backgroundChanger;

    public StyleSelectionUI styleSelectionUI;
    public bool test = false;

    // Start is called before the first frame update
    void Start()
    {
        // load data from the saveFile
        gesturesDataHandler = new FileDataHandler(Application.persistentDataPath, GesturesFileName);
        if (dataInputField)
            gesturesDataHandler.dataInputField = dataInputField;
        gestures = gesturesDataHandler.Load().gestures;
        previousGesture = new Gesture();

        handsDataHandler = new FileDataHandler(Application.persistentDataPath, HandsDatasFileName);
        referenceHandsDists = handsDataHandler.HandsDistsLoad();
        StartCoroutine(DelayRoutine(2.5f));
    }

    public IEnumerator DelayRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        rightFingerBones = new List<OVRBone>(rightSkeleton.Bones);
        leftFingerBones = new List<OVRBone>(leftSkeleton.Bones);
        if (URLInputField)
            URLInputField.text = frame.url;
        CalibrationCanvas.SetActive(true);    
    }

    // Update is called once per frame
    void Update()
    {
        currentTimeBetweenSigns += Time.deltaTime;
        bool playerMoving = false;
        if(hasStarted && !_hasToValidateQuest)
        {
            Gesture currentGesture = Recognize();
            bool hasRecognized = currentGesture.name != "";
            if (currentTimeBetweenSigns < minTimeBetweenSigns && currentGesture.name != "move" && currentGesture.name != "tp")
            {
                return;
            }
            if (!hasRecognized || !currentGesture.Equals(previousGesture))
            {
                currentGestureActivated = false;
                currentSignHoldTime = 0;
            }
            else
            {
                currentSignHoldTime += Time.deltaTime;
            }
            if (currentGesture.name == "move" && _playerCanMove)
            {
                movePlayer();
                playerMoving = true;
                previousGesture = null;
            }
            else if (currentGesture.name == "tp" && _playerCanMove && !currentGesture.Equals(previousGesture))
            {
                playerMovement.TeleportToIndicator();
                playerMoving = true;
            }
            previousGesture = currentGesture;
            if (!currentGestureActivated && currentSignHoldTime > minSignHoldTime)
            {
                currentGestureActivated = true;
                currentTimeBetweenSigns = 0;
                if (gestureRecognitionInputField)
                    gestureRecognitionInputField.text = currentGesture.name;
                if(currentGesture.name == "sos")
                {
                    if (audioHandler)
                        audioHandler.PlaySosSound();
                    if (_isPhrase)
                    {
                        _isPhrase = false;
                        floatingImagesHandler.SendImagesToTarget();
                        debugLog.text = "fin de phrase";
                        Generate();
                        if (questManager.currentQuest != null)
                        {
                            questManager.PopUpQuest.SetActive(true);
                            _hasToValidateQuest = true;
                        }
                        else
                            Reset();
                        sentenceParticlesLeft.Stop();
                        sentenceParticlesRight.Stop();
                        i = 0;
                    }    
                    else
                    {
                        if (!frame.generationEnded)
                            return;
                        debugLog.text = "début de phrase";
                        _isPhrase = true;
                        floatingImagesHandler.DeleteSymbols();
                        sentenceParticlesLeft.Play();
                        sentenceParticlesRight.Play();
                        i = 0;
                    }
                }
                else if (currentGesture.name == "change")
                {
                    if (styleSelectionUI)
                        styleSelectionUI.Toggle();
                }
                else
                {
                    if (audioHandler)
                        audioHandler.PlaySignSound();
                    if (_isPhrase && currentGesture.name != "")
                    {
                        ///-------------used for grammar------------
                        
                        if (currentGesture.type == "subject")
                        {
                            subject = currentGesture.name;
                        }
                        else if(currentGesture.type == "place")
                        {
                            place = " in " + currentGesture.name;
                        }
                        floatingImagesHandler.CreateImage(currentGesture.name, i % 2 == 0 ? rightFingerBones[8].Transform.position : leftFingerBones[8].Transform.position, currentGesture.type);
                        i++;
                    }
                    particleLeftManager.Play();
                    particleRightManager.Play();
                    colorChangeLeft.Pulse();
                    colorChangeRight.Pulse();
                }
                
                ///-------------used for grammar------------
                phrase = subject + place + other;
                if (phraseField)
                    phraseField.text = phrase;
            }
            else if(!hasRecognized)
            {
                gestureRecognitionInputField.text = " ";
            }
        }
        if (!playerMoving)
            playerMovement.PlayerMove(Vector3.zero);
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
            gesturesDataHandler.Save(gestureList);
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
        if (phraseField)
            phraseField.text = " ";
        phrase = "";
        subject = "";
        place = "";
        other = "";
    }

    public void Generate()
    {
        if (phrase == "")
            return;
        frame.SetPrompt(promptStyliser.ApplyCurrentStyle(phrase));
        frame.Generate();
        if (backgroundChanger)
            backgroundChanger.ChangeBackground(place);
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
        playerMovement.PlayerMove(-direction);
    }

    // functions of the 2 buttons on the pop up of the quest validation
    public void ValidateYes()
    {
        questManager.EvaluatePrompt(questManager.currentQuest, phrase);
        Reset();
        _hasToValidateQuest = false;
    }

    public void ValidateNo()
    {
        Reset();
        _hasToValidateQuest = false;
    }

    // ---------- welcome in the calibration world ----------

    // lance la calibration puis le jeu 
    public void CalibrationYes()
    {
        CalibrationCanvas.SetActive(false);
        StartCoroutine(StartCalibration()); 
    }

    // lance juste le jeu
    public void CalibrationNo()
    {
        CalibrationCanvas.SetActive(false);
        hasStarted = true;
    }

    // lance la calibration des mains de l'utilisateur et des gestes enregitrés
    public void Calibration()
    {
        HandsDists userHandsDists = new HandsDists();
        SaveDistances(userHandsDists);

        if (referenceHandsDists != null)
        {
            foreach (Gesture gesture in gestures)
            {
                CalibrateGesture(gesture, userHandsDists, referenceHandsDists);
            }
        }

        referenceHandsDists = userHandsDists;
    }
    public IEnumerator StartCalibration()
    {
        if(gesturesCanvasManagement.finGestureByName("calibration") != null)
        {
            gesturesCanvasManagement.showGesture("calibration");
        }
        _isCalibrating = true;
        cooldownCanvas.SetActive(true);
        cooldownInputField.text = "Calibration"; 
        yield return new WaitForSeconds(1f);
        cooldownInputField.text = "3";
        yield return new WaitForSeconds(1f);
        cooldownInputField.text = "2";
        yield return new WaitForSeconds(1f);
        cooldownInputField.text = "1";
        yield return new WaitForSeconds(1f);
        cooldownInputField.text = "CLIC";
        Calibration();
        yield return new WaitForSeconds(1f);
        cooldownCanvas.SetActive(false);
        _isCalibrating = false;
        hasStarted = true;
    }

    // enregistre les distance des mains de l'utilistateur dans handDist
    public void SaveDistances(HandsDists handsDists)
    {
        SaveDistancesOneHand(handsDists.leftHandDists, leftFingerBones);
        SaveDistancesOneHand(handsDists.rightHandDists, rightFingerBones);
        
        handsDataHandler.Save(handsDists);
    }

    // enregistre les distances d'une main de l'utilisateur 
    private void SaveDistancesOneHand(List<float> handDists, List<OVRBone> fingerBones)
    {
        // save distances of the thumb
        handDists.Add(Vector3.Distance(fingerBones[1].Transform.position, fingerBones[2].Transform.position));
        handDists.Add(Vector3.Distance(fingerBones[2].Transform.position, fingerBones[3].Transform.position));
        handDists.Add(Vector3.Distance(fingerBones[3].Transform.position, fingerBones[4].Transform.position));
        handDists.Add(Vector3.Distance(fingerBones[4].Transform.position, fingerBones[5].Transform.position));
        handDists.Add(Vector3.Distance(fingerBones[5].Transform.position, fingerBones[19].Transform.position));

        // save distances of the index
        handDists.Add(Vector3.Distance(fingerBones[1].Transform.position, fingerBones[6].Transform.position));
        handDists.Add(Vector3.Distance(fingerBones[6].Transform.position, fingerBones[7].Transform.position));
        handDists.Add(Vector3.Distance(fingerBones[7].Transform.position, fingerBones[8].Transform.position));
        handDists.Add(Vector3.Distance(fingerBones[8].Transform.position, fingerBones[20].Transform.position));

        // save distances pf the middle
        handDists.Add(Vector3.Distance(fingerBones[1].Transform.position, fingerBones[9].Transform.position));
        handDists.Add(Vector3.Distance(fingerBones[9].Transform.position, fingerBones[10].Transform.position));
        handDists.Add(Vector3.Distance(fingerBones[10].Transform.position, fingerBones[11].Transform.position));
        handDists.Add(Vector3.Distance(fingerBones[11].Transform.position, fingerBones[21].Transform.position));

        // save distances of the ring
        handDists.Add(Vector3.Distance(fingerBones[1].Transform.position, fingerBones[12].Transform.position));
        handDists.Add(Vector3.Distance(fingerBones[12].Transform.position, fingerBones[13].Transform.position));
        handDists.Add(Vector3.Distance(fingerBones[13].Transform.position, fingerBones[14].Transform.position));
        handDists.Add(Vector3.Distance(fingerBones[14].Transform.position, fingerBones[22].Transform.position));

        // save distances of the pinky 
        handDists.Add(Vector3.Distance(fingerBones[1].Transform.position, fingerBones[15].Transform.position));
        handDists.Add(Vector3.Distance(fingerBones[15].Transform.position, fingerBones[16].Transform.position));
        handDists.Add(Vector3.Distance(fingerBones[16].Transform.position, fingerBones[17].Transform.position));
        handDists.Add(Vector3.Distance(fingerBones[17].Transform.position, fingerBones[18].Transform.position));
        handDists.Add(Vector3.Distance(fingerBones[18].Transform.position, fingerBones[23].Transform.position));
    }
    // calibre un geste sur une taille de mains par rapport à celle rentrée en référence
    public void CalibrateGesture (Gesture gesture, HandsDists userHandsDists, HandsDists referenceHandsDist)
    {
        CalibrateGestureOneHand(gesture.rightFingerDatas, userHandsDists.rightHandDists, referenceHandsDist.rightHandDists);
        CalibrateGestureOneHand(gesture.leftFingerDatas, userHandsDists.leftHandDists, referenceHandsDist.leftHandDists);
    }

    // calibre une main d'un geste par rapport à une taille de main
    private void CalibrateGestureOneHand(List<Vector3> fingerDatas, List<float> userHandDists, List<float> referenceHandDist)
    {
        // calibrate the thumb
        Vector3 translationVector = findTranslationVector(fingerDatas[1], fingerDatas[2], userHandDists[0], referenceHandDist[0]);
        fingerDatas[2] += translationVector;
        fingerDatas[3] += translationVector;
        fingerDatas[4] += translationVector;
        fingerDatas[5] += translationVector;
        fingerDatas[19] += translationVector;

        translationVector = findTranslationVector(fingerDatas[2], fingerDatas[3], userHandDists[1], referenceHandDist[1]);
        fingerDatas[3] += translationVector;
        fingerDatas[4] += translationVector;
        fingerDatas[5] += translationVector;
        fingerDatas[19] += translationVector;

        translationVector = findTranslationVector(fingerDatas[3], fingerDatas[4], userHandDists[2], referenceHandDist[2]);
        fingerDatas[4] += translationVector;
        fingerDatas[5] += translationVector;
        fingerDatas[19] += translationVector;

        translationVector = findTranslationVector(fingerDatas[4], fingerDatas[5], userHandDists[3], referenceHandDist[3]);
        fingerDatas[5] += translationVector;
        fingerDatas[19] += translationVector;

        translationVector = findTranslationVector(fingerDatas[5], fingerDatas[19], userHandDists[4], referenceHandDist[4]);
        fingerDatas[19] += translationVector;

        // calibrate the index
        translationVector = findTranslationVector(fingerDatas[1], fingerDatas[6], userHandDists[5], referenceHandDist[5]);
        fingerDatas[6] += translationVector;
        fingerDatas[7] += translationVector;
        fingerDatas[8] += translationVector;
        fingerDatas[20] += translationVector;

        translationVector = findTranslationVector(fingerDatas[6], fingerDatas[7], userHandDists[6], referenceHandDist[6]);
        fingerDatas[7] += translationVector;
        fingerDatas[8] += translationVector;
        fingerDatas[20] += translationVector;

        translationVector = findTranslationVector(fingerDatas[7], fingerDatas[8], userHandDists[7], referenceHandDist[7]);
        fingerDatas[8] += translationVector;
        fingerDatas[20] += translationVector;

        translationVector = findTranslationVector(fingerDatas[8], fingerDatas[20], userHandDists[8], referenceHandDist[8]);
        fingerDatas[20] += translationVector;

        // calibrate the middle
        translationVector = findTranslationVector(fingerDatas[1], fingerDatas[9], userHandDists[9], referenceHandDist[9]);
        fingerDatas[9] += translationVector;
        fingerDatas[10] += translationVector;
        fingerDatas[11] += translationVector;
        fingerDatas[21] += translationVector;

        translationVector = findTranslationVector(fingerDatas[9], fingerDatas[10], userHandDists[10], referenceHandDist[10]);
        fingerDatas[10] += translationVector;
        fingerDatas[11] += translationVector;
        fingerDatas[21] += translationVector;

        translationVector = findTranslationVector(fingerDatas[10], fingerDatas[11], userHandDists[11], referenceHandDist[11]);
        fingerDatas[11] += translationVector;
        fingerDatas[21] += translationVector;

        translationVector = findTranslationVector(fingerDatas[11], fingerDatas[21], userHandDists[12], referenceHandDist[12]);
        fingerDatas[21] += translationVector;

        // calibrate the ring
        translationVector = findTranslationVector(fingerDatas[1], fingerDatas[12], userHandDists[13], referenceHandDist[13]);
        fingerDatas[12] += translationVector;
        fingerDatas[13] += translationVector;
        fingerDatas[14] += translationVector;
        fingerDatas[22] += translationVector;

        translationVector = findTranslationVector(fingerDatas[12], fingerDatas[13], userHandDists[14], referenceHandDist[14]);
        fingerDatas[13] += translationVector;
        fingerDatas[14] += translationVector;
        fingerDatas[22] += translationVector;

        translationVector = findTranslationVector(fingerDatas[13], fingerDatas[14], userHandDists[15], referenceHandDist[15]);
        fingerDatas[14] += translationVector;
        fingerDatas[22] += translationVector;

        translationVector = findTranslationVector(fingerDatas[14], fingerDatas[22], userHandDists[16], referenceHandDist[16]);
        fingerDatas[22] += translationVector;

        // calibrate the pinky
        translationVector = findTranslationVector(fingerDatas[1], fingerDatas[15], userHandDists[17], referenceHandDist[17]);
        fingerDatas[15] += translationVector;
        fingerDatas[16] += translationVector;
        fingerDatas[17] += translationVector;
        fingerDatas[18] += translationVector;
        fingerDatas[23] += translationVector;

        translationVector = findTranslationVector(fingerDatas[15], fingerDatas[16], userHandDists[18], referenceHandDist[18]);
        fingerDatas[16] += translationVector;
        fingerDatas[17] += translationVector;
        fingerDatas[18] += translationVector;
        fingerDatas[23] += translationVector;

        translationVector = findTranslationVector(fingerDatas[16], fingerDatas[17], userHandDists[19], referenceHandDist[19]);
        fingerDatas[17] += translationVector;
        fingerDatas[18] += translationVector;
        fingerDatas[23] += translationVector;

        translationVector = findTranslationVector(fingerDatas[17], fingerDatas[18], userHandDists[20], referenceHandDist[20]);
        fingerDatas[18] += translationVector;
        fingerDatas[23] += translationVector;

        translationVector = findTranslationVector(fingerDatas[18], fingerDatas[23], userHandDists[21], referenceHandDist[21]);
        fingerDatas[23] += translationVector;
    }

    private Vector3 findTranslationVector(Vector3 origin, Vector3 pointToMove, float userBoneDist, float referenceBoneDist)
    {
        Vector3 V1 = pointToMove - origin;
        float alpha = userBoneDist / referenceBoneDist;
        Vector3 translationVector = (alpha - 1f) * V1;
        return translationVector;
    }
}
