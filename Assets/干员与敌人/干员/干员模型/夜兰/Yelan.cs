using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yelan : OperatorCore
{
    [Header("夜兰的特效")]
    public GameObject NorArkAnim;
    public GameObject NorHitAnim;
    public GameObject Skill1_AtkAnim;
    public GameObject Skill1_Charge;
    public GameObject Skill1_HydroBoom;
    public GameObject Skill1_HitAnim;
    public GameObject Skill2_HitAnim;
    public GameObject Skill3_AtkAnim;
    public GameObject Skill3_HitAnim;
    public GameObject Skill3_UnderCircle;
    
    // 已实例化物体
    public lifeLine[] LifeLines = new lifeLine[3];
    public GameObject[] manyLines_self = new GameObject[5];
    public GameObject underLight;
    public GameObject DiceInHand;
    public exquisiteThrow Dice;

    public AudioClip norAtkAudio;
    public AudioClip skill1OutAudio;
    public AudioClip hydroBoomAudio;
    public AudioClip eStartAudio;
    public AudioClip eHitAudio;
    public AudioClip qStartAudio;
    public AudioClip qAtkStartAudio;
    public AudioClip qAtkHitAudio;

    private float[] Skill1_Multi = {1.8f, 2.1f, 2.4f, 2.8f, 3.2f, 3.7f, 4.2f};
    private float[] Skill2_Multi = {0.6f, 0.7f, 0.85f, 1.05f, 1.3f, 1.65f, 2f};
    private float[] Skill2_Duration = {15f, 16f, 17f, 18f, 19f, 21f, 23f, 25f};
    private float Skill2_SlowRate = 0.8f;
    private float Skill2_SlowDuration = 1f;
    private float[] Skill3_Multi = {0.35f, 0.41f, 0.48f, 0.56f, 0.64f, 0.73f, 0.84f};
    private float[] Skill3_Duration = {13f, 13f, 14f, 15f, 16f, 17f, 18f};
    private float Skill3_DamIncInit = 0.01f;
    private float Skill3_DamIncPerSecend = 0.035f;
    
    private float talent1_radius = 1.1f;
    private float[] talent1_LifeIncRate = {0, 0.06f, 0.12f, 0.18f, 0.25f};
    public float talent2_IncRate { get; private set; } = 0.12f;
    private ValueBuffInner talent1LifeIncInner = new ValueBuffInner(ValueBuffMode.Percentage, 0);

    public override void OperInit()
    {
        base.OperInit();
        for (int i = 0; i < 3; i++) LifeLines[i].gameObject.SetActive(false);
        for (int i = 0; i < 5; i++) manyLines_self[i].SetActive(false);
        underLight.SetActive(false);
        DiceInHand.SetActive(false);

        List<ElementType> cntList = new List<ElementType>();
        foreach (var OC in InitManager.operList)
        {
            if (!cntList.Contains(OC.od_.elementType))
                cntList.Add(OC.od_.elementType);
        }

        talent1LifeIncInner.v = talent1_LifeIncRate[Math.Min(cntList.Count, 4)];
        life_.AddValueBuff(talent1LifeIncInner);

        DieAction += core =>
        {
            talent1LifeIncInner.v = 0;
            life_.DelValueBuff(talent1LifeIncInner);
        };
    }

    protected override int operCmp(BattleCore a, BattleCore b)
    {
        Vector3 pos = transform.position;
        Vector3 apos = a.transform.position;
        Vector3 bpos = b.transform.position;
        if (BaseFunc.preEqual(pos, apos)) return 1;         // 如果是夜兰本体，排到最后
        if (BaseFunc.preEqual(pos, bpos)) return -1;
        
        bool afront = BaseFunc.isFront(pos, apos, directionVector);
        bool bfront = BaseFunc.isFront(pos, bpos, directionVector);
        if (afront && !bfront) return -1;           // 首先在正前方优先
        if (!afront && bfront) return 1;
        
        float adis = BaseFunc.xz_Distance(pos, apos);
        float bdis = BaseFunc.xz_Distance(pos, bpos);
        if (Math.Abs(adis - bdis) < 1e-3) return -a.tarPriority.CompareTo(b.tarPriority);
        return adis.CompareTo(bdis);    // 距离优先，距离相同则按优先级来
    }


    public override void OnAttack()
    {
        if (skillNum == 0 && sp_.CanReleaseSkill())
        {
            Archery(Skill1_Multi[skillLevel[0]], Skill1_AtkAnim, Skill1Attack);
            sp_.ReleaseSkill();
            AudioManager.PlayEFF(skill1OutAudio);
        }
        else
        {
            sp_.GetSp_Atk();
            Archery(1f, NorArkAnim, norAttack);
            AudioManager.PlayEFF(norAtkAudio, 0.8f);
        }
    }
    
    private void Archery(float multi, GameObject proArrow, Action<float, BattleCore, parabola, bool> endAttack)
    {// 射一支箭出去，攻击倍率为multi
        
        var arrow = PoolManager.GetObj(proArrow);
        parabola par = arrow.GetComponent<parabola>();
        
        Vector3 pos = transform.position;
        pos += new Vector3(ac_.dirRight ? 0.45f : -0.45f, 0.5f, 0.5f);
        par.Init(pos, this, target, 12f, endAttack, multi);
    }

    private void norAttack(float multi, BattleCore tarBC, parabola par, bool tarIsNull)
    {
        if (tarIsNull) return;
        
        GameObject hitAnim = PoolManager.GetObj(NorHitAnim);
        hitAnim.transform.parent = tarBC.transform;
        Vector3 pos = tarBC.animTransform.localPosition + new Vector3(0, 0, 0.3f);
        hitAnim.transform.localPosition = pos;
        DurationRecycleObj recycleObj = new DurationRecycleObj(hitAnim, 1f, tarBC, true);
        BuffManager.AddBuff(recycleObj);
        
        Battle(tarBC, atk_.val, DamageMode.Physical);
    }

    private void Skill1Attack(float multi, BattleCore tarBC, parabola par, bool tarIsNull)
    {
        Vector3 center = par.transform.position;
        center.y = 0;

        GameObject boom = PoolManager.GetObj(Skill1_HydroBoom);
        boom.transform.position = center;
        DurationRecycleObj recycleObj1 = new DurationRecycleObj(boom, 0.7f);
        BuffManager.AddBuff(recycleObj1);

        var tars = InitManager.GetNearByEnemy(center, talent1_radius);
        float dam = atk_.val * Skill1_Multi[skillLevel[0]];
        if (eliteLevel >= 2) dam += life_.life * talent2_IncRate;
        ElementSlot hydroSlot = new ElementSlot(ElementType.Hydro, 1f);

        foreach (var tarEC in tars)
        {
            GameObject hitAnim = PoolManager.GetObj(Skill1_HitAnim);
            hitAnim.transform.parent = tarEC.transform;
            Vector3 pos = tarEC.animTransform.localPosition + new Vector3(0, 0.05f, 0.35f);
            hitAnim.transform.localPosition = pos;
            DurationRecycleObj recycleObj2 = new DurationRecycleObj(hitAnim, 1f, tarEC, true);
            BuffManager.AddBuff(recycleObj2);

            Battle(tarEC, dam, DamageMode.Physical, hydroSlot, true, true);
        }
        
        AudioManager.PlayEFF(hydroBoomAudio);
    }

    public override void SkillStart_2()
    {
        anim.SetInteger("sta", 1);
        if(defaultFaceRight) ac_.LockRolAndRight();
        else ac_.LockRolAndLeft();
    }

    public override void SkillAtk_2()
    {
        base.SkillStart_2();
        AudioManager.PlayEFF(eStartAudio);
        anim.SetInteger("sta", 0);
        ac_.UnLockRol();

        Vector3 pos = BaseFunc.x0z(BaseFunc.FixCoordinate(transform.position));
        float duration = Skill2_Duration[skillLevel[1]];
        LifeLines[0].Init(pos, 1, direction, duration);
        LifeLines[1].Init(pos, 2, direction, duration);
        LifeLines[2].Init(pos, 3, direction, duration);
    }

    public void LifeLineAtk(EnemyCore tarEC)
    {
        GameObject hitAnim = PoolManager.GetObj(Skill2_HitAnim);
        hitAnim.transform.SetParent(tarEC.transform);
        Vector3 pos = tarEC.animTransform.localPosition + new Vector3(0, 0.05f, 0.35f);
        hitAnim.transform.localPosition = pos;
        DurationRecycleObj recycleObj = new DurationRecycleObj(hitAnim, 1f, tarEC, true);
        BuffManager.AddBuff(recycleObj);

        float dam = atk_.val * Skill2_Multi[skillLevel[1]];
        if (eliteLevel >= 2) dam += life_.life * talent2_IncRate;
        ElementSlot hydroSlot = new ElementSlot(ElementType.Hydro, 1f);
        Battle(tarEC, dam, DamageMode.Physical, hydroSlot, defaultElementTimer, true);

        EnemySlowDurationBuff slowBuff = new EnemySlowDurationBuff(
            tarEC, 1f - Skill2_SlowRate, Skill2_SlowDuration);
        BuffManager.AddBuff(slowBuff);

        AudioManager.PlayEFF(eHitAudio, 0.6f);
    }

    public override void SkillStart_3()
    {
        base.SkillStart_3();
        AudioManager.PlayEFF(qStartAudio);
        anim.SetInteger("sta", 2);
        underLight.SetActive(true);
        DiceInHand.SetActive(true);
        Vector3 scale = new Vector3(defaultFaceRight ? 1 : -1, 1, 1);
        DiceInHand.transform.localScale = scale;

        if (defaultFaceRight) ac_.LockRolAndRight();
        else ac_.LockRolAndLeft();

        if (Dice.gameObject.activeSelf) Dice.Retreat(null);
    }

    public IEnumerator ShowLines()
    {
        for (int i = 0; i < 5; i++)
        {
            manyLines_self[i].SetActive(true);
            yield return new WaitForSeconds(0.05f);
        }
    }
    
    public IEnumerator DisableLines()
    {
        for (int i = 0; i < 5; i++)
        {
            manyLines_self[i].SetActive(false);
            yield return new WaitForSeconds(0.08f);
        }
    }

    public override void SkillAtk_3()
    {
        anim.SetInteger("sta", 0);
        int lel = skillLevel[2];
        Dice.Init((OperatorCore) operatorList[0], Skill3_Duration[lel], Skill3_Multi[lel],
            Skill3_DamIncInit, Skill3_DamIncPerSecend,
            eliteLevel >= 2 ? talent2_IncRate : 0);
    }


    public override string GetSkillDescription(int SkillID)
    {
        int lel = skillLevel[SkillID];
        switch (SkillID)
        {
            case 0:
                return "夜兰的下一次攻击将发射“破局矢”，造成" +
                       CT.ChangeToColorfulPercentage(Skill1_Multi[lel]) +
                       "的范围" +
                       CT.GetColorfulText("水元素物理", CT.HydroBlue) +
                       "伤害";
            case 1:
                return "夜兰在正前方部署3道「" +
                       CT.GetColorfulText("络命丝", CT.HydroBlue) +
                       "」，持续" +
                       CT.GetColorfulText(Skill2_Duration[lel].ToString("f0")) +
                       "秒\n\n敌人在经过一道" +
                       CT.GetColorfulText("络命丝", CT.HydroBlue) +
                       "时，会受到一次" +
                       CT.ChangeToColorfulPercentage(Skill2_Multi[lel]) +
                       "的" + CT.GetColorfulText("水元素物理", CT.HydroBlue) +
                       "伤害，并被减速" +
                       CT.ChangeToColorfulPercentage(Skill2_SlowRate) +
                       "，持续" +
                       CT.GetColorfulText(Skill2_SlowDuration.ToString("f0")) +
                       "秒";
            default:
                return "夜兰选取范围内最近（优先选取正前方）的一名友方干员，为其凝聚「" +
                       CT.GetColorfulText("玄掷玲珑", CT.HydroBlue) +
                       "」协助战斗，" + CT.GetColorfulText("玄掷玲珑", CT.HydroBlue) +
                       "将持续" +
                       CT.GetColorfulText(Skill3_Duration[lel].ToString("f0")) +
                       "秒：\n\n·当附属干员进行普通攻击时，" +
                       CT.GetColorfulText("玄掷玲珑", CT.HydroBlue) +
                       "将进行协同攻击，造成3次" +
                       CT.ChangeToColorfulPercentage(Skill3_Multi[lel]) + "的" +
                       CT.GetColorfulText("水元素物理", CT.HydroBlue) + "伤害，该效果有" +
                       CT.GetColorfulText("2", CT.normalRed) + "秒冷却\n\n" +
                       "·为附属干员提供" +
                       CT.ChangeToColorfulPercentage(Skill3_DamIncInit) +
                       "的元素伤害加成，且每过一秒，该效果还将提高" +
                       CT.ChangeToColorfulPercentage(Skill3_DamIncPerSecend, 1) + "\n\n·" +
                       CT.GetColorfulText("玄掷玲珑", CT.HydroBlue) +
                       "将在夜兰退场时消失";
        }
    }

    public override string GetTalentDescription(int talentID)
    {
        if (talentID == 1)
        {
            string s = "";
            for (int i = 1; i < 5; i++)
            {
                s += CT.ChangeToColorfulPercentage(talent1_LifeIncRate[i]);
                if (i < 4) s += "/";
            }

            return "部署时根据场上干员的元素类型，每有1/2/3/4种不同的类型，夜兰获得" +
                   s + "的生命值上限提升";
        }
        else
        {
            if (eliteLevel < 2) return "";
            return "夜兰造成的所有" +
                   CT.GetColorfulText("水元素", CT.HydroBlue) +
                   "伤害得到提升，提升值相当于夜兰当前生命值的" +
                   CT.ChangeToColorfulPercentage(talent2_IncRate);
        }
    }
}
