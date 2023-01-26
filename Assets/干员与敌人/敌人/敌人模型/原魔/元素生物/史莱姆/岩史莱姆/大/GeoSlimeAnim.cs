using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeoSlimeAnim : MonoBehaviour
{
    private GeoSlime gs_;
    private Animator anim;

    private void Awake()
    {
        gs_ = transform.parent.GetComponent<GeoSlime>();
    }

    private void Start()
    {
        anim = gs_.anim;
    }

    public void ShieldUpEnd()
    {
        gs_.ShieldUpEnd();
    }

    public void ShieldDownEnd()
    {
        gs_.ShieldDownEnd();
    }

    private void ShieldGet()
    {
        gs_.ShieldGet();
    }
    
    
}
