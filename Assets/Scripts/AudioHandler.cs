using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip signSound;
    [SerializeField] private AudioClip sosSound;
    [SerializeField] private AudioClip tpSound;

    public void PlaySignSound()
    {
        audioSource.PlayOneShot(signSound);
    }

    public void PlaySosSound()
    {
        audioSource.PlayOneShot(sosSound);
    }

    public void PlayTPSound()
    {
        audioSource.PlayOneShot(tpSound);
    }

}
