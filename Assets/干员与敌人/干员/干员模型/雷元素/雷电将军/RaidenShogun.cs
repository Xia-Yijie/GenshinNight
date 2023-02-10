using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaidenShogun : OperatorCore
{
    [Header("雷电将军的特效")] 
    public GameObject GetWP_Star;
    public GameObject Skill1_CutDown;
    public GameObject skill2_eyeAnim;
    public GameObject skill2_atkRange;
    public GameObject skill2_eyeBurstHit;
    public thunderEye attachEye;
    public GameObject ReleaseLighting;
    public GameObject skill3_atkRange;
    public GameObject skill3_atkCut;
    public GameObject WpToSpAnim;
    
    public AudioClip badaoAudio;
    public AudioClip kanAudio;
    public AudioClip audioEye;
    public AudioClip norAtkAudio;
    public AudioClip spAtkAudio;
    
    private const float talent1_RaidenMul = 0.5f;       // 雷电将军释放技能得到的愿力
    private const float talent1_OtherMul = 0.1f;        // 其他干员释放技能将军得到的愿力
    private const float talent2_damInc = 0.003000001f;  // 每1%充能效率，提高伤害
    private const float talent2_wpInc = 0.02f;          // 每1%充能效率，提高愿力回复


    private float[] skill1_Multi = {2, 2.2f, 2.4f, 2.6f, 2.8f, 3, 3.2f};
    private float[] skill2_ReleaseMulti = {1.6f, 1.8f, 2f, 2.2f, 2.4f, 2.7f, 3f};
    [HideInInspector] public float[] skill2_CoorMulti = {1.4f, 1.5f, 1.6f, 1.8f, 2f, 2.2f, 2.5f};
    private float[] skill2_eyeDuration = {20, 21, 22, 23, 24, 25, 25};
    private float[] skill3_burstMulti = {4, 4.4f, 4.8f, 5.2f, 5.8f, 6.5f, 7.2f};
    private float[] skill3_atkMulti = {1.2f, 1.3f, 1.4f, 1.6f, 1.8f, 2f, 2.2f};
    private float[] skill3_wpRecover = {0.9f, 1, 1.1f, 1.2f, 1.3f, 1.4f, 1.5f};

    private float wp = 0;           // 愿力
    private int maxWP = 100;
    private float lastRecharge = 1;     // 上一帧的充能效率
    private ValueBuffInner talent2BuffInner;
    private int lastSkillNum;           // 上一帧选择的技能
    private float skill3WpCost;         // 3技能消耗的愿力


    public override void OperInit()
    {
        base.OperInit();
        SPController.releaseAction += ReleaseActionBack;
        
        talent2BuffInner = new ValueBuffInner(ValueBuffMode.Fixed, 0);
        elementDamage.AddValueBuff(talent2BuffInner);
        lastRecharge = 1;
        
        DieAction += DieCallBack;
        
        attachEye.gameObject.SetActive(false);
    }

    protected override void Start_Core()
    {
        base.Start_Core();
        SPController.releaseAction -= ReleaseActionBack;
        DieAction -= DieCallBack;
    }

    protected override void Update_Core()
    {
        base.Update_Core();
        
        // 天赋1
        if (lastSkillNum != skillNum)
        {
            lastSkillNum = skillNum;
            float wpcost = Math.Min(wp, sp_.maxSp);
            if (wpcost == 0) return;
            wp -= wpcost;
            sp_.sp += wpcost;
            
            GameObject wpTosp = PoolManager.GetObj(WpToSpAnim);
            wpTosp.transform.SetParent(transform);
            wpTosp.transform.localPosition = new Vector3(0, 0, 0.4f);
            DurationRecycleObj recycleObj = new DurationRecycleObj(wpTosp, 0.7f, this, true);
            BuffManager.AddBuff(recycleObj);
        } 

        if (eliteLevel < 2) return;     // 天赋2
        if (Math.Abs(lastRecharge - sp_.spRecharge.val) < 1e-4) return;
        lastRecharge = sp_.spRecharge.val;
        talent2BuffInner.v = (lastRecharge - 1f) * 100 * talent2_damInc;
        elementDamage.RefreshValue();
    }


    public override void OnAttack()
    {
        Battle(target, atk_.val);
        attachEye.RaidenAttack(target);
        AudioManager.PlayEFF(norAtkAudio);
        
        if (skillNum == 0)
        {
            sp_.GetSp_Atk();
            if (sp_.CanReleaseSkill()) anim.SetBool("sp", true);
        }
    }

    public override void SkillAtk_1()
    {
        float dam = atk_.val * skill1_Multi[skillLevel[0]];
        anim.SetBool("sp", false);
        sp_.ReleaseSkill();
        AudioManager.PlayEFF(spAtkAudio);

        GameObject cutDown = PoolManager.GetObj(Skill1_CutDown);
        float dirX = ac_.dirRight ? 1 : -1;
        Vector3 pos = transform.position + new Vector3(dirX * 0.6f, 0, 1f);
        cutDown.transform.position = pos;
        cutDown.transform.localScale = new Vector3(dirX * 0.25f, 0.22f, 0.25f);
        DurationRecycleObj recycleObj = new DurationRecycleObj(cutDown, 0.25f);
        BuffManager.AddBuff(recycleObj);
        
        Battle(target, dam, true);
        attachEye.RaidenAttack(target);
        
        if (targetIsKilled)
        {
            sp_.sp = sp_.maxSp;
            anim.SetBool("sp", true);
        }
    }


    public override void SkillStart_2()
    {
        base.SkillStart_2();
        SkillAnimStaBuff animStaBuff = new SkillAnimStaBuff(this, anim, 1);
        BuffManager.AddBuff(animStaBuff);
        
        SkillAtkRangeBuff atkRangeBuff = new SkillAtkRangeBuff(this, skill2_atkRange);
        BuffManager.AddBuff(atkRangeBuff);
        
        AudioManager.PlayEFF(audioEye);
    }

    public override void SkillAtk_2()
    {
        Vector3 pos = transform.position;
        Vector3 rol = Vector3.zero;
        Vector3 scale = Vector3.one;
        switch (direction)
        {
            case FourDirection.Right:
                pos += new Vector3(1.5f, 0, 0.6f);
                rol = new Vector3(60, 0, 0);
                scale = new Vector3(0.48f, 0.48f, 0.48f);
                break;
            case FourDirection.Left:
                pos += new Vector3(-1.5f, 0, 0.6f);
                rol = new Vector3(60, 0, 0);
                scale = new Vector3(-0.48f, 0.48f, 0.48f);
                break;
            case FourDirection.UP:
                pos += new Vector3(0, 0, 1.5f);
                rol = new Vector3(60, 0, -90);
                scale = new Vector3(-0.48f, 0.48f, 0.48f);
                break;
            case FourDirection.Down:
                pos += new Vector3(0, 0, -1.6f);
                rol = new Vector3(60, 0, 90);
                scale = new Vector3(-0.48f, 0.48f, 0.48f);
                break;
        }
        
        GameObject eye = PoolManager.GetObj(skill2_eyeAnim);
        Transform eeeye = eye.transform.Find("雷眼");
        Transform elele = eye.transform.Find("环绕闪电");
        eye.transform.position = pos;
        eye.transform.eulerAngles = rol;
        eye.transform.localScale = scale;
        eeeye.eulerAngles = elele.eulerAngles = new Vector3(60, 0, 0);
        
        DurationRecycleObj recycleObj = new DurationRecycleObj(eye, 1f);
        BuffManager.AddBuff(recycleObj);

        StartCoroutine(Skill2_ComeOn());
    }

    private IEnumerator Skill2_ComeOn()
    {
        yield return new WaitForSeconds(0.9f);

        float dam = atk_.val * skill2_ReleaseMulti[skillLevel[1]];
        ElementSlot slot = new ElementSlot(ElementType.Electro, 1f);
        foreach (var enemy in enemyList)
        {
            GameObject hit = PoolManager.GetObj(skill2_eyeBurstHit);
            hit.transform.SetParent(enemy.transform);
            hit.transform.localPosition = new Vector3(0, 0, 0.8f);
            DurationRecycleObj recycleObj = new DurationRecycleObj(hit, 0.667f, enemy, true);
            BuffManager.AddBuff(recycleObj);

            Battle(enemy, dam, DamageMode.Magic, slot, true, true);
        }

        yield return new WaitForSeconds(0.1f);
        
        attachEye.Init(skill2_eyeDuration[skillLevel[1]]);
    }


    public override void SkillStart_3()
    {
        base.SkillStart_3();
        skill3WpCost = wp > 100 ? 100 : wp;     // 3技能消耗所有愿力
        wp = 0;
        
        SkillAnimStaBuff animStaBuff = new SkillAnimStaBuff(this, anim, 2);
        BuffManager.AddBuff(animStaBuff);

        DurationAtkRangeBuff atkRangeBuff = new DurationAtkRangeBuff(skill3_atkRange, this, 2f);
        BuffManager.AddBuff(atkRangeBuff);

        GameObject releaseLighting = PoolManager.GetObj(ReleaseLighting);
        releaseLighting.transform.SetParent(transform);
        releaseLighting.transform.localPosition = new Vector3(0, 0.281f, 0.486f);
        DurationRecycleObj recycleObj = new DurationRecycleObj(releaseLighting, 2f, this, true);
        BuffManager.AddBuff(recycleObj);
        
        AudioManager.PlayEFF(badaoAudio);
    }

    public void Skill3_Burst()
    {
        float dam = atk_.val * skill3_burstMulti[skillLevel[2]];
        dam *= (1 + skill3WpCost / 100f);       // 消耗愿力提高伤害
        ElementSlot slot = new ElementSlot(ElementType.Electro, 1f);
        foreach (var enemy in enemyList)
        {
            Battle(enemy, dam, DamageMode.Magic, slot, true, true, true);
        }
        attachEye.RaidenAttack(target);
    }

    public void Skill3_AtkAudio()
    {
        AudioManager.PlayEFF(kanAudio);
    }
    
    public override void SkillAtk_3()
    {
        int lel = skillLevel[2];
        float dam = atk_.val * skill3_atkMulti[lel];
        dam *= (1 + skill3WpCost / 100f);       // 消耗愿力提高伤害
        ElementSlot slot = new ElementSlot(ElementType.Electro, 1f);

        Battle(target, dam, DamageMode.Magic, slot, defaultElementTimer, true);
        attachEye.RaidenAttack(target);

        // 回愿力
        wp += skill3_wpRecover[lel] + (talent2_wpInc * (sp_.spRecharge.val - 1) * 100);
        
        GameObject cut = PoolManager.GetObj(skill3_atkCut);
        cut.transform.SetParent(transform);
        float dirX = ac_.dirRight ? 1 : -1;
        Vector3 scale = cut.transform.localScale;
        cut.transform.localPosition = new Vector3(dirX * 0.7f, 0, 0.6f);
        cut.transform.localScale = new Vector3(dirX * Mathf.Abs(scale.x), scale.y, scale.z);
        DurationRecycleObj recycleObj = new DurationRecycleObj(cut, 0.3f, this, true);
        BuffManager.AddBuff(recycleObj);
    }


    private void ReleaseActionBack(SPController spController)
    {// 加给静态函数的回调，监控所有干员释放技能
        if (!spController.bc_.CompareTag("operator")) return;

        float wpInc = 0;
        if (spController.bc_ == this)
        {
            wpInc = skillNum switch
            {
                0 => talent1_RaidenMul * spController.sp * 2,
                1 => talent1_RaidenMul * spController.sp,
                _ => 0
            };
        }
        else wpInc = talent1_OtherMul * spController.sp;
        wp = wp + wpInc > maxWP ? maxWP : wp + wpInc;

        if (spController.bc_ == this) return;   // 自己释放技能不加特效
        GameObject star = PoolManager.GetObj(GetWP_Star);
        star.transform.SetParent(transform);
        star.transform.localPosition = new Vector3(0, 0, 0.5f);
        DurationRecycleObj recycleObj = new DurationRecycleObj(star, 0.5f, this, true);
        BuffManager.AddBuff(recycleObj);
    }

    private void DieCallBack(BattleCore bc)     // 雷电将军的死亡回调函数
    {
        // 去除监听所有技能释放
        SPController.releaseAction -= ReleaseActionBack;
        // 损失一半的愿力
        wp /= 2f;
        // 去除天赋2伤害增加buff
        elementDamage.DelValueBuff(talent2BuffInner);
    }

    public override string GetSkillDescription(int SkillID)
    {
        int lel = skillLevel[SkillID];
        switch (SkillID)
        {
            case 0:
                return "下一次攻击将造成" +
                       CT.ChangeToColorfulPercentage(skill1_Multi[lel]) +
                       "攻击力的物理伤害，如果将目标击倒则立即恢复所有技力\n\n本技能为雷电将军提供的愿力为通常的" +
                       CT.GetColorfulText("2", CT.normalBlue) +
                       "倍";
            case 1:
                return "雷电将军展开净土的一角，对前方大范围的敌人造成" +
                       CT.ChangeToColorfulPercentage(skill2_ReleaseMulti[lel]) +
                       "的" +
                       CT.GetColorfulText("雷元素魔法", CT.ElectroPurple) +
                       "伤害，并为雷电将军授以「雷罚恶曜之眼」，持续" +
                       CT.GetColorfulText(skill2_eyeDuration[lel].ToString("f0"), CT.normalBlue) +
                       "秒\n\n「雷罚恶曜之眼」\n•存在期间，将为雷电将军提供" +
                       CT.GetColorfulText("100%", CT.normalBlue) +
                       "的额外充能效率\n•雷电将军攻击后，「雷罚恶曜之眼」会进行协同攻击，召下一道落雷造成" +
                       CT.ChangeToColorfulPercentage(skill2_CoorMulti[lel]) +
                       "的范围" +
                       CT.GetColorfulText("雷元素魔法", CT.ElectroPurple) +
                       "伤害。每" +
                       CT.GetColorfulText("3", CT.normalRed) +
                       "秒至多进行一次协同攻击";

            default:
                return "消耗所有存储的愿力（最高计入100点），斩出粉碎一切诅咒的梦想一刀，造成" +
                       CT.ChangeToColorfulPercentage(skill3_burstMulti[lel]) +
                       "的" +
                       CT.GetColorfulText("雷元素魔法", CT.ElectroPurple) +
                       "伤害，随后进入梦想一心状态：普通攻击造成" +
                       CT.ChangeToColorfulPercentage(skill3_atkMulti[lel]) +
                       "的" +
                       CT.GetColorfulText("雷元素魔法", CT.ElectroPurple) +
                       "伤害，并为自己回复" +
                       CT.GetColorfulText(skill3_wpRecover[lel].ToString("f1"), CT.normalBlue) +
                       "点愿力\n\n每消耗1点愿力，本技能造成的伤害将提高" +
                       CT.GetColorfulText("1%", CT.normalBlue) +
                       "\n\n本技能的技力消耗不会为雷电将军提供愿力";
        }
    }

    public override string GetTalentDescription(int talentID)
    {
        if (talentID == 1)
        {
            return CT.GetColorfulText("雷电将军", CT.normalBlue) +
                   "/" +
                   CT.GetColorfulText("其他干员", CT.normalBlue) +
                   "释放技能时，每消耗" +
                   CT.GetColorfulText("1", CT.normalBlue) +
                   "点技力会为雷电将军提供" +
                   CT.GetColorfulText(talent1_RaidenMul.ToString("f1"), CT.normalBlue) +
                   "/" +
                   CT.GetColorfulText(talent1_OtherMul.ToString("f1"), CT.normalBlue) +
                   "点愿力。雷电将军倒下或撤退时会" +
                   CT.GetColorfulText("损失一半", CT.normalRed) +
                   "的愿力\n雷电将军在场上切换技能时，将消耗自身的愿力，每点愿力将为雷电将军恢复" +
                   CT.GetColorfulText("1", CT.normalBlue) +
                   "点技力\n\n当前拥有的愿力为：" +
                   CT.GetColorfulText(wp.ToString("f1"), CT.ElectroPurple) +
                   "/" + maxWP;
        }
        else
        {
            if (eliteLevel < 2) return "";
            return "基于充能效率超过100%的部分，每1%使雷电将军获得下列效果：\n•" +
                   // CT.GetColorfulText("元素伤害", CT.ElectroPurple) +
                   "元素伤害提高" +
                   CT.GetColorfulText((talent2_damInc * 100).ToString("f1") + "%", CT.normalBlue) +
                   "\n•梦想一心每次提供的愿力回复增加" +
                   CT.GetColorfulText(talent2_wpInc.ToString("f2"), CT.normalBlue);
        }
    }
    
    public override string GetSkillAtkRangeName(int SkillID)
    {
        return SkillID switch
        {
            1 => skill2_atkRange.name,
            2 => skill3_atkRange.name,
            _ => ""
        };
    }
}


public class DurationAtkRangeBuff : DurationBuffSlot
{
    private GameObject atkRange;
    private OperatorCore oc_;
    
    public DurationAtkRangeBuff(GameObject range, OperatorCore oc, float dur) : base(dur)
    {
        atkRange = range;
        oc_ = oc;
    }
    
    public override void BuffStart()
    {
        oc_.ChangeAtkRange(atkRange);
    }

    public override void BuffEnd()
    {
        oc_.ChangeAtkRange();
    }
    
}