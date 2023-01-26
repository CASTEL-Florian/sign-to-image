using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoTp : MonoBehaviour
{
    [SerializeField] private List<GameObject> rightFingerBones;
    [SerializeField] private List<GameObject> leftFingerBones;
    [SerializeField] private float timeForEachSign = 1f;
    [SerializeField] private GestureDetection gestureDetection;

    [SerializeField] private GameObject rightHand;
    [SerializeField] private GameObject leftHand;
    [SerializeField] private Transform centerEye;
    [SerializeField] private float initialDistanceFromPlayer = 1f;
    private float currentTime = 0f;
    private string currentSign = "move";
    private List<Gesture> gestureList;

    private Vector3 initialPosLeft;
    private Vector3 initialPosRight;
    private Quaternion initialRotLeft;
    private Quaternion initialRotRight;

    private void Start()
    {
        rightHand.SetActive(false);
        leftHand.SetActive(false);
        StartCoroutine(WaitAndSetPosition());
    }

    private IEnumerator WaitAndSetPosition()
    {
        yield return new WaitForSeconds(0.5f);
        Vector3 pos = centerEye.position;
        pos.y = transform.position.y;
        transform.position = pos;
        transform.position = pos + centerEye.forward * initialDistanceFromPlayer;
        transform.LookAt(2 * transform.position - pos);
        initialPosLeft = leftHand.transform.position;
        initialRotLeft = leftHand.transform.rotation;
        initialRotRight = rightHand.transform.rotation;
        initialPosRight = rightHand.transform.position;
        rightHand.SetActive(true);
        leftHand.SetActive(true);
    }

    private void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime > timeForEachSign)
        {
            currentTime = 0;
            Swap();
        }
    }

    private void Swap()
    {
        gestureList = gestureDetection.gestures;
        currentSign = currentSign == "move" ? "tp" : "move";
        rightHand.transform.position = new Vector3(0, 0, 0);
        rightHand.transform.rotation = Quaternion.Euler(0, 0, 0);
        leftHand.transform.position = new Vector3(0, 0, 0);
        leftHand.transform.rotation = Quaternion.Euler(0, 0, 0);

        for (int i = 0; i < gestureList.Count; i++)
        {
            if (gestureList[i].name == currentSign)
            {

                for (int k = 0; k < gestureList[i].leftFingerDatas.Count; k++)
                {
                    rightFingerBones[k].transform.position = gestureList[i].rightFingerDatas[k];
                    rightFingerBones[k].transform.rotation = gestureList[i].rightFingerRotations[k];
                    leftFingerBones[k].transform.position = gestureList[i].leftFingerDatas[k];
                    leftFingerBones[k].transform.rotation = gestureList[i].leftFingerRotations[k];
                }
                rightHand.transform.position = initialPosRight;
                rightHand.transform.rotation = initialRotRight;
                leftHand.transform.position = initialPosLeft;
                leftHand.transform.rotation = initialRotLeft;
                break;
            }
        }
    }
}
