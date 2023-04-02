using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class Ayaka : OperatorCore
{
    [Header("神里绫华的特效")]
    public GameObject NorAtkAnim;
    public GameObject CryoAtkAnim;
    public GameObject Skill1Anim;
    public GameObject Skill1Hit;
    public GameObject Skill1AtkRange;
    public FrostFlakeSeki frostFlake;
    public GameObject Skill2Hit;
    public GameObject Skill3SwordSmall;
    public GameObject Skill3Sword;
    public GameObject Skill3AtkRange;
    public GameObject Skill3Hit;
    public GameObject Skill3SwordSmall_p;
    public GameObject Skill3Sword_p;
    public GameObject Skill3Hit_p;
    public GameObject Talent1Anim;
    public GameObject talent2Anim;
    public AudioClip norAtkAudio;
    public AudioClip eStartAudio;
    public AudioClip qStartAudio;
    public AudioClip cutAudio;
    public AudioClip qHitAudio;
    public AudioClip talent1Audio;
    public AudioClip skillStart;
    

    private float[] Skill1_Multi = {2f, 2.3f, 2.6f, 3f, 3.4f, 3.8f, 4.2f};
    public float[] Skill3_Multi{ get; private set; } = {1f, 1.15f, 1.3f, 1.5f, 1.7f, 1.9f, 2.1f};
    public float Skill3_AtkCoolTime { get; private set; } = 0.4f;
    public float Skill3_DurTime { get; private set; } = 5f;
    private float[] Skill2_Multi = {1f, 1.15f, 1.3f, 1.5f, 1.7f, 1.9f, 2.1f};

    private float[] talent1_RetreatTimeDec = {10, 15, 20};
    private float talent1_raduis = 0.6f;
    public float talent1_DamInc { get; private set; } = 0.18f;
    public float talent1_AtkSpeedInc { get; private set; } = 30f;
    private float talent1_DurTime = 8f;
    public float talent2_CoolTime{ get; private set; } = 10f;
    public float talent2_DamInc{ get; private set; } = 2.98f;
    public float talent2_DurTime{ get; private set; } = 0.5f;
    private AyakaTalent2 talent2;
    private ElementTimer Skill3Timer;
    private int Skill2Count;
    [HideInInspector] public bool talent1Valid = false;

    protected override void Awake_Core()
    {
        base.Awake_Core();
        Skill3Timer = new ElementTimer(this, 1.9f);
        talent2 = new AyakaTalent2(this);
    }

    protected override void Update_Core()
    {
        base.Update_Core();
        talent2.Update();

        if (skillNum == 0 && sp_.CanReleaseSkill() && enemyList.Count > 0)
        {
            sp_.ReleaseSkill();
            SkillStart_1();
        }
    }

    public override void OperInit()
    {
        base.OperInit();

        frostFlake.gameObject.SetActive(false);
        talent2Anim.SetActive(false);
        talent2.Clear(this);
        DieAction += talent2.Clear;
        
        // 天赋1减少部署冷却
        ValueBuffInner talent1_ReTimeBuff = new ValueBuffInner(
            ValueBuffMode.Fixed, -talent1_RetreatTimeDec[eliteLevel]);
        recoverTime.AddValueBuff(talent1_ReTimeBuff);
    }

    public override void OnAttack()
    {
        base.OnAttack();
        AudioManager.PlayEFF(norAtkAudio);
        
        GameObject nor = PoolManager.GetObj(NorAtkAnim);
        nor.transform.position = transform.position + new Vector3(0, 0, 0.4f);
        Vector3 scale = nor.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (ac_.dirRight ? 1 : -1);
        nor.transform.localScale = scale;
        DurationRecycleObj recycleObj = new DurationRecycleObj(nor, 1f);
        BuffManager.AddBuff(recycleObj);
    }

    public void OnAttackSp()
    {
        NorAtkAction?.Invoke(this);
        AudioManager.PlayEFF(norAtkAudio);
        
        GameObject nor = PoolManager.GetObj(CryoAtkAnim);
        nor.transform.position = transform.position + new Vector3(0, 0, 0.4f);
        Vector3 scale = nor.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (ac_.dirRight ? 1 : -1);
        nor.transform.localScale = scale;
        DurationRecycleObj recycleObj = new DurationRecycleObj(nor, 1f);
        BuffManager.AddBuff(recycleObj);

        ElementSlot cryoSlot = new ElementSlot(ElementType.Cryo, 1f);
        Battle(target, atk_.val, DamageMode.Magic, cryoSlot, defaultElementTimer, true);
    }

    public override void OnStart()
    {
        base.OnStart();
        GameObject obj = PoolManager.GetObj(Talent1Anim);
        obj.transform.position = transform.position;
        DurationRecycleObj recycleObj = new DurationRecycleObj(obj, 2f);
        BuffManager.AddBuff(recycleObj);

        var tars = InitManager.GetNearByEnemy(transform.position, talent1_raduis);
        ElementSlot cryoSlot = new ElementSlot(ElementType.Cryo, 1f);
        foreach (var tarEC in tars)
        {
            Battle(tarEC, 0, DamageMode.Physical, cryoSlot, true);
        }

        if (tars.Count > 0) Talent1Hit();
    }

    private void Talent1Hit()
    {
        AyakaTalent1Buff talent1Buff = new AyakaTalent1Buff(this, talent1_DurTime);
        BuffManager.AddBuff(talent1Buff);

        DurationAtkSpeedBuff atkSpeedBuff = new DurationAtkSpeedBuff(atkSpeedController,
            talent1_AtkSpeedInc, talent1_DurTime);
        BuffManager.AddBuff(atkSpeedBuff);

        if (eliteLevel >= 2)
        {
            talent2.GetTalent();
        }
    }

    public override void SkillStart_1()
    {
        base.SkillStart_1();

        SkillAnimStaBuff staBuff = new SkillAnimStaBuff(this, anim, 1);
        BuffManager.AddBuff(staBuff);
        SkillAtkRangeBuff atkRangeBuff = new SkillAtkRangeBuff(this, Skill1AtkRange);
        BuffManager.AddBuff(atkRangeBuff);
    }

    public override void SkillAtk_1()
    {
        base.SkillAtk_1();

        GameObject obj = PoolManager.GetObj(Skill1Anim);
        obj.transform.position = transform.position;
        DurationRecycleObj recycleObj = new DurationRecycleObj(obj, 1.5f);
        BuffManager.AddBuff(recycleObj);

        foreach (var tarBC in enemyList)
        {
            GameObject hit = PoolManager.GetObj(Skill1Hit);
            hit.transform.SetParent(tarBC.transform);
            hit.transform.localPosition = tarBC.animTransform.localPosition + new Vector3(0, 0.1f, 0.35f);
            DurationRecycleObj recycleObj2 = new DurationRecycleObj(hit, 1, tarBC, true);
            BuffManager.AddBuff(recycleObj2);

            float dam = atk_.val * Skill1_Multi[skillLevel[0]];
            ElementSlot cryoSlot = new ElementSlot(ElementType.Cryo, 2f);
            Battle(tarBC, dam, DamageMode.Magic, cryoSlot, true, true);
        }
    }

    public override void SkillStart_3()
    {
        base.SkillStart_3();

        SkillAnimStaBuff staBuff = new SkillAnimStaBuff(this, anim, 2);
        BuffManager.AddBuff(staBuff);

        SkillLockDirectionBuff lockDirectionBuff = new SkillLockDirectionBuff(this);
        BuffManager.AddBuff(lockDirectionBuff);
    }

    public override void SkillAtk_3()
    {
        base.SkillAtk_3();
        frostFlake.Init(transform.position, direction);
    }

    public override void SkillStart_2()
    {
        base.SkillStart_2();
        AudioManager.PlayEFF(skillStart);
        
        SkillAnimStaBuff staBuff = new SkillAnimStaBuff(this, anim, 3);
        BuffManager.AddBuff(staBuff);

        SkillAtkSpeedBuff atkSpeedBuff = new SkillAtkSpeedBuff(atkSpeedController, 0, this, 1f);
        BuffManager.AddBuff(atkSpeedBuff);

        SkillAtkRangeBuff atkRangeBuff = new SkillAtkRangeBuff(this, Skill3AtkRange);
        BuffManager.AddBuff(atkRangeBuff);
        
        SkillLockDirectionBuff lockDirectionBuff = new SkillLockDirectionBuff(this);
        BuffManager.AddBuff(lockDirectionBuff);

        int dir = direction switch
        {
            FourDirection.UP => 0,
            FourDirection.Down => 1,
            _ => 2,
        };
        anim.SetInteger("direction", dir);

        Skill2Count = 0;
    }


    public override void SkillAtk_2()
    {
        base.SkillAtk_2();
        Skill2Count++;

        Vector3 rol = direction switch
        {
            FourDirection.Right => new Vector3(0, 0, 0),
            FourDirection.UP => new Vector3(0, -90, 0),
            FourDirection.Left => new Vector3(0, 180, 0),
            FourDirection.Down => new Vector3(0, 90, 0),
            _ => new Vector3(0, 0, 0),
        };

        GameObject smallSword =
            talent1Valid ? PoolManager.GetObj(Skill3SwordSmall) : PoolManager.GetObj(Skill3SwordSmall_p);
        smallSword.transform.position = transform.position;
        Vector3 scale = smallSword.transform.localScale;
        scale.x *= ac_.dirRight ? 1 : -1;
        smallSword.transform.localScale = scale;
        DurationRecycleObj recycleObj1 = new DurationRecycleObj(smallSword, 1f);
        BuffManager.AddBuff(recycleObj1);

        GameObject sword = 
            talent1Valid ? PoolManager.GetObj(Skill3Sword) : PoolManager.GetObj(Skill3Sword_p);
        sword.transform.position = transform.position;
        sword.transform.eulerAngles = rol;
        DurationRecycleObj recycleObj2 = new DurationRecycleObj(sword, 1f);
        BuffManager.AddBuff(recycleObj2);

        StartCoroutine(Skill2_Damage());
    }

    IEnumerator Skill2_Damage()
    {
        yield return new WaitForSeconds(0.15f);

        float dam = atk_.val * Skill3_Multi[skillLevel[1]];
        ElementSlot cryoSlot = new ElementSlot(ElementType.Cryo, 1f);
        ElementSlot noneSlot = new ElementSlot();
        for (int i = 0; i < 3; i++)
        {
            foreach (var tarEC in enemyList)
            {
                Battle(tarEC, dam, talent1Valid ? DamageMode.Magic : DamageMode.Physical,
                    talent1Valid ? cryoSlot : noneSlot, Skill3Timer, true);
                
                GameObject hit = 
                    talent1Valid ? PoolManager.GetObj(Skill3Hit) : PoolManager.GetObj(Skill3Hit_p);
                hit.transform.SetParent(tarEC.transform);
                hit.transform.localPosition = tarEC.animTransform.localPosition + new Vector3(0, 0.05f, 0.3f);
                DurationRecycleObj recycleObj = new DurationRecycleObj(hit, 1f, tarEC, true);
                BuffManager.AddBuff(recycleObj);
            }
            yield return new WaitForSeconds(0.15f);
        }
    }

    public void Skill2_End()
    {
        DurationDizzyBuff dizzyBuff = new DurationDizzyBuff(this, Skill2Count);
        BuffManager.AddBuff(dizzyBuff);
    }

    public override void GetFinalDamage_Attacker(BattleCore tarBC, DamageMode mode, ElementSlot slot, float damage)
    {
        base.GetFinalDamage_Attacker(tarBC, mode, slot, damage);
        
        talent2.CauseDamage();
    }


    public override string GetSkillDescription(int SkillID)
    {
        int lel = skillLevel[SkillID];
        string str = "";
        switch (SkillID)
        {
            case 0:
                return "神里绫华唤起盛开的冰之华，对周围的敌人造成" +
                       CT.ChangeToColorfulPercentage(Skill1_Multi[lel]) + "的" +
                       CT.GetColorfulText("冰元素法术", CT.CryoWhite) + "伤害\n\n" +
                       "若原攻击范围内存在敌人，该技能会自动释放";
            case 1:
                return "进入神里流·倾姿态。在该姿态下，神里绫华将进行连续居合重击：攻击范围" +
                       CT.GetColorfulText("扩大") + "，攻击间隔" +
                       CT.GetColorfulText("缩短") +
                       "，每次攻击对范围内的所有敌人造成" + CT.GetColorfulText("3") + "次" +
                       CT.ChangeToColorfulPercentage(Skill2_Multi[lel]) + "的" +
                       CT.GetColorfulText("物理", CT.NoneGray) + "伤害" +
                       "（若处于神里流·霰步增益状态下，造成" +
                       CT.GetColorfulText("冰元素法术", CT.CryoWhite) + "伤害）\n\n" +
                       "连续重击会大幅度消耗体力，神里绫华每进行一次居合重击，就会在结束神里流·倾姿态后被眩晕" +
                       CT.GetColorfulText("1", CT.normalRed) + "秒";
            default:
                return "神里绫华以倾奇之姿汇聚寒霜，放出持续行进的" +
                       CT.GetColorfulText("霜见雪关扉", CT.CryoWhite) + "\n\n" +
                       CT.GetColorfulText("霜见雪关扉", CT.CryoWhite) +
                       "会持续前进，每" +
                       CT.GetColorfulText(Skill3_AtkCoolTime.ToString("f1")) +
                       "秒对其范围内的敌人造成" +
                       CT.ChangeToColorfulPercentage(Skill3_Multi[lel]) + "的" +
                       CT.GetColorfulText("冰元素法术", CT.CryoWhite) +
                       "伤害，持续" +
                       CT.GetColorfulText(Skill3_DurTime.ToString("f0")) + "秒";
            
        }
    }
    
    public override string GetTalentDescription(int talentID)
    {
        if (talentID == 1)
        {
            return "·神里绫华的再部署时间减少" +
                   CT.GetColorfulText(talent1_RetreatTimeDec[eliteLevel].ToString("f0")) +
                   "秒\n·部署后，立刻对周围敌人施加一次" +
                   CT.GetColorfulText("冰元素附着", CT.CryoWhite) +
                   "\n\n如果此次冰元素附着命中敌人，神里绫华会额外获得以下效果，持续" +
                   CT.GetColorfulText(talent1_DurTime.ToString("f0")) + "秒：\n" +
                   "·普通攻击和居合重击造成" + CT.GetColorfulText("冰元素法术", CT.CryoWhite) + "伤害\n" +
                   "·元素伤害+" + CT.ChangeToColorfulPercentage(talent1_DamInc) + "，" +
                   "攻击速度+" + CT.GetColorfulText(talent1_AtkSpeedInc.ToString("f0"));

        }
        else
        {
            if (eliteLevel < 2) return "";
            return "如果神里绫华在" + CT.GetColorfulText(talent2_CoolTime.ToString("f0")) +
                   "秒内没有造成任何伤害，则会获得「" + CT.GetColorfulText("薄冰舞踏", CT.CryoWhite) +
                   "」\n·使神里绫华的元素伤害+" + CT.ChangeToColorfulPercentage(talent2_DamInc) +
                   "，并在下一次造成伤害的" + CT.GetColorfulText(talent2_DurTime.ToString("f1")) +
                   "秒后清除\n·若神里流·霰步命中敌人，立刻获得「" +
                   CT.GetColorfulText("薄冰舞踏", CT.CryoWhite) + "」效果";
        }
    }
    
    public override string GetSkillAtkRangeName(int SkillID)
    {
        return SkillID switch
        {
            0 => Skill1AtkRange.name,
            1 => Skill3AtkRange.name,
            _ => ""
        };
    }
}

public class SkillLockDirectionBuff : SkillBuffSlot
{
    private OperatorCore oc_;
    
    public SkillLockDirectionBuff(OperatorCore oc) : base(oc)
    {
        oc_ = oc;
    }
    
    public override void BuffStart()
    {
        base.BuffStart();
        
        if (oc_.defaultFaceRight)
            oc_.ac_.LockRolAndRight();
        else
            oc_.ac_.LockRolAndLeft();
    }

    public override void BuffUpdate() { }

    public override void BuffEnd()
    {
        base.BuffEnd();
        oc_.ac_.UnLockRol();
    }
    
}

public class AyakaTalent1Buff : BattleCoreDurationBuff
{
    private Ayaka ayaka;
    private ValueBuffInner damBuff;

    public AyakaTalent1Buff(Ayaka ayaka_, float t) : base(ayaka_, t)
    {
        ayaka = ayaka_;
    }

    public override void BuffStart()
    {
        ayaka.anim.SetBool("sp", true);
        damBuff = new ValueBuffInner(ValueBuffMode.Fixed, ayaka.talent1_DamInc);
        ayaka.elementDamage.AddValueBuff(damBuff);
        ayaka.talent1Valid = true;
    }

    public override void BuffEnd()
    {
        ayaka.anim.SetBool("sp", false);
        ayaka.elementDamage.DelValueBuff(damBuff);
        ayaka.talent1Valid = false;
    }
}

public class AyakaTalent2
{
    private Ayaka ayaka;
    private float coolTime;
    private float durTime;
    private bool valid;
    private bool causeDamage;
    private ValueBuffInner damIncBuff;
    
    
    public AyakaTalent2(Ayaka ayaka_)
    {
        ayaka = ayaka_;
        coolTime = ayaka.talent2_CoolTime;
        durTime = 0;
        valid = false;
        causeDamage = false;
    }

    public void Update()
    {
        if (ayaka.eliteLevel < 2) return;
        if (!valid)
        {
            coolTime -= Time.deltaTime;
            if (coolTime <= 0)
            {
                GetTalent();
            }
        }
        else if(causeDamage)
        {
            durTime -= Time.deltaTime;
            if (durTime <= 0)
            {
                DelTalent();
            }
        }
    }

    public void GetTalent()
    {
        if (ayaka.eliteLevel < 2) return;
        valid = true;
        coolTime = ayaka.talent2_CoolTime;
        durTime = ayaka.talent2_DurTime;
        damIncBuff = new ValueBuffInner(ValueBuffMode.Fixed, ayaka.talent2_DamInc);
        ayaka.elementDamage.AddValueBuff(damIncBuff);
        ayaka.talent2Anim.SetActive(true);
    }

    public void CauseDamage()
    {
        if (ayaka.eliteLevel < 2) return;
        if (valid)
        {
            causeDamage = true;
        }
        else
        {
            coolTime = ayaka.talent2_CoolTime;
        }
    }

    public void DelTalent()
    {
        valid = false;
        causeDamage = false;
        ayaka.elementDamage.DelValueBuff(damIncBuff);
        damIncBuff = null;
        coolTime = ayaka.talent2_CoolTime;
        durTime = 0;
        ayaka.talent2Anim.SetActive(false);
    }

    public void Clear(BattleCore bc)
    {
        DelTalent();
    }
    
}