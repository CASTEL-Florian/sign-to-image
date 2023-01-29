using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    [SerializeField] private UnityEvent onTriggerEvent;
    [SerializeField] private bool onlyHandsTrigger = false;
    [SerializeField] AudioHandler audioHandler;
    SceneFader SceneFader;
    private void Awake()
    {
        audioHandler = FindObjectOfType<AudioHandler>();
        if (onTriggerEvent.GetPersistentEventCount() == 0)
        {
            SceneFader = FindObjectOfType<SceneFader>();
           
            onTriggerEvent.AddListener(loadScene);
        }
    }

    private void loadScene()
    {

        SceneFader.LoadScene(0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (onlyHandsTrigger)
        {
            if (other.tag == "Hand")
                onTriggerEvent.Invoke();
            return;
        }
        if (other.tag == "Hand"||other.tag=="Player")
        {
            audioHandler.StopGaleryleMusic();
           onTriggerEvent.Invoke();
        }
    }

    public void Activate()
    {
        onTriggerEvent.Invoke();
    }
}
