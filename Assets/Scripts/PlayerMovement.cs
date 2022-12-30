using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public TMP_InputField debugLog;
    [SerializeField] private CharacterController controller;
    [SerializeField] private float speedLerpRate = 0.1f;
    private Vector3 currentVelocity = new Vector3();
    private Vector3 initialPos;
    private Vector3 direction;
    private void Start()
    {
        initialPos = transform.position;
    }

    public void PlayerMove(Vector3 dir)
    {
        direction = dir;
    }
    private void Update()
    {
        currentVelocity = Vector3.Lerp(currentVelocity, speed * direction, speedLerpRate);
        controller.SimpleMove(currentVelocity);
        if (direction.magnitude > 0)
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
