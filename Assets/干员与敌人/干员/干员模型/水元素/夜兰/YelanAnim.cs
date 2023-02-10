using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class YelanAnim : MonoBehaviour
{
    private Yelan yelan;

    private void Awake()
    {
        yelan = transform.parent.GetComponent<Yelan>();
    }

    public void Skill1_ChargeBegin()
    {
        if (yelan.skillNum != 0 || !yelan.sp_.CanReleaseSkill()) return;
        GameObject charge = PoolManager.GetObj(yelan.Skill1_Charge);
        charge.transform.SetParent(yelan.transform);
        int dx = yelan.ac_.dirRight ? 1 : -1;
        charge.transform.localPosition = new Vector3(0.375f * dx, 0.465f, 0.587f);
        DurationRecycleObj recycleObj = new DurationRecycleObj(charge, 0.6f, yelan, true);
        BuffManager.AddBuff(recycleObj);
    }

    public void ShowLines()
    {
        yelan.StartCoroutine(yelan.ShowLines());
    }


    public void DisableLines()
    {
        yelan.StartCoroutine(yelan.DisableLines());
    }

    public void Skill3_End()
    {
        yelan.underLight.SetActive(false);
        yelan.DiceInHand.SetActive(false);
    }
    
    
}
