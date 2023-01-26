using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PyroSlimeAnim : MonoBehaviour
{
    private PyroSlime ps_;

    private void Awake()
    {
        ps_ = transform.parent.GetComponent<PyroSlime>();
    }

    public void SpAtkStart()
    {
        ps_.SpAtkStart();
    }

    public void SpAtkOut()
    {
        ps_.SpAtkOut();
    }
    
    public void SpAtkAudio()
    {
        AudioManager.PlayEFF(ps_.fireAudio);
    }

    public void SpAtkEnd()
    {
        ps_.SpAtkEnd();
    }
}
