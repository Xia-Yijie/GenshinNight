using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioCarrier : MonoBehaviour
{
    private AudioSource audio;

    private void Awake()
    {
        audio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!audio.isPlaying)
        {
            gameObject.SetActive(false);
            AudioManager.EFF_List.Add(this);
        }
    }

    public void PlayAudio(AudioClip clip, float volume = 1)
    {
        audio.clip = clip;
        audio.volume = volume;
        audio.Play();
    }
}
