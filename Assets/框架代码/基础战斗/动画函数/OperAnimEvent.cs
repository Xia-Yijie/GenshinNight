using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperAnimEvent : MonoBehaviour
{
    private OperatorCore oc_;

    private void Awake()
    {
        oc_ = transform.parent.GetComponent<OperatorCore>();
    }

    public void OnStart()
    {
        oc_.OnStart();
    }

    public void OnAttack()
    {
        oc_.OnAttack();
    }

    public void OnDie()
    {
        if (oc_.dizziness > 0 || !oc_.dying) return;
        oc_.OnDie();
    }

    public void skill1()
    {
        oc_.SkillAtk_1();
    }
    
    public void skill2()
    {
        oc_.SkillAtk_2();
    }
    
    public void skill3()
    {
        oc_.SkillAtk_3();
    }

}


public class SkillAnimStaBuff : SkillBuffSlot
{
    private Animator anim;
    private int sta;
    private int preSta;

    public SkillAnimStaBuff(BattleCore bc_, Animator Anim, int newSta) : base(bc_)
    {
        anim = Anim;
        sta = newSta;
        preSta = anim.GetInteger("sta");
    }

    public override void BuffStart()
    {
        anim.SetInteger("sta", sta);
        bc_.NorAtkClear();
        base.BuffStart();
    }

    public override void BuffUpdate() { }

    public override void BuffEnd()
    {
        anim.SetInteger("sta", preSta);
        bc_.NorAtkClear();
        base.BuffEnd();
    }
}