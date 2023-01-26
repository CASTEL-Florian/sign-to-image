using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutoManager : MonoBehaviour
{
    [SerializeField] Transform CenterEyeAnchor;
    public GestureDetection gestureDetection;
    public GrimoireCarousel grimoireCarousel;
    [SerializeField] private TextMeshProUGUI tutoText;
    [SerializeField] private List<GameObject> textQuestManagerList;

    [SerializeField] private List<GameObject> textGrimoireList;
    [SerializeField] private List<GameObject> textTutoFindSubjectList;
    [SerializeField] private List<GameObject> textTutoFindLocationList;
    [SerializeField] private List<GameObject> textTutoFindSubjectRadicalList;
    [SerializeField] private List<GameObject> textTutoFindSubjectConceptList;

    [SerializeField] private List<GameObject> textSmallCanvasList;
    [SerializeField] private List<GameObject> textBigCanvasList;

    [HideInInspector] public bool _isInTuto;
    [HideInInspector] public float currentTutoStep;
    [HideInInspector] public bool _canChangeStep = false;

    private GameObject rightHand;
    private GameObject leftHand;
    private List<GameObject> rightFingerBones;
    private List<GameObject> leftFingerBones;
    private List<Gesture> gestureList;
    // Start is called before the first frame update
    void Start()
    {
        rightHand = grimoireCarousel.rightHand;
        leftHand = grimoireCarousel.leftHand;
        rightFingerBones = grimoireCarousel.rightFingerBones;
        leftFingerBones = grimoireCarousel.leftFingerBones;
        StartCoroutine(TutoStep1());
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = CenterEyeAnchor.rotation;
    }

    public IEnumerator TutoStep1()
    {
        tutoText.text = "Bonjour, bienvenue dans ton atelier.\n";
        yield return new WaitForSeconds(2f);
        tutoText.text += "Ici, tout fonctionne à partir des signes de tes mains";
        yield return new WaitForSeconds(5f);
        tutoText.text = "Avant toute chose, commençons par calibrer tes mains.\n ";
        yield return new WaitForSeconds(5f);
        tutoText.text += "Regarde-les, je t'en prie \n \n ";
        yield return new WaitForSeconds(5f);
        tutoText.text += "Calibration en cours...";
        StartCoroutine(gestureDetection.StartCalibration());
        yield return new WaitForSeconds(5f);
        tutoText.text = "Bien, maintenant, tu dois apprendre à te déplacer.\n\n";
        yield return new WaitForSeconds(5f);
        tutoText.text += "Fais le geste suivant : ";
        ShowGestureTuto("move");
        _canChangeStep = true;
    }

    public IEnumerator TutoStep2()
    {
        currentTutoStep = 2;
        _canChangeStep = false;
        StartCoroutine(DesactivateHandsTuto(3f));
        tutoText.text = "Très bien.\n\n";
        yield return new WaitForSeconds(2f);
        tutoText.text += "Maintenant, rejoins le tableau de commande pour en choisir une.";
        ActivateList(textQuestManagerList);
        _canChangeStep = true;
    }

    public IEnumerator TutoStep3()
    {
        DesactivateList(textQuestManagerList);
        currentTutoStep = 3;
        _canChangeStep = false;
        tutoText.text = "Très bien.\n\n";
        yield return new WaitForSeconds(2f);
        tutoText.text += "Avant de faire ce tableau, tu dois apprendre les bons signes. \n\n";
        yield return new WaitForSeconds(2f);
        tutoText.text += "Déplace toi vers le grimoire et ouvre le. \n\n";
        ActivateList(textGrimoireList);
        _canChangeStep = true;
    }

    public IEnumerator TutoStep4()
    {
        currentTutoStep = 4;
        _canChangeStep = false;
        tutoText.text = "Bien, trouve le geste pour ton sujet : \n";
        yield return new WaitForSeconds(2f);
        tutoText.text = "Monstre Marin";
        ActivateList(textTutoFindSubjectList);
        _canChangeStep= true;
    }

    public IEnumerator TutoStep4Concept()
    {
        DesactivateList(textTutoFindSubjectList);
        DesactivateList(textTutoFindSubjectRadicalList);
        currentTutoStep = 4.1f;
        ActivateList(textTutoFindSubjectConceptList);
        _canChangeStep = true;
        yield return null;
    }

    public IEnumerator TutoStep4Radical()
    {
        DesactivateList(textTutoFindSubjectList);
        DesactivateList(textTutoFindSubjectConceptList);
        currentTutoStep = 4.2f;
        ActivateList(textTutoFindSubjectRadicalList);
        _canChangeStep = true;
        yield return null;
    }
    public IEnumerator TutoStep5()
    {
        DesactivateList(textTutoFindSubjectList);
        currentTutoStep = 5;
        _canChangeStep = false;
        tutoText.text = "Bien, trouve le geste pour ton lieu : \n";
        yield return new WaitForSeconds(2f);
        tutoText.text = "Océan";
        ActivateList(textTutoFindLocationList);
        _canChangeStep = true;
    }

    public IEnumerator TutoStep6()
    {
        DesactivateList(textGrimoireList);
        currentTutoStep = 6;
        _canChangeStep = false;
        tutoText.text = "Bien, maintenant que tu maitrise les signes, tu vas pouvoir peindre\n";
        yield return new WaitForSeconds(3f);
        tutoText.text += "Dirige toi vers le tableau";
        ActivateList(textSmallCanvasList);
        _canChangeStep = true;
    }

    public IEnumerator TutoStep7()
    {
        yield return null;
    }

    private void ShowGestureTuto(string name)
    {
        gestureList = gestureDetection.gestures;

        for (int i = 0; i < gestureList.Count; i++)
        {
            if (gestureList[i].name == name)
            {               
                leftHand.SetActive(true);
                rightHand.SetActive(true);
          
                for (int k = 0; k < gestureList[i].leftFingerDatas.Count; k++)
                {
                    rightFingerBones[k].transform.position = gestureList[i].rightFingerDatas[k];
                    rightFingerBones[k].transform.rotation = gestureList[i].rightFingerRotations[k];
                    leftFingerBones[k].transform.position = gestureList[i].leftFingerDatas[k];
                    leftFingerBones[k].transform.rotation = gestureList[i].leftFingerRotations[k];
                }
                switch(name)
                {
                    case "move":
                        rightHand.transform.position = transform.position + new Vector3(-0.25f, 0f, 1.4f);
                        rightHand.transform.rotation = Quaternion.Euler(90f, 0, 90f);
                        leftHand.transform.position = transform.position + new Vector3(0.25f, 0f, 1.4f);
                        leftHand.transform.rotation = Quaternion.Euler(0, -90f, 90f);
                        break;
                }
                break;
            }
        }
    }

    private IEnumerator DesactivateHandsTuto(float delay)
    {
        yield return new WaitForSeconds(delay);
        rightHand.transform.position = new Vector3(0, 0, 0);
        rightHand.transform.rotation = Quaternion.Euler(0, 0, 0);
        leftHand.transform.position = new Vector3(0, 0, 0);
        leftHand.transform.rotation = Quaternion.Euler(0, 0, 0);
        leftHand.SetActive(false);
        rightHand.SetActive(false);
    }

    private void ActivateList(List<GameObject> gameObjectList)
    {
        foreach(GameObject g in gameObjectList)
        {
            g.SetActive(true);
        }
    }

    private void DesactivateList(List<GameObject> gameObjectList)
    { 
        foreach (GameObject g in gameObjectList)
        {
            g.SetActive(false);
        }
    }
}
