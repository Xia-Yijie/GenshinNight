using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lumine_Anemo_Anim : MonoBehaviour
{
    private Lumine_Anemo lumine;

    private void Awake()
    {
        lumine = transform.parent.GetComponent<Lumine_Anemo>();
    }

    public void Skill1_Begin()
    {
        lumine.Skill1_Begin();
    }
    
    
    
}
