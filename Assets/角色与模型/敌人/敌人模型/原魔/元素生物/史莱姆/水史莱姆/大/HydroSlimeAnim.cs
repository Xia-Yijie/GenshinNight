using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HydroSlimeAnim : MonoBehaviour
{
    private HydroSlime hs_;

    private void Awake()
    {
        hs_ = transform.parent.GetComponent<HydroSlime>();
    }

    public void SpAtkStart()
    {
        hs_.SpAtkStart();
        AudioManager.PlayEFF(hs_.paopaoOut);
    }

    public void SpAtkOut()
    {
        hs_.SpAtkOut();
    }

    public void SpAtkEnd()
    {
        hs_.SpAtkEnd();
    }
    
    
}
