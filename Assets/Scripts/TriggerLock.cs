using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerLock : MonoBehaviour
{
    public bool locked = false;
    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Hand")
        {
            locked = true;
        }


    }
    private void OnTriggerExit(Collider other)
    {

        if (other.tag == "Hand")
        {
            locked = false;
        }
    }
}
