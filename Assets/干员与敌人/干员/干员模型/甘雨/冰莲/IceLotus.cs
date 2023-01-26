using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceLotus : OperatorCore
{
    private Ganyu gy_;
    public GameObject StartBoom;
    public GameObject DieBoom;

    private float StartBoomRadius = 0.85f;
    private float DieBoomRadius = 1.1f;

    private bool firstInit = true;

    protected override void Awake_Core()
    {
        base.Awake_Core();
        gy_ = transform.parent.GetComponent<Ganyu>();
    }
    
    protected override void Start_Core()
    {
        base.Start_Core();
        gameObject.SetActive(false);
    }
    
    public override void OperInit()
    {
        base.OperInit();
        skillLevel[0] = gy_.skillLevel[1];     // 冰莲技能等级与甘雨的2技能等级相同（好看）
        int lel = gy_.skillLevel[1];
        sp_.Init(this, od_.maxSP0[lel], od_.maxSP0[lel], od_.duration0[lel],
            od_.skill0_recoverType, od_.skill0_releaseType, od_.spRecharge);

        sp_.ReleaseSkill(true);         // 部署后直接放技能，且不会触发技力相关委托
        
        tarPriority += 10000;           // 表示嘲讽，高优先级
        prePutOn = true;                // 撤退时可以直接撤
        
        ac_.LockRolAndRight();          // 锁定旋转

        life_.ChangeBaseValue(gy_.life_.val * gy_.iceLotusLifeMulti[lel]);  // 继承生命
        
        // 制造爆炸
        if (firstInit)
        {
            firstInit = false;
            Retreat();
            return;
        }
        GameObject boom = PoolManager.GetObj(StartBoom);
        boom.transform.position = transform.position;
        DurationRecycleObj recycleObj = new DurationRecycleObj(boom, 1f);
        BuffManager.AddBuff(recycleObj);

        var tars = InitManager.GetNearByEnemy(transform.position, StartBoomRadius);
        float dam = gy_.atk_.val * gy_.skill2_Multi[gy_.skillLevel[1]];
        ElementSlot cryoSlot = new ElementSlot(ElementType.Cryo, 1f);
        foreach (var EC in tars)
        {
            gy_.Battle(EC, dam, DamageMode.Physical, cryoSlot, true, true);
        }
        
        // 死亡时的爆炸委托
        DieAction += DieBoomAction;
        // 甘雨死亡时撤退
        gy_.DieAction += GanyuDie;
    }
    
    protected override void Update_Core()
    {
        base.Update_Core();
        if(sp_.CanReleaseSkill()) sp_.ReleaseSkill();
        if (!sp_.during && !dying)
        {// 时间到了受到1e8点伤害
            GetDamage(1e8f, DamageMode.Magic);
        }
    }

    private void DieBoomAction(BattleCore bc)
    {
        // 制造爆炸
        GameObject boom = PoolManager.GetObj(DieBoom);
        boom.transform.position = transform.position;
        DurationRecycleObj recycleObj = new DurationRecycleObj(boom, 1f);
        BuffManager.AddBuff(recycleObj);

        var tars = InitManager.GetNearByEnemy(transform.position, DieBoomRadius);
        float dam = gy_.atk_.val * gy_.skill2_Multi[gy_.skillLevel[1]];
        ElementSlot cryoSlot = new ElementSlot(ElementType.Cryo, 1f);
        foreach (var EC in tars)
        {
            gy_.Battle(EC, dam, DamageMode.Physical, cryoSlot, true, true);
        }
        
        AudioManager.PlayEFF(gy_.lotusBoomAudio);
    }

    private void GanyuDie(BattleCore bc)
    {
        Retreat();
    }
    
}
