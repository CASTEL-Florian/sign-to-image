using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public TMP_InputField debugLog;
    [SerializeField] private CharacterController controller;
    private Vector3 initialPos;
    private void Start()
    {
        initialPos = transform.position;
    }
    public void PlayerMove(Vector3 direction)
    {
        controller.Move(speed * direction * Time.deltaTime);
        //transform.position += direction * Time.deltaTime * speed;
        debugLog.text = "isMoving";
    }

    public void ResetPos()
    {
        transform.position = initialPos;
    }

    public void Teleport(Vector3 newPos)
    {
        transform.position = newPos;
    }
}
