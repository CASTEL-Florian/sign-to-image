using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigPainting : MonoBehaviour
{
    [SerializeField] private float scaleMultiplier = 5f;
    [SerializeField] private float positionOffset = 10f;
    [SerializeField] private float timeToTransition = 1f;
    [SerializeField] private float activationDistance = 1f;
    [SerializeField] private List<GameObject> objectsToHide;
    [SerializeField] private List<GameObject> objectsToShow;
    [SerializeField] private List<MeshRenderer> rendererToHide;
    [SerializeField] private OVRScreenFade centerEyeAnchorFader;
    [SerializeField] private PlayerMovement player;
    [SerializeField] private Transform centerEyeAnchor;

    [SerializeField] private GameObject tutoCanvas;
    private Vector3 tutoCanvasPosBigFrame = new Vector3(0.86f, 1.92f, -3.345f);
    private Vector3 tutoCanvasPos;
    [SerializeField] private QuestManager questManager;
    [SerializeField] private Transform endPosBigFrame;
    private Vector3 ValAndXPPos;
    [SerializeField] private Transform ValAndXPosBigFrame;

    private bool activated = false;
    private bool transitioning = false;
    private float currentTime = 0;
    private Vector3 initialPos;

    // c pour les tests
    [SerializeField] private bool activia = false;
    [SerializeField] private bool desactivia = false;
    [SerializeField] private bool testTuto = false;
    private void Start()
    {
        initialPos = transform.position;
        ValAndXPPos = questManager.PopUpQuest.transform.position;
        tutoCanvasPos = tutoCanvas.transform.position; 
    }

    private void Update()
    {
        if (activia) // test
        {
            Activate();
            activia = false;
        }
        if(desactivia)  // test
        {
            Desactivate();
            desactivia = false;
        }
            
        if (transitioning)
            return;
        if (currentTime > timeToTransition)
        {
            if (activated)
                Desactivate();
            else
                Activate();
            currentTime = 0;
        }
        else if (activated)
        {
            if (DistanceXZ(centerEyeAnchor.position, initialPos) > activationDistance)
            {
                currentTime += Time.deltaTime;
            }
            else
            {
                currentTime = 0;
            }
        } else
        {
            if (DistanceXZ(centerEyeAnchor.position, transform.position) < activationDistance)
            {
                currentTime += Time.deltaTime;
            }
            else
            {
                currentTime = 0;
            }
        }
    }

    public void Activate()
    {
        if (transitioning || activated)
            return;

        if (tutoCanvas.GetComponent<TutoManager>()._isInTuto && tutoCanvas.GetComponent<TutoManager>().currentTutoStep < 6)
            return;

        StartCoroutine(ActivationRoutine());
    }

    private IEnumerator ActivationRoutine()
    {
        transitioning = true;
        centerEyeAnchorFader.FadeOut();
        yield return new WaitForSeconds(centerEyeAnchorFader.fadeTime);
        foreach (var item in objectsToHide)
        {
            if (item.GetComponent<QuestPaperManager>() != null)
            {
                if (item.GetComponent<QuestPaperManager>()._isSelected)
                {
                    item.GetComponent<QuestPaperManager>()._isSelectedInBigPainting = true;
                }
                else
                {
                    item.GetComponent<QuestPaperManager>().questManager._isInBigPicture = true;
                    item.SetActive(false);
                }    
            }   
            else
            {
                item.SetActive(false);
            }          
        }
        foreach (var item in objectsToShow)
        {
            item.SetActive(true);
        }
        foreach (var rend in rendererToHide)
        {
            rend.enabled = false;
        }
        if(tutoCanvas.GetComponent<TutoManager>()._isInTuto && tutoCanvas.GetComponent<TutoManager>().currentTutoStep == 6 || testTuto)
        {
            tutoCanvas.transform.position = tutoCanvasPosBigFrame;
            StartCoroutine(tutoCanvas.GetComponent<TutoManager>().TutoStep7());
        }  
        if(questManager.currentQuest != null)
        {
            questManager.currentQuestPaper.transform.position = endPosBigFrame.position;
            questManager.currentQuestPaper.transform.rotation = endPosBigFrame.rotation;
        }
        questManager.PopUpQuest.transform.position = ValAndXPPos;
        questManager.ExpGainSummary.transform.position = ValAndXPPos;

        player.Teleport(new Vector3(initialPos.x, player.transform.position.y, initialPos.z));
        transform.localScale = transform.localScale * scaleMultiplier;
        transform.Translate(-transform.forward * positionOffset);
        centerEyeAnchorFader.FadeIn();
        yield return new WaitForSeconds(centerEyeAnchorFader.fadeTime);
        transitioning = false;
        activated = true;
    }

    public void Desactivate()
    {
        if (transitioning || !activated)
            return;
        StartCoroutine(DesactivationRoutine());
    }

    private IEnumerator DesactivationRoutine()
    {
        transitioning = true;
        centerEyeAnchorFader.FadeOut();
        yield return new WaitForSeconds(centerEyeAnchorFader.fadeTime);
        foreach (var item in objectsToHide)
        {
            if (item.GetComponent<QuestPaperManager>() != null)
            {
                item.GetComponent<QuestPaperManager>().questManager._isInBigPicture = false;
                item.GetComponent<QuestPaperManager>()._isSelectedInBigPainting = false;
                if (!(item.GetComponent<QuestPaperManager>().quest == null))
                    item.SetActive(true);
            }
            else
            {
                item.SetActive(true);
            }         
        }
        foreach (var item in objectsToShow)
        {
            item.SetActive(false);
        }
        foreach (var rend in rendererToHide)
        {
            rend.enabled = true;
        }

        if (tutoCanvas.GetComponent<TutoManager>()._isInTuto && tutoCanvas.GetComponent<TutoManager>().currentTutoStep == 12 || testTuto)
        {
            tutoCanvas.transform.position = tutoCanvasPos;
            StartCoroutine(tutoCanvas.GetComponent<TutoManager>().TutoEnd());
        }
        if (questManager.currentQuest != null)
        {
            questManager.currentQuestPaper.transform.position = questManager.endPosition.position;
            questManager.currentQuestPaper.transform.rotation = questManager.endPosition.rotation;
        }
        questManager.PopUpQuest.transform.position = ValAndXPosBigFrame.position;
        questManager.ExpGainSummary.transform.position = ValAndXPosBigFrame.position;

        transform.localScale = transform.localScale / scaleMultiplier;
        transform.position = initialPos;
        player.ResetPos();
        centerEyeAnchorFader.FadeIn();
        yield return new WaitForSeconds(centerEyeAnchorFader.fadeTime);
        transitioning = false;
        activated = false;
    }

    private float DistanceXZ(Vector3 a, Vector3 b)
    {
        Vector3 c = new Vector3(a.x - b.x, 0, a.z - b.z);
        return c.magnitude;
    }
}
