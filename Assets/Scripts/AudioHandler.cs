using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip signSound;
    [SerializeField] private AudioClip sosSound;
    [SerializeField] private AudioClip tpSound;
    [SerializeField] private AudioClip switchStyleSound;
    [SerializeField] private AudioClip GaleryMusic;
  
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
    public void PlaySwitchStyleSound()
    {
        audioSource.PlayOneShot(switchStyleSound);
    }
    public void PlayGaleryleMusic()
    {
        audioSource.clip = GaleryMusic;
        audioSource.loop = true;
        audioSource.Play();
    }
}
