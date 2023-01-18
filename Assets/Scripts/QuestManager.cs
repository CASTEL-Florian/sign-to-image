using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest 
{
    public int rank;
    public string name;
    public string description;
    public List<string> requestedPromptItems;
    public float XPGiven;
    public bool _hasBeenDone;
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
    public List<Quest> currentQuestsList;
    public FileDataHandler playerDataHandler;
    private bool _canRedoneQuest = false;

    // const xp 
    private float xpToLevel2 = 100;
    private float xpToLevel3 = 150;
    private float xpToLevel4 = 200;
    private float xpToLevel5 = 300;

    // Start is called before the first frame update
    void Start()
    {
        playerDataHandler = new FileDataHandler(Application.persistentDataPath, PlayerDatasFileName);
        playerInfos = playerDataHandler.PlayerLoad();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // add a quest to the currentQuestsList, based on the level of the player
    public void AddQuest()
    {
        if(currentQuestsList.Count < 4)
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
                if (!quest._hasBeenDone || _canRedoneQuest)
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
            currentQuestsList.Add(questPool[randomIndex]);
        }
    }
        
}
