using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestPaperManager : MonoBehaviour
{
    public QuestManager questManager;
    public Quest quest;
    [SerializeField] private GameObject QuestTitle;
    [SerializeField] private GameObject QuestRank;
    [SerializeField] private GameObject QuestDescription;

    private bool _isSelected = false;
    private bool _isMoving = false;
    private bool _canMove = false;
    private Vector3 startPosition;
    private Quaternion startRotation;
    [HideInInspector] public Transform endPosition;
    private float minDistance = 0f;
    private float speed = 2f;
    private float rotSpeed = 2f;

    [SerializeField] private GameObject QuestAccept;
    [SerializeField] private GameObject QuestRefuse;
    [SerializeField] private GameObject QuestDeselect;

    // Start is called before the first frame update
    void Start()
    {
        QuestTitle.GetComponent<TextMeshPro>().text = quest.name;
        QuestRank.GetComponent<TextMeshPro>().text = quest.rank.ToString();
        QuestDescription.GetComponent<TextMeshPro>().text = quest.description;
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if(_canMove)
        {
            _isMoving = true;
            if(_isSelected)
            {
                transform.position = Vector3.MoveTowards(transform.position, endPosition.position, Time.deltaTime * speed);
                transform.rotation = Quaternion.Lerp(transform.rotation, endPosition.rotation, rotSpeed * Time.deltaTime);              
                if (Vector3.Distance(transform.position, endPosition.position) <= minDistance)
                {
                    transform.rotation = endPosition.rotation;
                    _canMove = false;
                    _isMoving = false;
                    QuestAccept.SetActive(false);
                    QuestRefuse.SetActive(false);
                    QuestDeselect.SetActive(true);
                }
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, startPosition, Time.deltaTime * speed);
                transform.rotation = Quaternion.Lerp(transform.rotation, startRotation, rotSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, startPosition) <= minDistance)
                {
                    transform.rotation = startRotation;
                    _canMove = false;
                    _isMoving =false;
                    QuestAccept.SetActive(false);
                    QuestRefuse.SetActive(false);
                    QuestDeselect.SetActive(true);
                }
            }
        }
    }

    public void SelectQuest()
    {
        if(!_isMoving)
        {
            questManager.SelectQuest(quest, this.gameObject);
            _isSelected = true;
            _canMove = true;
        }
    }

    public void DeselectQuest()
    {
        questManager.DeselectQuest();
        _isSelected = false;
        _canMove = true;
    }

    public void RemoveQuest()
    {
        if(!_isSelected)
        {
            questManager.RemoveQuest(quest);
        }
    }
}
