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
    [SerializeField] private Button retourButon;
    private SignsData[] Signs;
    private int currentSignIndex = 1;


   

    // Start is called before the first frame update
    private void Start()
    {
        initDic();

        //nextButton.onClick.AddListener(NextSign);
        //previousButton.onClick.AddListener(PreviousSign);

        // retourButon.onClick.AddListener(retour);

        // Recuperation des donnees des signes
        Signs = Resources.LoadAll<SignsData>("ScriptableObjects");

        UpdateGrimoireUI();
    }

    private void initDic()
    {
        EnName = new Dictionary<string, string>();
        EnName.Add("Marin", "sea");
        EnName.Add("Terrestre", "mountain");
        EnName.Add("Aerien", "flying");
        EnName.Add("Fantastique", "fantasy");
        EnName.Add("Lovecraftien", "lovecraftian");
        EnName.Add("Occulte", "occult");
        EnName.Add("Mystique", "mystic");
        EnName.Add("Surnaturel", "supernatural");
        EnName.Add("Lieu", "mountain");
        EnName.Add("Animal", "animal");
        EnName.Add("Monstre", "monster");
        EnName.Add("fantome", "ghost");
        EnName.Add("Bete", "beast");
        EnName.Add("Humain", "human");
        EnName.Add("Alien", "alien");
        EnName.Add("Mammifere", "mammal");
        EnName.Add("Fantome", "ghost");
        EnName.Add("Horreur", "horror");
        EnName.Add("Esprit", "spirit");
        EnName.Add("Entité", "entity");
    }

   /* private void retour()
    {
       
    }*/
 
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
                leftHand.SetActive(true);
                rightHand.SetActive(true);
                for (int k = 0; k < gestureList[i].leftFingerDatas.Count; k++)
                {
                    rightFingerBones[k].transform.position = gestureList[i].rightFingerDatas[k];
                    rightFingerBones[k].transform.rotation = gestureList[i].rightFingerRotations[k];
                    leftFingerBones[k].transform.position = gestureList[i].leftFingerDatas[k];
                    leftFingerBones[k].transform.rotation = gestureList[i].leftFingerRotations[k];
                }
                rightHand.transform.position = transform.position + new Vector3(0.25f, 0f, 0.4f) ;
                rightHand.transform.rotation = Quaternion.Euler(0, -90f, -90f);
                leftHand.transform.position = transform.position + new Vector3(-0.25f, 0f, 0.4f);
                leftHand.transform.rotation = Quaternion.Euler(0, -90f, 90f);
                showRoutine = StartCoroutine(DesactivateHands(10f));
                break;
            }
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

    public void EqName()
    {
        string name = transform.parent.parent.GetChild(2).GetComponent<TextMeshPro>().text;
        if(name != null)
            FindContaining(EnName[name]);
    }
}