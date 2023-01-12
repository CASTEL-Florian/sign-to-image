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
        if (other.tag == "Player")
        {
            onTriggerEvent.Invoke();
        }
    }
}
