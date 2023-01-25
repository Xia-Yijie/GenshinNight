using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Jean : OperatorCore
{
    [Header("琴的特效")]
    public GameObject norArkAnim;
    public GameObject healAnim;
    public GameObject GaleSword;
    public GameObject GaleWind;
    public GameObject GaleHit;
    public GameObject Skill1_AtkRange_Show;
    public GameObject Skill2_AtkRange;
    public GameObject Skill3_AtkRange;
    public GameObject Skill3_DomainRange;
    public GameObject HealCircle;
    public GameObject PowerUp;
    
    // 已实例化的
    public GameObject lightWalls;
    public AudioSource norAtkAudio;
    public AudioSource galeAudio;
    public AudioSource skill3Audio;
    public AudioSource healAudio;


    private float[] skill1_Multi = {0.8f, 0.9f, 1f, 1.15f, 1.3f, 1.5f, 1.8f};
    private float[] skill2_Multi = {3f, 3.4f, 3.8f, 4.3f, 4.8f, 5.4f, 6f};
    private float[] skill2_Power = new float[7];
    private float[] skill3_BurstDamMulti = {1f, 1.15f, 1.3f, 1.5f, 1.7f, 2f, 2.4f};
    private float[] skill3_BurstHealMulti = {2.8f, 3.3f, 3.8f, 4.4f, 5f, 5.7f, 6.5f};
    private float[] skill3_continueHealMulti = {0.8f, 0.9f, 1f, 1.15f, 1.3f, 1.5f, 1.8f};
    private float skill3_DamReduce = 0.25f;

    private float[] talent1_rate = {0.2f, 0.2f, 0.25f};
    private float talent1_Multi = 1f;
    private float talent2_rate = 0.2f;

    [HideInInspector] public List<BattleCore> aroundOperList = new List<BattleCore>();

    protected override void Awake_Core()
    {
        base.Awake_Core();
        // 初始化风压剑的力度
        skill2_Power[0] = PushAndPullController.mediumForce;
        skill2_Power[1] = PushAndPullController.mediumForce;
        skill2_Power[2] = PushAndPullController.greatForce;
        skill2_Power[3] = PushAndPullController.greatForce;
        skill2_Power[4] = PushAndPullController.greatForce;
        skill2_Power[5] = PushAndPullController.superGreatForce;
        skill2_Power[6] = PushAndPullController.superGreatForce;
    }

    public override void OperInit()
    {
        base.OperInit();
        lightWalls.SetActive(false);
        reduceObjList.Clear();
    }


    protected override void Update_Core()
    {
        base.Update_Core();
        aroundOperList.Sort((x, y) =>
            x.life_.Percentage().CompareTo(y.life_.Percentage()));  // 按生命值百分比升序排列

        if (skillNum == 0 && sp_.CanReleaseSkill() && aroundOperList[0].life_.Percentage() <= 0.5f) 
        {
            sp_.ReleaseSkill();
            SkillStart_1();
        }
    }


    public override void OnAttack()
    {
        base.OnAttack();    // 一次普通的攻击
        norAtkAudio.Play();

        GameObject norAtk = PoolManager.GetObj(norArkAnim);
        int dx = ac_.dirRight ? 1 : -1;
        norAtk.transform.position = transform.position + new Vector3(0.12f * dx, 0, 0.5f);
        Vector3 rol = ac_.dirRight ? new Vector3(30, 75, 0) : new Vector3(0, -90, 0);
        norAtk.transform.eulerAngles = rol;
        DurationRecycleObj recycleObj = new DurationRecycleObj(norAtk, 0.6f);
        BuffManager.AddBuff(recycleObj);

        if (Random.Range(0f, 1f) <= talent1_rate[eliteLevel])
        {
            Heal(aroundOperList[0], atk_.val, true);
            PutHealAnim(aroundOperList[0]);
        }
    }

    public override void SkillStart_1()
    {
        base.SkillStart_1();
        anim.SetInteger("sta", 1);
    }


    public override void SkillAtk_1()
    {
        base.SkillAtk_1();
        anim.SetInteger("sta", 0);

        BattleCore tarBC = aroundOperList[0];
        PutHealAnim(tarBC);

        float count = atk_.val * skill1_Multi[skillLevel[0]];
        Heal(tarBC, count, true);

        if (eliteLevel >= 2) sp_.GetSp(sp_.maxSp * talent2_rate);
    }

    private void PutHealAnim(BattleCore tarBC)
    {
        GameObject heal = PoolManager.GetObj(healAnim);
        heal.transform.SetParent(tarBC.transform);
        heal.transform.localPosition = Vector3.zero;
        DurationRecycleObj recycleObj = new DurationRecycleObj(heal, 1f, tarBC, true);
        BuffManager.AddBuff(recycleObj);
        
        healAudio.Play();
    }

    public override void SkillStart_2()
    {
        base.SkillStart_2();
        anim.SetInteger("sta", 2);
        galeAudio.Play();

        if (defaultFaceRight) ac_.LockRolAndRight();
        else ac_.LockRolAndLeft();
        
        ChangeAtkRange(Skill2_AtkRange);
        if (eliteLevel >= 2) sp_.GetSp(sp_.maxSp * talent2_rate);
    }

    public override void SkillAtk_2()
    {
        base.SkillAtk_2();
        anim.SetInteger("sta", 0);

        GameObject sword = PoolManager.GetObj(GaleSword);
        sword.transform.position = transform.position + new Vector3(0, 0, 0.6f);
        float roly = direction switch
        {
            FourDirection.Right => -90,
            FourDirection.Left => 90,
            FourDirection.UP => 180,
            _ => 0
        };
        sword.transform.eulerAngles = new Vector3(0, roly, 0);
        DurationRecycleObj swordRecycle = new DurationRecycleObj(sword, 1f);
        BuffManager.AddBuff(swordRecycle);

        GameObject wind = PoolManager.GetObj(GaleWind);
        wind.transform.position = transform.position + new Vector3(0, 0, 0.5f);
        roly = direction switch
        {
            FourDirection.Right => 90,
            FourDirection.Left => -90,
            FourDirection.UP => 0,
            _ => 180
        };
        wind.transform.eulerAngles = new Vector3(0, roly, 0);
        DurationRecycleObj windRecycle = new DurationRecycleObj(wind, 1f);
        BuffManager.AddBuff(windRecycle);

        Vector3 dir = direction switch
        {
            FourDirection.Right => new Vector3(1,0,0),
            FourDirection.Left => new Vector3(-1,0,0),
            FourDirection.UP => new Vector3(0,0,1),
            _ => new Vector3(0,0,-1)
        };
        ElementSlot anemoSlot = new ElementSlot(ElementType.Anemo, 2f);
        float dam = atk_.val * skill2_Multi[skillLevel[1]];
        foreach (var tarBC in enemyList)
        {
            Battle(tarBC, dam, DamageMode.Physical, anemoSlot, true, true, true);
            EnemyCore tarEC = (EnemyCore) tarBC;
            tarEC.ppc_.Push(dir, skill2_Power[skillLevel[1]]);

            GameObject hit = PoolManager.GetObj(GaleHit);
            hit.transform.position = tarBC.animTransform.position + new Vector3(0, 0, 0.3f);
            hit.transform.SetParent(tarBC.transform);
            DurationRecycleObj hitRecycle = new DurationRecycleObj(hit, 0.8f, tarBC, true);
            BuffManager.AddBuff(hitRecycle); 
        }
    }

    public void Skill2_End()
    {
        ac_.UnLockRol();
        ChangeAtkRange();
    }


    public override void SkillStart_3()
    {
        base.SkillStart_3();
        ChangeAtkRange(Skill3_AtkRange);
        skill3Audio.Play();

        SkillAnimStaBuff animStaBuff = new SkillAnimStaBuff(this, anim, 3);
        BuffManager.AddBuff(animStaBuff);
        
        if (defaultFaceRight)   // 锁定旋转，默认朝向
            ac_.LockRolAndRight();
        else
            ac_.LockRolAndLeft();
    }

    public void Skill3_Burst()
    {// 爆发治疗的时机
        GameObject burst = PoolManager.GetObj(HealCircle);
        burst.transform.position = transform.position;
        DurationRecycleObj recycleObj = new DurationRecycleObj(burst, 1f);
        BuffManager.AddBuff(recycleObj);

        ElementSlot anemoSlot = new ElementSlot(ElementType.Anemo, 2f);
        float count = atk_.val * skill3_BurstHealMulti[skillLevel[2]];
        float dam = atk_.val * skill3_BurstDamMulti[skillLevel[2]];
        foreach (var tarBC in operatorList)
        {
            Heal(tarBC,count,anemoSlot,true,true,true,true);
        }
        foreach (var tarBC in enemyList)
        {
            Battle(tarBC, dam, DamageMode.Physical, anemoSlot, true, true);
        }
        
        lightWalls.SetActive(true);
        ChangeAtkRange(Skill3_DomainRange);
        StartCoroutine(Skill3_DomainLoop());
    }

    // 范围内伤害减免
    private Dictionary<BattleCore, GameObject> reduceObjList =
        new Dictionary<BattleCore, GameObject>();       // 身上的提升标志
    private int preOperListCount = -1;
    IEnumerator Skill3_DomainLoop()
    {// 实时更新范围内干员的伤害减免

        DieAction += Skill3_End;     // 琴死时调用，清空范围buff
        yield return null;
        while (sp_.during)
        {
            if (operatorList.Count == preOperListCount) yield return null;     // 一般来说，数量没变就是没变
            
            // 扫描当前的operatorList，把多的加进去
            foreach (var bc_ in operatorList)
            {
                if (reduceObjList.ContainsKey(bc_)) continue;

                bc_.getDamFuncList.Add(damReduce_JeanSkill3);

                GameObject powerUp = PoolManager.GetObj(PowerUp);
                powerUp.transform.SetParent(bc_.transform);
                powerUp.transform.localPosition = new Vector3(0, 0, 0.3f);
                reduceObjList.Add(bc_, powerUp);
            }
        
            // 扫描当前damIncBuffList，把多的扔掉
            List<BattleCore> delBCList = new List<BattleCore>();
            foreach (var pp in reduceObjList)
            {
                if(!operatorList.Contains(pp.Key))
                    delBCList.Add(pp.Key);
            }
            foreach (var bc_ in delBCList)
            {
                bc_.getDamFuncList.Remove(damReduce_JeanSkill3);
            
                PoolManager.RecycleObj(reduceObjList[bc_]);
                reduceObjList.Remove(bc_);
            }
            preOperListCount = operatorList.Count;
            yield return null;
        }

        Skill3_End(this);
        DieAction -= Skill3_End;


    }

    private float damReduce_JeanSkill3(float x)
    {
        return x * (1f - skill3_DamReduce);
    }

    public void Skill3_DomainHeal()
    {
        ElementSlot anemoSlot = new ElementSlot(ElementType.Anemo, 2f);
        BattleCore tarBC = aroundOperList[0];
        Heal(tarBC, atk_.val * skill3_continueHealMulti[skillLevel[2]]
            , anemoSlot, true, true, false, true);
        
        PutHealAnim(tarBC);
    }

    public void Skill3_End(BattleCore bc)
    {
        foreach (var pp in reduceObjList)
        {
            pp.Key.getDamFuncList.Remove(damReduce_JeanSkill3);
            PoolManager.RecycleObj(pp.Value);
        }
        reduceObjList.Clear();
        
        ac_.UnLockRol();
        lightWalls.SetActive(false);
        ChangeAtkRange();
        if (eliteLevel >= 2) sp_.GetSp(sp_.maxSp * talent2_rate);
    }

    public override string GetSkillDescription(int SkillID)
    {
        int lel = skillLevel[SkillID];
        switch (SkillID)
        {
            case 0:
                return "为周围一格范围内的一位生命值百分比最低的干员治疗，治疗量为攻击力的"+
                       CT.ChangeToColorfulPercentage(skill1_Multi[lel])+
                       "\n\n当范围内有干员生命值低于50%时，本技能会自动释放";
            case 1:
                return "施展风压剑，对范围内所有敌人造成" +
                       CT.ChangeToColorfulPercentage(skill2_Multi[lel]) + "的" +
                       CT.GetColorfulText("风元素物理", CT.AnemoGreen) +
                       "伤害，并将他们" +
                       CT.GetColorfulText(PushAndPullController.PowerInterpreter(skill2_Power[lel])) +
                       "的推开";
            default:
                return "呼唤风的护佑，为大范围内的干员回复" +
                       CT.ChangeToColorfulPercentage(skill3_BurstHealMulti[lel]) +
                       "攻击力的生命值，并" +
                       CT.GetColorfulText("扩散", CT.AnemoGreen) +
                       "他们附着的元素。同时对敌人造成" +
                       CT.ChangeToColorfulPercentage(skill3_BurstDamMulti[lel]) + "的" +
                       CT.GetColorfulText("风元素物理", CT.AnemoGreen) +
                       "伤害。随后召唤范围较小的" +
                       CT.GetColorfulText("蒲公英领域", CT.AnemoGreen) + ":\n\n" +
                       "·每隔一段时间，琴就会为领域内生命值百分比最低的一位干员回复" +
                       CT.ChangeToColorfulPercentage(skill3_continueHealMulti[lel]) +
                       "攻击力的生命值，并" +
                       CT.GetColorfulText("扩散", CT.AnemoGreen) +
                       "其附着的元素\n·领域内的干员获得" +
                       CT.ChangeToColorfulPercentage(skill3_DamReduce) +
                       "的伤害减免";
        }
    }
    
    public override string GetTalentDescription(int talentID)
    {
        if (talentID == 1)
        {
            return "琴的普通攻击有" +
                   CT.ChangeToColorfulPercentage(talent1_rate[eliteLevel]) +
                   "的几率，为周围一格范围内的一位生命值百分比最低的干员治疗，治疗量为攻击力的" +
                   CT.ChangeToColorfulPercentage(talent1_Multi);
        }
        else
        {
            if (eliteLevel < 2) return "";
            return "释放技能后，立刻回复消耗技力的" +
                   CT.ChangeToColorfulPercentage(talent2_rate);
        }
    }
    
    public override string GetSkillAtkRangeName(int SkillID)
    {
        return SkillID switch
        {
            0 => Skill1_AtkRange_Show.name,
            1 => Skill2_AtkRange.name,
            2 => Skill3_AtkRange.name,
            _ => ""
        };
    }
}
