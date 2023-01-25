using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CryoSlimeAnim : MonoBehaviour
{
    private CryoSlime cs_;

    private void Awake()
    {
        cs_ = transform.parent.GetComponent<CryoSlime>();
    }

    public void SpAttackStart()
    {
        cs_.SpAtkStart();
    }

    public void SpAtkEnd()
    {
        cs_.SpAtkEnd();
    }
    
}
