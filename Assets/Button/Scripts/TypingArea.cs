using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypingArea : MonoBehaviour
{
    public GameObject leftHand;
    public GameObject rightHand;
    public GameObject leftTypingHand;
    public GameObject rightTypingHand;

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponentInParent<OVRHand>().gameObject != null)
        {
            GameObject hand = other.GetComponentInParent<OVRHand>().gameObject;
            if(hand == null) 
                return;

            if(hand == leftHand)
            {
                leftTypingHand.SetActive(true);
            }
            else if (hand == rightHand)
            {
                rightTypingHand.SetActive(true);
            }
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.GetComponentInParent<OVRHand>().gameObject != null)
        {
            GameObject hand = other.GetComponentInParent<OVRHand>().gameObject;
            if(hand == null) 
                return;

            if(hand == leftHand)
            {
                leftTypingHand.SetActive(false);
            }
            else if (hand == rightHand)
            {
                rightTypingHand.SetActive(false);
            }
        } 
    }
}
