using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectSlimeAnim : MonoBehaviour
{
    private ElectSlime es_;

    private void Awake()
    {
        es_ = transform.parent.GetComponent<ElectSlime>();
    }

    public void SpAtkStart()
    {
        es_.SpAtkStart();
    }
    
    public void SpAtkMove()
    {
        es_.SpAtkMove();
    }

    public void SpAtkAudio()
    {
        AudioManager.PlayEFF(es_.jumpAudio);
    }
    
    public void SpAtkDown()
    {
        es_.SpAtkDown();
        AudioManager.PlayEFF(es_.downAudio);
    }
    
    public void SpAtkEnd()
    {
        es_.SpAtkEnd();
    }
    
}
