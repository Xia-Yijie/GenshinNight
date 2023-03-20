using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dehya : OperatorCore
{
    [Header("迪希雅的特效")] 
    public GameObject NorAtkAnim;
    public GameObject LeftFistAnim;
    public GameObject LeftFistHit;
    public GameObject RightFistFire;
    public GameObject RightFistAnim;
    public GameObject RightFistHit;
    public GameObject RightFistHit2;
    public GameObject FistAtkRange;
    public DehyaField dehyaField;
    public GameObject FieldSufferAnim;
    public GameObject FieldAtkAnim;
    public GameObject FieldHitAnim;
    public AudioClip NorAtkAudio;
    public AudioClip LeftFistAudio;
    public AudioClip RightFistAudio;
    public AudioClip StartEAudio;
    public AudioClip StartQAudio;
    public AudioClip FieldAtkAudio;
    public AudioClip LeftHitAudio;
    public AudioClip RightHitAudio;
    

    private float[] LeftFist_Multi = {1f, 1.15f, 1.3f, 1.5f, 1.7f, 1.9f, 2.2f};
    private float[] LeftFist_Heal = {120, 140, 160, 180, 210, 240, 270};
    private float Skill1_SufferRate = 0.5f;
    private float[] RightFist_Multi = {1.5f, 1.7f, 2f, 2.3f, 2.6f, 3f, 3.4f};
    private float[] RightFist_PenetrationRate = {0.06f, 0.07f, 0.08f, 0.09f, 0.1f, 0.11f, 0.12f};
    private float PenetrationDuration = 4f;
    private float[] Skill2_DamInc = {0.08f, 0.1f, 0.12f, 0.14f, 0.16f, 0.18f, 0.2f};
    private float[] Skill2_DrainRate = {0.05f, 0.055f, 0.06f, 0.07f, 0.08f, 0.09f, 0.1f};
    private float[] Skill3_AtkDec = {0.1f, 0.09f, 0.08f, 0.07f, 0.06f, 0.05f, 0.04f};
    private float[] Skill3_DefDec = {0.8f, 0.72f, 0.64f, 0.56f, 0.48f, 0.4f, 0.32f};
    private float Skill3_LifeDeadLine = 0.1f;
    private float[] Skill3_LifeRecover = {0.03f, 0.035f, 0.04f, 0.05f, 0.06f, 0.07f, 0.08f};

    private bool isFist = false;
    private float FistAtkInterval = 1.01f;
    private float SufferRate = 0.25f;
    private float CoordinatedAttackRate = 1f;
    private float CoordinatedNorCoolTime = 6f;
    private float FistDamDec = 0.1f;
    private float CoordinatedFistCoolTime = 2.5f;
    private bool isRightFist;   // 刚造成伤害的是否是右拳
    private float fieldAtkRadius = 0.8f;
    
    [HideInInspector] public List<OperatorCore> FieldOperList = new List<OperatorCore>();
    [HideInInspector] public List<EnemyCore> FieldEnemyList = new List<EnemyCore>();
    private ElementTimer FieldSufferAnimTimer;
    private SimpleTimer FieldAtkTimer_Slow;
    private SimpleTimer FieldAtkTimer_Fast;
    

    protected override void Awake_Core()
    {
        base.Awake_Core();
        FieldSufferAnimTimer = new ElementTimer(this, 0.6f);
        FieldAtkTimer_Slow = new SimpleTimer(this, CoordinatedNorCoolTime);
        FieldAtkTimer_Fast = new SimpleTimer(this, CoordinatedFistCoolTime);
    }

    public override void OperInit()
    {
        base.OperInit();
        dehyaField.OpenField();
        DieAction += dehyaField.DehyaDie;
        isFist = false;
        isRightFist = false;
    }

    public override void OnAttack()
    {
        base.OnAttack();
        var norAtk = PoolManager.GetObj(NorAtkAnim);
        norAtk.transform.SetParent(transform);
        norAtk.transform.localPosition = new Vector3(0, 0, 0.4f);
        if (ac_.dirRight) norAtk.transform.eulerAngles = new Vector3(-2.856f, 308.213f, -13.893f);
        else norAtk.transform.eulerAngles = new Vector3(5.376f, 47.928f, 180f);
        DurationRecycleObj recycleObj = new DurationRecycleObj(norAtk, 1f, this, true);
        BuffManager.AddBuff(recycleObj);
        AudioManager.PlayEFF(NorAtkAudio, 0.6f);
    }


    private void Fist_Start(int sta)
    {// 变为拳击形态，以及对应的出拳方式
        SkillActionBuff actionBuff = new SkillActionBuff(this,
            () =>
            {
                anim.SetBool("sp", true);
                isFist = true;
            },
            () =>
            {
                anim.SetBool("sp", false);
                isFist = false;
            });
        BuffManager.AddBuff(actionBuff);

        SkillAnimStaBuff animStaBuff = new SkillAnimStaBuff(this, anim, sta);
        BuffManager.AddBuff(animStaBuff);

        SkillAtkRangeBuff atkRangeBuff = new SkillAtkRangeBuff(this, FistAtkRange);
        BuffManager.AddBuff(atkRangeBuff);

        SkillAtkSpeedBuff atkSpeedBuff = new SkillAtkSpeedBuff(atkSpeedController,
            0, this, FistAtkInterval);
        BuffManager.AddBuff(atkSpeedBuff);
    }

    public override void SkillStart_1()
    {
        base.SkillStart_1();
        Fist_Start(1);
        AudioManager.PlayEFF(StartEAudio);
    }

    public override void SkillAtk_1()
    {
        var leftAnim = PoolManager.GetObj(LeftFistAnim);
        leftAnim.transform.SetParent(transform);
        leftAnim.transform.localPosition = new Vector3(0, 0, 0.5f);
        int dy = ac_.dirRight ? 1 : -1;
        leftAnim.transform.eulerAngles = new Vector3(0, dy * 90, 0);
        DurationRecycleObj recycleObj1 = new DurationRecycleObj(leftAnim, 1f, this, true);
        BuffManager.AddBuff(recycleObj1);
        AudioManager.PlayEFF(LeftHitAudio);
        NorAtkAction?.Invoke(this);

        // 回复自身生命值
        GetHeal(this, LeftFist_Heal[skillLevel[0]]);

        if (skillNum == 0)
        {
            LeftDamage(target);
        }
        else
        {
            foreach (var EC in enemyList) 
                LeftDamage(EC);
        }
    }

    private void LeftDamage(BattleCore tarBC)
    {
        var leftHit = PoolManager.GetObj(LeftFistHit);
        leftHit.transform.SetParent(tarBC.transform);
        leftHit.transform.localPosition = tarBC.animTransform.localPosition + new Vector3(0, 0, 0.3f);
        DurationRecycleObj recycleObj = new DurationRecycleObj(leftHit, 1f, tarBC, true);
        BuffManager.AddBuff(recycleObj);

        float damage = atk_.val * LeftFist_Multi[skillLevel[0]];
        ElementSlot slot = new ElementSlot();
        Battle(tarBC, damage, DamageMode.Physical, slot, false, true);
    }


    public override void SkillStart_2()
    {
        base.SkillStart_2();
        Fist_Start(2);
        AudioManager.PlayEFF(StartEAudio);
        
        // 给自己加元素伤害
        SkillValueBuff valueBuff = new SkillValueBuff(elementDamage, ValueBuffMode.Fixed,
            Skill2_DamInc[skillLevel[1]], this);
        BuffManager.AddBuff(valueBuff);
    }

    public override void SkillAtk_2()
    {
        int dx = ac_.dirRight ? 1 : -1;

        var rightAnim = PoolManager.GetObj(RightFistAnim);
        rightAnim.transform.SetParent(transform);
        rightAnim.transform.localPosition = new Vector3(0, 0, 0.5f);
        rightAnim.transform.eulerAngles = new Vector3(0, dx * (-90), 0);
        DurationRecycleObj recycleObj1 = new DurationRecycleObj(rightAnim, 1f, this, true);
        BuffManager.AddBuff(recycleObj1);

        var rightFire = PoolManager.GetObj(RightFistFire);
        rightFire.transform.SetParent(transform);
        rightFire.transform.localPosition = new Vector3(dx * 0.05f, 0, 0.45f);
        rightFire.transform.localScale = new Vector3(dx * 0.14f, 0.2f, 0.2f);
        DurationRecycleObj recycleObj2 = new DurationRecycleObj(rightFire, 1f, this, true);
        BuffManager.AddBuff(recycleObj2);

        var rightHit = PoolManager.GetObj(RightFistHit);
        rightHit.transform.position = transform.position + new Vector3(dx * 0.5f, 0, 0.5f);
        rightHit.transform.eulerAngles = new Vector3(0, dx * 90, 0);
        DurationRecycleObj recycleObj3 = new DurationRecycleObj(rightHit, 1f);
        BuffManager.AddBuff(recycleObj3);
        AudioManager.PlayEFF(RightHitAudio);
        NorAtkAction?.Invoke(this);

        if (skillNum == 1)
        {
            RightDamage(target);
        }
        else
        {
            foreach (var EC in enemyList)
                RightDamage(EC);
        }
    }

    private void RightDamage(BattleCore tarBC)
    {
        var rightHit = PoolManager.GetObj(RightFistHit2);
        rightHit.transform.SetParent(tarBC.transform);
        rightHit.transform.localPosition = tarBC.animTransform.localPosition + new Vector3(0, 0.1f, 0.35f);
        DurationRecycleObj recycleObj = new DurationRecycleObj(rightHit, 1f, tarBC, true);
        BuffManager.AddBuff(recycleObj);
        
        float damage = atk_.val * RightFist_Multi[skillLevel[1]];
        ElementSlot slot = new ElementSlot(ElementType.Pyro, 1f);
        isRightFist = true;
        Battle(tarBC, damage, DamageMode.Physical, slot, defaultElementTimer, true);
    }

    public override void GetFinalDamage_Attacker(BattleCore tarBC, DamageMode mode, ElementSlot slot, float damage)
    {// 迪希雅对敌人造成伤害后调用的函数，右拳时会触发
        if (!isRightFist) return;
        isRightFist = false;
        int lel = skillLevel[1];
        
        // 右拳造成的伤害会造成破甲效果
        DurationValueBuff valueBuff = new DurationValueBuff(tarBC.def_, ValueBuffMode.Percentage,
            -RightFist_PenetrationRate[lel], PenetrationDuration);
        BuffManager.AddBuff(valueBuff);
        
        // 如果处于二技能状态，还会造成吸血效果
        if (skillNum == 1 && sp_.during)
        {
            float heal = damage * Skill2_DrainRate[lel];
            GetHeal(this, heal);
        }
    }


    public override void SkillStart_3()
    {
        base.SkillStart_3();
        Fist_Start(3);
        int lel = skillLevel[2];
        
        // 技能持续时间内，关闭净焰剑狱
        SkillActionBuff closeBuff = new SkillActionBuff(this,
            (() => dehyaField.CloseField()),
            (() => dehyaField.OpenField()));
        BuffManager.AddBuff(closeBuff);

        // 攻击力和防御力下降效果
        SkillValueBuff atkBuff = new SkillValueBuff(atk_, ValueBuffMode.Percentage,
            -Skill3_AtkDec[lel], this);
        BuffManager.AddBuff(atkBuff);
        SkillValueBuff defBuff = new SkillValueBuff(def_, ValueBuffMode.Percentage,
            -Skill3_DefDec[lel], this);
        BuffManager.AddBuff(defBuff);
        
        // 生命值监控buff
        DehyaSkill3LifeBuff lifeBuff = new DehyaSkill3LifeBuff(this);
        BuffManager.AddBuff(lifeBuff);
        
        AudioManager.PlayEFF(StartQAudio);
    }




    public void OperIntoField(OperatorCore oc)
    {// 有干员进入净焰剑狱
        oc.getFinalDamFuncList.Add(SufferDamage);
    }

    public void OperOutField(OperatorCore oc)
    {// 有干员从净焰剑狱中离开
        oc.getFinalDamFuncList.Remove(SufferDamage);
    }

    public void EnemyIntoField(EnemyCore ec)
    {// 有敌人进入净焰剑狱
        ec.getFinalDamFuncList.Add(FieldEnemyGetDamage);
    }

    public void EnemyOutField(EnemyCore ec)
    {// 有敌人从净焰剑狱中离开
        ec.getFinalDamFuncList.Remove(FieldEnemyGetDamage);
    }

    private float SufferDamage(BattleCore oper, BattleCore attacker, float damage)
    {
        bool isSkill1 = skillNum == 0 && sp_.during;
        float rate = isSkill1 ? Skill1_SufferRate : SufferRate;
        float suffer = damage * rate;

        DamageMode mode = isSkill1 ? DamageMode.Physical : DamageMode.Real;
        ElementSlot slot = new ElementSlot();
        GetDamage(attacker, suffer, mode, slot, false, true);
        
        // 如果可以，加上防御动画
        if (FieldSufferAnimTimer.AttachElement(oper))
        {
            var obj = PoolManager.GetObj(FieldSufferAnim);
            obj.transform.SetParent(oper.transform);
            obj.transform.localPosition = new Vector3(0, 0, 0.5f);
            DurationRecycleObj recycleObj = new DurationRecycleObj(obj, 0.5f, oper, true);
            BuffManager.AddBuff(recycleObj);
        }

        return damage - suffer;     // 迪希雅承担过后，剩下的伤害
    }

    public float FieldEnemyGetDamage(BattleCore enemy, BattleCore attacker, float damage)
    {// 当领域内敌人受到大于0的伤害后调用
        if (eliteLevel < 2) return damage;
        if (damage <= 0 ) return damage;
        if (!isFist && !FieldAtkTimer_Slow.TryToReset()) return damage;
        if (isFist && !FieldAtkTimer_Fast.TryToReset()) return damage;
        
        // 在敌人位置召唤攻击动画
        Vector3 center = enemy.transform.position;
        var atkAnim = PoolManager.GetObj(FieldAtkAnim);
        atkAnim.transform.position = center;
        DurationRecycleObj recycleObj = new DurationRecycleObj(atkAnim, 0.7f);
        BuffManager.AddBuff(recycleObj);
        
        // 对小范围敌人造成伤害
        var tars = InitManager.GetNearByEnemy(center, fieldAtkRadius);
        foreach (var tarEC in tars)
        {
            if (tarEC.isDrone) continue;
            float dam = atk_.val * CoordinatedAttackRate;
            ElementSlot pyroSlot = new ElementSlot(ElementType.Pyro, 1f);
            Battle(tarEC, dam, DamageMode.Physical, pyroSlot, true, true);

            var hit = PoolManager.GetObj(FieldHitAnim);
            hit.transform.SetParent(tarEC.transform);
            hit.transform.localPosition = tarEC.animTransform.localPosition + new Vector3(0, 0.1f, 0.4f);
            DurationRecycleObj recycleObj2 = new DurationRecycleObj(hit, 0.5f, tarEC, true);
            BuffManager.AddBuff(recycleObj2);
        }

        AudioManager.PlayEFF(FieldAtkAudio);
        return damage;
    }

    public IEnumerator Skill3Heal()
    {// 被迫结束技能10秒后，治疗自身
        for (int i = 0; i < 10; i++)
        {
            GetHeal(this, life_.val * Skill3_LifeRecover[skillLevel[2]]);
            yield return new WaitForSeconds(1f);
        }
    }

    public override string GetSkillDescription(int SkillID)
    {
        int lel = skillLevel[SkillID];
        string str = "";
        switch (SkillID)
        {
            case 0:
                return "迪希雅放弃不便的大剑，进入拳击形态，使用左拳攻击敌人\n" +
                       "迪希雅的左拳能对敌人造成" +
                       CT.ChangeToColorfulPercentage(LeftFist_Multi[lel]) + "的" +
                       CT.GetColorfulText("物理伤害", CT.NoneGray) +
                       "，每次使用左拳攻击都将恢复自己" +
                       CT.GetColorfulText(LeftFist_Heal[lel].ToString("f0")) +
                       "生命值\n\n释放该技能，迪希雅还会获得如下效果：\n" +
                       "·「净焰剑狱」的伤害分担比例变为" +
                       CT.ChangeToColorfulPercentage(Skill1_SufferRate) + "\n" +
                       "·「净焰剑狱」分担的伤害不再以" +
                       CT.GetColorfulText("真实伤害", CT.NoneGray) +
                       "的形式作用于自身，而是以" +
                       CT.GetColorfulText("物理伤害", CT.NoneGray) +
                       "的形式（受防御力影响）";
            case 1:
                return "迪希雅放弃不便的大剑，进入拳击形态，使用右拳攻击敌人\n" +
                       "迪希雅的右拳能对敌人造成" +
                       CT.ChangeToColorfulPercentage(RightFist_Multi[lel]) + "的" +
                       CT.GetColorfulText("火元素物理", CT.PyroRed) +
                       "伤害，每次使用右拳攻击都会让敌人的防御力-" +
                       CT.ChangeToColorfulPercentage(RightFist_PenetrationRate[lel]) +
                       "，持续" + CT.GetColorfulText(PenetrationDuration.ToString("f0")) +
                       "秒（可以叠加）\n\n释放该技能，迪希雅还会获得如下效果：\n" +
                       "·元素伤害+" + CT.ChangeToColorfulPercentage(Skill2_DamInc[lel]) +
                       "\n·将造成伤害的" +
                       CT.ChangeToColorfulPercentage(Skill2_DrainRate[lel]) +
                       "转化为自身生命值";
            default:
                return "迪希雅手痒难耐，渴望打架。放弃不便的大剑，进入「炽炎狮子」拳击形态\n" +
                       "在该形态下，迪希雅的攻击会对范围内" +
                       CT.GetColorfulText("所有敌人") +
                       "交替使用" +
                       CT.GetColorfulText("左拳和右拳") +
                       "，造成对应的伤害与特殊效果\n\n" +
                       "但同时，「炽炎狮子」形态下的迪希雅也将：\n·" +
                       CT.GetColorfulText("失去", CT.normalRed) + "「净焰剑狱」领域\n" +
                       "·攻击力-" + CT.ChangeToColorfulPercentage(Skill3_AtkDec[lel], CT.normalRed) +
                       "，防御力-" + CT.ChangeToColorfulPercentage(Skill3_DefDec[lel], CT.normalRed) +
                       "\n·生命值低于" + CT.ChangeToColorfulPercentage(Skill3_LifeDeadLine, CT.normalRed) +
                       "时，会" + CT.GetColorfulText("强制结束", CT.normalRed) +
                       "「炽炎狮子」形态，并在接下来的10秒内，每秒恢复自身最大生命值的" +
                       CT.ChangeToColorfulPercentage(Skill3_LifeRecover[lel]);
        }
    }
    
    public override string GetTalentDescription(int talentID)
    {
        if (talentID == 1)
        {
            bool isSkill1 = skillNum == 0 && sp_.during;
            float rate = isSkill1 ? Skill1_SufferRate : SufferRate;
            return "迪希雅持续召唤「净焰剑狱」领域\n" +
                   "处于该领域中的友方干员受到伤害时，会将受到最终伤害的" +
                   CT.ChangeToColorfulPercentage(rate) + "，以" +
                   CT.GetColorfulText(isSkill1 ? "物理伤害" : "真实伤害", CT.NoneGray) +
                   "的形式转移给迪希雅承担\n\n" +
                   "迪希雅的技能可以让她进入拳击形态，该形态下，迪希雅的攻击间隔" +
                   CT.GetColorfulText("减小") +
                   "，攻击距离" + CT.GetColorfulText("缩短", CT.normalRed) +
                   "，且拥有各种特殊的效果";
        }
        else
        {
            if (eliteLevel < 2) return "";
            return "处于「净焰剑狱」中的敌人在受到不为0的伤害时，迪希雅将进行协同攻击，在小范围内造成" +
                   CT.ChangeToColorfulPercentage(CoordinatedAttackRate) + "的" +
                   CT.GetColorfulText("火元素物理", CT.PyroRed) +
                   "伤害。该效果每" +
                   CT.GetColorfulText(CoordinatedNorCoolTime.ToString("f0"), CT.normalRed) +
                   "秒只能触发一次\n\n若迪希雅处于拳击形态，还会获得以下效果：\n" +
                   "·「净焰剑狱」会让其中的干员获得" +
                   CT.ChangeToColorfulPercentage(FistDamDec) +
                   "的伤害减免\n·「熔铁流狱」的冷却时间降低" +
                   CT.GetColorfulText((CoordinatedNorCoolTime - CoordinatedFistCoolTime).ToString("f1")) +
                   "秒";
        }
    }
}

public class DehyaSkill3LifeBuff : SkillBuffSlot
{// 迪希雅三技能，控制生命底线的buff

    private Dehya dehya;
    
    public DehyaSkill3LifeBuff(Dehya dehya_) : base(dehya_)
    {
        dehya = dehya_;
    }

    public override void BuffUpdate()
    {
        
    }

    public override bool BuffEndCondition()
    {
        if (base.BuffEndCondition()) return true;
        return dehya.life_.Percentage() < 0.1f;     // 低于10%时触发
    }

    public override void BuffEnd()
    {
        base.BuffEnd();
        if (dehya.life_.Percentage() < 0.1f)
            dehya.StartCoroutine(dehya.Skill3Heal());
        dehya.sp_.EndSkillWithFunc();
    }
}