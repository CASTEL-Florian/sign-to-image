using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class GrimoireCarousel : MonoBehaviour
{
    public GestureDetection gestureDetection;
    private List<Gesture> gestureList;

    public GameObject rightHand;
    public GameObject leftHand;

    public List<GameObject> rightFingerBones;
    public List<GameObject> leftFingerBones;
    private Coroutine showRoutine;

    private Dictionary<string,string> EnName;


/*    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;*/
    [SerializeField] private Image SignSpriteImageFirstPage;
    [SerializeField] private TextMeshProUGUI SignNameTextFirstPage;
    [SerializeField] private TextMeshProUGUI SignDescriptionTextFirstPage;
    [SerializeField] private Image SignSpriteImageSecondPage;
    [SerializeField] private TextMeshProUGUI SignNameTextSecondPage;
    [SerializeField] private TextMeshProUGUI SignDescriptionTextSecondPage;
    private SignsData[] Signs;
    private int currentSignIndex = 1;


    public TutoManager tutoManager;

    // Start is called before the first frame update
    private void Start()
    {
        initDic();

        //nextButton.onClick.AddListener(NextSign);
        //previousButton.onClick.AddListener(PreviousSign);


        // Recuperation des donnees des signes
        Signs = Resources.LoadAll<SignsData>("ScriptableObjects");

        UpdateGrimoireUI();
    }

    private void initDic()
    {
        EnName = new Dictionary<string, string>
        {
            { "Marin", "sea" },
            { "Terrestre", "mountain" },
            { "Aerien", "flying" },
            { "Fantastique", "fantasy" },
            { "Lovecraftien", "lovecraftian" },
            { "Occulte", "occult" },
            { "Mystique", "mystic" },
            { "Surnaturel", "supernatural" },
            { "Lieu", "mountain" },
            { "Animal", "animal" },
            { "Monstre", "monster" },
            { "fantome", "ghost" },
            { "Bete", "beast" },
            { "Humain", "human" },
            { "Alien", "alien" },
            { "Mammifere", "mammal" },
            { "Fantome", "ghost" },
            { "Horreur", "horror" },
            { "Esprit", "spirit" },
            { "Entité", "entity" },
            { "Phrase", "sos" },
            { "Move", "move" },
            { "TP", "tp" }
        };
    }

    public void NextSign()
    {
        currentSignIndex+=2;
        if (currentSignIndex >= Signs.Length)
        {
            currentSignIndex = 1;
        }

        UpdateGrimoireUI();
    }

    public void PreviousSign()
    {
        currentSignIndex-=2;
        if (currentSignIndex <= 0)
        {
            currentSignIndex = Signs.Length - 1;
        }

        UpdateGrimoireUI();
    }

    private void UpdateGrimoireUI()
    {       
        SignsData currentSign = Signs[currentSignIndex-1];
        SignNameTextFirstPage.SetText(currentSign.SignNameText);
        SignSpriteImageFirstPage.sprite = currentSign.SignSprite;
        SignDescriptionTextFirstPage.SetText(currentSign.SignDescriptionText.ToString());

        SignsData currentSignPage2 = Signs[currentSignIndex];
        SignNameTextSecondPage.SetText(currentSignPage2.SignNameText);
        SignSpriteImageSecondPage.sprite = currentSignPage2.SignSprite;
        SignDescriptionTextSecondPage.SetText(currentSignPage2.SignDescriptionText.ToString());    
    }


    public void showGesture(string name)
    {
        if (showRoutine != null)
        {
            rightHand.transform.position = new Vector3(0, 0, 0);
            rightHand.transform.rotation = Quaternion.Euler(0, 0, 0);
            leftHand.transform.position = new Vector3(0, 0, 0);
            leftHand.transform.rotation = Quaternion.Euler(0, 0, 0);
            leftHand.SetActive(false);
            rightHand.SetActive(false);
            StopCoroutine(showRoutine);
        }
        gestureList = gestureDetection.gestures;

        for (int i = 0; i < gestureList.Count; i++)
        {
            if (gestureList[i].name == name)
            {
                //gerer main droite main gauche ou les deux via ID
                if(currentSignIndex <= 9)
                {
                    leftHand.SetActive(true);
                    rightHand.SetActive(false);
                }
                else if(currentSignIndex <= 21)
                {
                    leftHand.SetActive(false);
                    rightHand.SetActive(true);
                }
                else
                {
                    leftHand.SetActive(true);
                    rightHand.SetActive(true);
                }
                for (int k = 0; k < gestureList[i].leftFingerDatas.Count; k++)
                {
                    rightFingerBones[k].transform.position = gestureList[i].rightFingerDatas[k];
                    rightFingerBones[k].transform.rotation = gestureList[i].rightFingerRotations[k];
                    leftFingerBones[k].transform.position = gestureList[i].leftFingerDatas[k];
                    leftFingerBones[k].transform.rotation = gestureList[i].leftFingerRotations[k];
                }
                rightHand.transform.position = transform.position + new Vector3(0.4f, 0f, -0.25f) ;
                rightHand.transform.rotation = Quaternion.Euler(0, 90f - transform.rotation.eulerAngles.y, -90f);
                leftHand.transform.position = transform.position + new Vector3(0.4f, 0f, 0.25f);
                leftHand.transform.rotation = Quaternion.Euler(0, 90f - transform.rotation.eulerAngles.y, 90f);
                showRoutine = StartCoroutine(DesactivateHands(10f));
                break;
            }
        }

        if(tutoManager._isInTuto && name == "monster" && tutoManager._canChangeStep)
        {
            if(tutoManager.currentTutoStep == 4)
            {
                StartCoroutine(tutoManager.TutoStep4Radical());
            }
            else if(tutoManager.currentTutoStep == 4.1f)
            {
                StartCoroutine(tutoManager.TutoStep5());
            }
        }

        if (tutoManager._isInTuto && name == "sea" && tutoManager._canChangeStep)
        {
            if (tutoManager.currentTutoStep == 4)
            {
                StartCoroutine(tutoManager.TutoStep4Concept());
            }
            else if (tutoManager.currentTutoStep == 4.2f)
            {
                StartCoroutine(tutoManager.TutoStep5());
            }
        }

        if(tutoManager._isInTuto && name == "sea" && tutoManager.currentTutoStep == 5 && tutoManager._canChangeStep)
        {
            StartCoroutine(tutoManager.TutoStep6());
        }
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


    //call when page touch
    private void FindContaining(string name)
    {
        string trueName = gestureDetection.gestures.Find(
        delegate (Gesture g)
        {
                return g.name.Contains(name);
        }
        ).name;
        showGesture(trueName);
    }

    //button call
    public void EqName(TextMeshProUGUI txt)
    {
        if (txt != null)
        {
            string name = txt.text;
            if (EnName.ContainsKey(name))
            {
                FindContaining(EnName[name]);
            }
        }
    }
}