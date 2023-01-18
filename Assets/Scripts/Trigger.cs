using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    [SerializeField] private UnityEvent onTriggerEvent;
    [SerializeField] private bool onlyHandsTrigger = false;
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
            onTriggerEvent.Invoke();
        }
    }

    public void Activate()
    {
        onTriggerEvent.Invoke();
    }
}
