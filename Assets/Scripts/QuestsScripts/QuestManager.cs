using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Quest 
{
    public int rank;
    public string name;
    public string description;
    public List<string> requestedPromptItems;
    public float XPGiven;
    public bool _hasBeenDone;

    public Quest()
    {
        rank = 1;
        name = "test";
        description = "je suis une description etxt, normalement vous ne me voyez pas, sauf si on est en plein test";
        requestedPromptItems = new List<string>();
        XPGiven = 0;
        _hasBeenDone = false;
    }
}

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
    [HideInInspector]
    public PlayerInfos playerInfos;
    public Quest currentQuest;
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

    // Start is called before the first frame update
    void Start()
    {
        playerDataHandler = new FileDataHandler(Application.persistentDataPath, PlayerDatasFileName);
        playerInfos = playerDataHandler.PlayerLoad();
        //playerInfos = new PlayerInfos();
        Quest testquest = new Quest();
        Quest testquest2 = new Quest();
        testquest2.name = "test2";
        testquest2.description = "ceci est la meilleure description que vous verrez aujourd'hui !";
        playerInfos.quests.Add(testquest);
        playerInfos.quests.Add(testquest2);
        if (playerInfos.currentQuestsList.Count == 0)
        {
            AddQuest();
        }
        GenerateQuestPlaceHolder();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LevelUp()
    {
        switch(playerInfos.currentLevel)
        {
            case 1:
                if (playerInfos.currentXP > xpToLevel2)
                    playerInfos.currentLevel = 2;
                break;
            case 2:
                if (playerInfos.currentXP > xpToLevel3)
                    playerInfos.currentLevel = 3;
                break;
            case 3:
                if (playerInfos.currentXP > xpToLevel4)
                    playerInfos.currentLevel = 4;
                break;
            case 4:
                if (playerInfos.currentXP > xpToLevel5)
                    playerInfos.currentLevel = 5;
                break;
        }
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
            int randomIndex = Random.Range(0, questPool.Count);
            playerInfos.currentQuestsList.Add(questPool[randomIndex]);
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
        }
        for(int i = 0; i < playerInfos.currentQuestsList.Count; i++)
        {
            QuestPaperList[i].SetActive(true);
            QuestPaperList[i].GetComponent<QuestPaperManager>().quest = playerInfos.currentQuestsList[i];
            QuestPaperList[i].GetComponent<QuestPaperManager>().endPosition = endPosition;
        }
    }

    public void RemoveQuest(Quest quest)
    {
        foreach(Quest quest1 in playerInfos.currentQuestsList)
        {
            if (quest == quest1)
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
        float xpGained = 0;
        for(int i = 0; i < words.Length; i++)
        {
            if(quest.requestedPromptItems.Contains(words[i]))
            {
                xpGained += 0.5f;
            }
        }
        playerInfos.currentXP += xpGained * quest.XPGiven;
        LevelUp();
        quest._hasBeenDone = true;
        currentQuestPaper.GetComponent<QuestPaperManager>().RemoveQuest();
        SavePlayerInfos();
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
}
