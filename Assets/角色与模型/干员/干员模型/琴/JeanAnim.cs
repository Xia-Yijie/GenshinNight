using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JeanAnim : MonoBehaviour
{
    private Jean jean;

    private void Awake()
    {
        jean = transform.parent.GetComponent<Jean>();
    }

    public void Skill2_End()
    {
        jean.Skill2_End();
    }

    public void Skill3_Burst()
    {
        jean.Skill3_Burst();
    }

    public void Skill3_DomainHeal()
    {
        jean.Skill3_DomainHeal();
    }


}
