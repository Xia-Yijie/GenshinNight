using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DehyaAnim : MonoBehaviour
{
    private Dehya dehya;

    private void Awake()
    {
        dehya = transform.parent.GetComponent<Dehya>();
    }

    public void LeftFistAudio()
    {
        AudioManager.PlayEFF(dehya.LeftFistAudio);
    }

    public void RightFistAudio()
    {
        AudioManager.PlayEFF(dehya.RightFistAudio);
    }
    
    
}
