using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    [SerializeField] private UnityEvent onTriggerEvent;
    private void OnTriggerEnter(Collider other)
    {
        print("trigger : " + other.tag);
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
