using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shopDialogAnimEvent : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void SetDisAppearFalse()
    {
        anim.SetBool("disappear", false);
    }
    
}
