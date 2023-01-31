using System;
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
#nullable enable
    [SerializeField] private AudioClip? GaleryMusic;
    [SerializeField] private AudioClip? AtelierMusic;
    [SerializeField] private AudioSource atelier;
#nullable disable
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
    public void StopGaleryleMusic()
    {
        audioSource.Stop();
    }
    public void PlayAtelierleSound()
    {
        atelier.clip = AtelierMusic;
        atelier.loop = true;
        atelier.Play();
    }
    public void StopAtelierSound()
    {

        atelier.Stop();
    }
    public void playBackGroundClip(AudioClip backGroundAudioClip)
    {
        audioSource.clip = backGroundAudioClip;
        audioSource.loop = true;
        audioSource.Play();
    }
    public void StopBackGroundClip()
    {
        audioSource.Stop();
    }
}
