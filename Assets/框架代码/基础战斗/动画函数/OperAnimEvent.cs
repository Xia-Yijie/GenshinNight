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

    public void FightBegin()
    {
        if (oc_.tarIsNull) FightEnd();
        oc_.NorAtkStartCool();
        // if (!oc_.anim.GetBool("fight")) oc_.anim.SetBool("fight", true);
    }
    
    public void OnAttack()
    {
        oc_.OnAttack();
        oc_.NorAtkAction?.Invoke(oc_);
    }

    public void FightEnd()
    {
        oc_.anim.SetBool("fight", false);
    }

    public void OnDie()
    {
        if (oc_.dizziness > 0 && !oc_.dying) return;
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

public class SkillActionBuff : SkillBuffSlot
{
    private Action startAction;
    private Action endAction;

    public SkillActionBuff(BattleCore bc_, Action start, Action end) : base(bc_)
    {
        startAction = start;
        endAction = end;
    }

    public override void BuffStart()
    {
        startAction?.Invoke();
        base.BuffStart();
    }

    public override void BuffUpdate() { }

    public override void BuffEnd()
    {
        endAction?.Invoke();
        base.BuffEnd();
    }
}