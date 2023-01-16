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
    [SerializeField] private TeleportHandler tpHandler;
    [SerializeField] private float maxTimeToTeleport = 1f;
    private float currentTime = 0;
    [SerializeField] private AudioHandler audioHandler;
    [SerializeField] private Transform cameraRig;
    [SerializeField] private Transform centerEyeAnchorTranform;
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
        currentTime += Time.deltaTime;
        currentVelocity = Vector3.Lerp(currentVelocity, speed * direction, speedLerpRate);
        controller.SimpleMove(currentVelocity);
        
        if (direction.magnitude > 0)
        {
            if (tpHandler)
            {
                tpHandler.Show();
                tpHandler.ShootRay();
            }
            currentTime = 0;
        }
        else
        {
            tpHandler.Hide();
        }
    }
    public void ResetPos()
    {
        Teleport(initialPos);
    }

    public void Teleport(Vector3 newPos)
    {
        controller.enabled = false;
        transform.position = newPos;
        Vector3 offset = centerEyeAnchorTranform.position - controller.transform.position;
        offset.y = 0;
        cameraRig.transform.position -= offset;
        controller.enabled = true;
    }
    public void TeleportToIndicator()
    {
        if (currentTime > maxTimeToTeleport)
            return;
        Trigger trigger = tpHandler.GetCurrentTrigger();
        if (trigger)
            trigger.Activate();
        else
        {
            if (audioHandler)
                audioHandler.PlayTPSound();
            Teleport(tpHandler.GetIndicatorPosition());
        }
    }
}
