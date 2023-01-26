using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Achou : OperatorCore
{
    [HideInInspector] public Itto itto;
    
    public override void OperInit()
    {
        base.OperInit();
        skillLevel[0] = itto.skillLevel[1];     // 阿丑的默认技能等级与一斗的2技能等级相同
        int lel = skillLevel[skillNum];
        sp_.Init(this, od_.maxSP0[lel], od_.maxSP0[lel], od_.duration0[lel],
            od_.skill0_recoverType, od_.skill0_releaseType, od_.spRecharge);

        sp_.ReleaseSkill(true);         // 部署后直接放技能
        
        tarPriority += 10000;           // 表示嘲讽，高优先级
        prePutOn = true;                // 撤退时可以直接撤
    }

    protected override void Start_Core()
    {
        base.Start_Core();
        gameObject.SetActive(false);
    }

    protected override void Update_Core()
    {
        base.Update_Core();
        if(sp_.CanReleaseSkill()) sp_.ReleaseSkill();
        if(!sp_.during) Retreat();
    }
}
