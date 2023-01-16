using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    [SerializeField] private UnityEvent onTriggerEvent;
    [SerializeField] private bool handTrigger = false;
    private void OnTriggerEnter(Collider other)
    {
        if (handTrigger)
        {
            if (other.tag == "Hand")
                onTriggerEvent.Invoke();
            return;
        }
        if (other.tag == "Player")
        {
            onTriggerEvent.Invoke();
        }
    }

    public void Activate()
    {
        onTriggerEvent.Invoke();
    }
}
