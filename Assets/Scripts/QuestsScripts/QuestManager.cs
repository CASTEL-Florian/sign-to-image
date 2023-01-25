using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Quest
{
    public int rank;
    public string titre;
    public string description;
    public List<string> requestedPromptItems;
    public float XPGiven;
    public bool _hasBeenDone;

    public Quest()
    {
        rank = 1;
        titre = "test";
        description = "je suis une description etxt, normalement vous ne me voyez pas, sauf si on est en plein test";
        requestedPromptItems = new List<string>();
        XPGiven = 0;
        _hasBeenDone = false;
    }
}

[System.Serializable]
public class PlayerInfos
{
    public int currentLevel;
    public float currentXP;
    public List<Quest> currentQuestsList;
    public List<Quest> quests;

    public PlayerInfos()
    {
        currentLevel = 1;
        currentXP = 0;
        currentQuestsList = new List<Quest>();
        quests = new List<Quest>();
    }
}
public class QuestManager : MonoBehaviour
{
    public string PlayerDatasFileName;
    public PlayerInfos playerInfos;
    public Quest currentQuest;
    [HideInInspector]
    public FileDataHandler playerDataHandler;
    private bool _canRedoneQuest = false;

    // const xp 
    private float xpToLevel2 = 100;
    private float xpToLevel3 = 250;
    private float xpToLevel4 = 450;
    private float xpToLevel5 = 750;

    private GameObject currentQuestPaper;
    public List<GameObject> QuestPaperList;
    [SerializeField] private Transform endPosition;

    // diploma is here to track the current level of the player
    public GameObject DiplomaText;

    // pop up so the player can send or not his current painting for a quest
    public GameObject PopUpQuest;

    public bool _isPhrase;
    public bool ValidationOui;
    public bool ValidationNon;

    // pop up used to show the results of the woke done by the player during the quest
    [SerializeField] private GameObject ExpGainSummary;
    [SerializeField] private GameObject MarkText;
    [SerializeField] private GameObject NewXpText;
    [SerializeField] private GameObject LevelUpText;
    [SerializeField] private GameObject FinalCommentary;

    [HideInInspector] public bool _isInBigPicture = false;


    // Start is called before the first frame update
    void Start()
    {       
        playerDataHandler = new FileDataHandler(Application.persistentDataPath, PlayerDatasFileName);
        playerInfos = playerDataHandler.PlayerLoad();
        if (playerInfos.currentQuestsList.Count == 0)
        {
            AddQuest();
        }
        GenerateQuestPlaceHolder();
        UpdateDiploma();
        currentQuest = null;
    }

    // Update is called once per frame
    void Update()
    {
        if(_isPhrase && currentQuest != null)
        {
            _isPhrase = false;
            sendPhrase();
        }
        if (ValidationOui)
            ValidateYes(); ValidationOui = false;
        if (ValidationNon)
            ValidateNo(); ValidationNon = false;
    }

    public bool LevelUp()
    {
        switch(playerInfos.currentLevel)
        {
            case 1:
                if (playerInfos.currentXP > xpToLevel2)
                {
                    playerInfos.currentLevel = 2;
                    UpdateDiploma(); 
                    return true;
                }
                break;
                    
            case 2:
                if (playerInfos.currentXP > xpToLevel3)
                {
                    playerInfos.currentLevel = 3;
                    UpdateDiploma(); 
                    return true;
                }
                break;
            case 3:
                if (playerInfos.currentXP > xpToLevel4)
                {
                    playerInfos.currentLevel = 4; 
                    UpdateDiploma(); 
                    return true;
                }
                break;
                
            case 4:
                if (playerInfos.currentXP > xpToLevel5)
                {
                    playerInfos.currentLevel = 5; 
                    UpdateDiploma(); 
                    return true;
                }
                break;    
        }
        return false;
    }
    public void SavePlayerInfos()
    {
        playerDataHandler.Save(playerInfos);
    }

    // add a quest to the currentQuestsList, based on the level of the player
    public void AddQuest()
    {
        if (playerInfos.currentQuestsList.Count < 4)
        {
            List<Quest> questPool = new List<Quest>();
            int rank1percent = 0, rank2percent = 0, rank3percent = 0;

            switch (playerInfos.currentLevel)
            {
                case 1:
                    rank1percent = 8;
                    rank2percent = 2;
                    rank3percent = 0;
                    break;
                case 2:
                    rank1percent = 5;
                    rank2percent = 4;
                    rank3percent = 1;
                    break;
                case 3:
                    rank1percent = 3;
                    rank2percent = 6;
                    rank3percent = 1;
                    break;
                case 4:
                    rank1percent = 2;
                    rank2percent = 4;
                    rank3percent = 4;
                    break;
                case 5:
                    rank1percent = 1;
                    rank2percent = 3;
                    rank3percent = 6;
                    break;
            }
            foreach (Quest quest in playerInfos.quests)
            {
                if ((!quest._hasBeenDone || _canRedoneQuest) && !playerInfos.currentQuestsList.Contains(quest))
                {
                    switch (quest.rank)
                    {
                        case 1:
                            for (int i = 0; i < rank1percent; i++)
                            {
                                questPool.Add(quest);
                            }
                            break;
                        case 2:
                            for (int i = 0; i < rank2percent; i++)
                            {
                                questPool.Add(quest);
                            }
                            break;
                        case 3:
                            for (int i = 0; i < rank3percent; i++)
                            {
                                questPool.Add(quest);
                            }
                            break;
                    }
                }
            }
            if(questPool.Count > 0)
            {
                int randomIndex = Random.Range(0, questPool.Count);
                playerInfos.currentQuestsList.Add(questPool[randomIndex]);
            }
        }
    }
    
    public void SelectQuest(Quest quest, GameObject questPaper)
    {
        if(currentQuestPaper)
        {
            currentQuestPaper.GetComponent<QuestPaperManager>().DeselectQuest();
        }
        currentQuestPaper = questPaper;
        currentQuest = quest;
    }
    public void DeselectQuest()
    {
        currentQuestPaper = null;
        currentQuest = null;
    }

    public void GenerateQuestPlaceHolder()
    {
        for(int i = 0; i < QuestPaperList.Count; i++)
        {
            QuestPaperList[i].SetActive(false);
            QuestPaperList[i].GetComponent<QuestPaperManager>().quest = null;
        }
        for(int i = 0; i < playerInfos.currentQuestsList.Count; i++)
        {
            QuestPaperList[i].GetComponent<QuestPaperManager>().quest = playerInfos.currentQuestsList[i];
            QuestPaperList[i].GetComponent<QuestPaperManager>().endPosition = endPosition;
            if(QuestPaperList[i].GetComponent<QuestPaperManager>()._isSelected || !_isInBigPicture)
                QuestPaperList[i].SetActive(true);
        }
        SavePlayerInfos();
    }

    public void RemoveQuest(Quest quest)
    {
        foreach(Quest quest1 in playerInfos.currentQuestsList)
        {
            if (quest.titre == quest1.titre)
            {
                playerInfos.currentQuestsList.Remove(quest1);
                break;
            }
        }
        GenerateQuestPlaceHolder();
    }

    public void EvaluatePrompt(Quest quest, string prompt)
    {
        string[] words = prompt.Split(' ');
        float eval = 0;
        for(int i = 0; i < words.Length; i++)
        {
            if(quest.requestedPromptItems.Contains(words[i]))
            {
                eval += 0.5f;
            }
        }
        StartCoroutine(ShowResultCoroutine(eval, eval * quest.XPGiven));
        foreach (Quest quest1 in playerInfos.quests)
        {
            if (quest.titre == quest1.titre)
            {
                quest1._hasBeenDone = true;
                break;
            }
        }
        if(currentQuestPaper.GetComponent<QuestPaperManager>()._isSelectedInBigPainting)
        {
            currentQuestPaper.GetComponent<QuestPaperManager>()._isSelectedInBigPainting = false;
            currentQuestPaper.SetActive(false);
        }
        currentQuestPaper.GetComponent<QuestPaperManager>().RemoveQuest();
    }

    public void UpdateDiploma()
    {
        switch(playerInfos.currentLevel)
        {
            case 1:
                DiplomaText.GetComponent<TextMeshPro>().text = "DEBUTANT";
                break;
            case 2:
                DiplomaText.GetComponent<TextMeshPro>().text = "APPRENTIE";
                break;
            case 3:
                DiplomaText.GetComponent<TextMeshPro>().text = "PEINTRE CONFIRME";
                break;
            case 4:
                DiplomaText.GetComponent<TextMeshPro>().text = "SENIOR";
                break;
            case 5:
                DiplomaText.GetComponent<TextMeshPro>().text = "MAÎTRE";
                break;
        }
    }

    // --------------------- fonctions utilisées pour des tests -------------------------

    private string phrase = "fox in forest";

    public void ValidateYes()
    {
        EvaluatePrompt(currentQuest, phrase);
        PopUpQuest.SetActive(false);
        AddQuest();
        GenerateQuestPlaceHolder();
    }

    public void ValidateNo()
    {
        PopUpQuest.SetActive(false);
        //currentQuestPaper.GetComponent<QuestPaperManager>().DeselectQuest();
        AddQuest();
        GenerateQuestPlaceHolder();
        SavePlayerInfos();
    }

    public void sendPhrase()
    {
        if(currentQuest != null)
        {
            PopUpQuest.SetActive(true);
        }
    }


    public IEnumerator ShowResultCoroutine(float evaluation, float xpGained)
    {
        ExpGainSummary.SetActive(true);
        ExpGainSummary.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(0.5f);
        MarkText.GetComponent<TextMeshPro>().text = "Vous avez fait " + (evaluation * 100).ToString() + "% de ce que voulait le client.";
        MarkText.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        NewXpText.SetActive(true);
        if(xpGained == 0)
        {
            NewXpText.GetComponent<TextMeshPro>().text = "Exp : " + (playerInfos.currentXP).ToString() + " Xp";
        }
        else
        {
            for (float i = 0; i < xpGained; i++)
            {
                NewXpText.GetComponent<TextMeshPro>().text = "Exp : " + (playerInfos.currentXP + i + 1).ToString() + " Xp";
                yield return new WaitForSeconds(0.5f / xpGained);
            }
            playerInfos.currentXP += xpGained;
        }
        yield return new WaitForSeconds(0.5f);
        if(LevelUp())
        {
            LevelUpText.SetActive(true);
            yield return new WaitForSeconds(0.5f);
        }
        FinalCommentary.SetActive(true);
        switch(evaluation)
        {
            case 0:
                FinalCommentary.GetComponent<TextMeshPro>().text = "Faites mieux la prochaine fois !";
                break;
            case 0.5f:
                FinalCommentary.GetComponent<TextMeshPro>().text = "C'est pas mal !";
                break;
            case 1.0f:
                FinalCommentary.GetComponent<TextMeshPro>().text = "Bravo !";
                break;
        }
        yield return new WaitForSeconds(4.0f);
        MarkText.SetActive(false);
        NewXpText.SetActive(false);
        FinalCommentary.SetActive(false);
        ExpGainSummary.SetActive(false);
        SavePlayerInfos();
    }
}
