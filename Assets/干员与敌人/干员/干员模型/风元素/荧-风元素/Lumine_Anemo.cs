using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Lumine_Anemo : TravellerCore
{
    [Header("荧的特效")]
    public GameObject NorAtkSlash;
    public GameObject Skill1_SmallSlash;
    public GameObject Skill1_BigSlash;
    public GameObject Skill1_AtkRange;
    public GameObject Skill1_Hit;
    public GameObject Skill2_Anim;
    public GameObject Skill2_Hit;
    public GameObject Skill2_BurstHit;
    public GameObject Skill2_AtkRange;
    public GameObject Skill3_FlyAnim;
    public LumineTornado tornado;
    public GameObject HealAnim;
    public GameObject GreenBuffAnim;
    public AudioClip NorAtkAudio;
    public AudioClip SharpAtkAudio;
    public AudioClip Skill2Audio;
    public AudioClip Skill2HitAudio;
    public AudioClip Skill3Audio;

    private float[] Skill1_Multi = {1.1f, 1.25f, 1.4f, 1.6f, 1.8f, 2.05f, 2.4f};
    private float[] Skill2_DurMilti = {1f, 1.15f, 1.3f, 1.5f, 1.7f, 1.9f, 2.2f};
    private float[] Skill2_BurstMilti = {2f, 2.3f, 2.6f, 3f, 3.4f, 3.8f, 4.4f};
    private float[] Skill2_BurstPower;
    private float[] Skill3_Duration = {4, 4, 4, 4.5f, 4.5f, 4.5f, 5};
    private float[] Skill3_Multi = {1.3f, 1.5f, 1.7f, 2f, 2.3f, 2.6f, 3f};
    private float[] Skill3_EleMulti = {0.35f, 0.42f, 0.5f, 0.58f, 0.66f, 0.75f, 0.85f};
    private ElementTimer skill3_timer;
    private GreenShadowTimer greenTimer;    // 翠绿之影计时器
    
    
    protected override void Awake_Core()
    {
        base.Awake_Core();
        Skill2_BurstPower = new float[]
        {
            PushAndPullController.littleForce, PushAndPullController.littleForce,
            PushAndPullController.littleForce, PushAndPullController.littleForce,
            PushAndPullController.mediumForce, PushAndPullController.mediumForce,
            PushAndPullController.mediumForce,
        };
        skill3_timer = new ElementTimer(this, 2f);
        greenTimer = new GreenShadowTimer(this, 8f);

        if (gameManager.knowledgeData.BuffAnemo.num > 0)
        {// 愿风神忽悠你，全场+10法抗，-15%再部署时间
            InitManager.GlobleOperInitAction += core =>
            {// 减法抗
                ValueBuffInner inner = new ValueBuffInner(ValueBuffMode.Fixed, 10);
                core.magicDef_.AddValueBuff(inner);
            };
            InitManager.GlobleOperInitAction += core =>
            {// 减再部署时间
                ValueBuffInner inner2 = new ValueBuffInner(ValueBuffMode.Percentage, -0.15f);
                core.recoverTime.AddValueBuff(inner2);
            };
        }

        SwirlAction += greenTimer.Add;     // 给自己加上翠绿之影回调
    }

    public override void OperInit()
    {
        base.OperInit();
        tornado.gameObject.SetActive(false);
        KnowledgeAction.ActAnemo(this);
        if (eliteLevel >= 2 && skillNum == 2 && gameManager.knowledgeDataStrengthen.SpInitAnemo.num > 0)
        {// 装备风息激荡的旅行者在部署时，直接补满所有技力
            sp_.GetSp(sp_.maxSp);
        }

        DieAction += ClearGreenShadow;  // 死亡时删除所有翠绿之影
    }

    public void Skill1_Begin()
    {// 在攻击动画开头调用，如果本次攻击会释放一技能，将攻击范围转变
        if (skillNum == 0 && sp_.CanReleaseSkill())
            ChangeAtkRange(Skill1_AtkRange);
    }

    public override void OnAttack()
    {
        if (skillNum == 0 && sp_.CanReleaseSkill())
        {// 可以释放一技能了
            sp_.ReleaseSkill();
            bool isSuper = eliteLevel >= 2 && gameManager.knowledgeDataStrengthen.SuperSlashAnemo.num > 0;

            var anemoSlash = PoolManager.GetObj(isSuper ? Skill1_BigSlash : Skill1_SmallSlash);
            anemoSlash.transform.SetParent(transform);
            anemoSlash.transform.localPosition = direction switch
            {
                FourDirection.Right => new Vector3(0, 0, 0.4f),
                FourDirection.Left => new Vector3(0, 0, 0.4f),
                _ => Vector3.zero
            };
            anemoSlash.transform.eulerAngles = direction switch
            {
                FourDirection.Right => new Vector3(0, 90, 0),
                FourDirection.Left => new Vector3(0, -90, 0),
                FourDirection.Down => new Vector3(0, 180, 0),
                _ => new Vector3(0, 0, 15)
            };
            DurationRecycleObj recycleObj2 = new DurationRecycleObj(anemoSlash, 1f, this, true);
            BuffManager.AddBuff(recycleObj2);

            // 造成伤害
            StartCoroutine(Skill1_CauseDamage(isSuper));
            AudioManager.PlayEFF(SharpAtkAudio);
        }
        else
        {
            if (skillNum == 0) sp_.GetSp_Atk();
            AudioManager.PlayEFF(NorAtkAudio);
        }

        var slash = PoolManager.GetObj(NorAtkSlash);
        slash.transform.SetParent(transform);
        int fdir = ac_.dirRight ? 1 : -1;
        slash.transform.localPosition = new Vector3(0.2f * fdir, 0.1f, 0.3f);
        slash.transform.eulerAngles = new Vector3(60, -135 * fdir, 20 * fdir);
        slash.transform.localScale = new Vector3(-0.5f * fdir, 0.5f, 0.5f);
        DurationRecycleObj recycleObj = new DurationRecycleObj(slash, 1f, this, true);
        BuffManager.AddBuff(recycleObj);
        
        Battle(target, atk_.val);
    }

    IEnumerator Skill1_CauseDamage(bool isSuper)
    {
        yield return new WaitForSeconds(0.04f);
        bool isSharp = gameManager.knowledgeData.SharpAnemo.num > 0;
        int cnt = isSuper ? 3 : 1;
        bool canAttach = true;
        float dam = atk_.val * Skill1_Multi[skillLevel[0]] * (isSuper ? 0.7f : 1);
        ElementSlot anemoSlot = new ElementSlot(ElementType.Anemo, 1f);
        for (int i = 0; i < cnt; i++)
        {
            foreach (var EC in enemyList)
            {
                var args = new BattleArgs {ignoreDefPercentage = isSharp ? 0.25f : 0};// 无视防御
                Battle(EC, dam, DamageMode.Physical, anemoSlot, canAttach, true, false,
                    args);
                
                GameObject hitAnim = PoolManager.GetObj(Skill1_Hit);
                hitAnim.transform.parent = EC.transform;
                Vector3 pos = EC.animTransform.localPosition + new Vector3(0, 0, 0.3f);
                hitAnim.transform.localPosition = pos;
                DurationRecycleObj recycleObj3 = new DurationRecycleObj(hitAnim, 0.5f, EC, true);
                BuffManager.AddBuff(recycleObj3);
            }
            canAttach = false;
            yield return new WaitForSeconds(0.12f);
        }
        ChangeAtkRange();   // 将攻击范围变回来
    }


    public override void SkillStart_2()
    {
        SkillAnimStaBuff staBuff = new SkillAnimStaBuff(this, anim, 1);
        BuffManager.AddBuff(staBuff);

        SkillAtkRangeBuff atkRangeBuff = new SkillAtkRangeBuff(this, Skill2_AtkRange, true);
        BuffManager.AddBuff(atkRangeBuff);

        GameObject wind = PoolManager.GetObj(Skill2_Anim);
        PalmVortex pv = wind.GetComponent<PalmVortex>();
        Vector3 center = pv.Init(this);

        StartCoroutine(Skill2_DurDamage(BaseFunc.x0z(BaseFunc.FixCoordinate(center))));
        AudioManager.PlayEFF(Skill2Audio);
    }

    IEnumerator Skill2_DurDamage(Vector3 center)
    {
        yield return new WaitForSeconds(0.4f);

        bool canPull = gameManager.knowledgeData.WhirlingAnemo.num > 0;
        int cnt = 5;
        bool canAttach = true;
        float dam = atk_.val * Skill2_DurMilti[skillLevel[1]];
        ElementSlot anemoSlot = new ElementSlot(ElementType.Anemo, 1f);

        for (int i = 0; i < cnt; i++)
        {
            if (enemyList.Count > 0) AudioManager.PlayEFF(Skill2HitAudio);
            foreach (var BC in enemyList)
            {
                EnemyCore EC = (EnemyCore) BC;
                Battle(EC, dam, DamageMode.Physical, anemoSlot, canAttach, true);

                if (canPull) // 对敌人施加吸附力
                    EC.ppc_.Absorb_Center(center, PushAndPullController.littleForce, 1.4f);
                
                
                GameObject hit = PoolManager.GetObj(Skill2_Hit);
                hit.transform.SetParent(EC.transform);
                Vector3 pos = EC.animTransform.localPosition + new Vector3(0, 0.05f, 0.3f);
                hit.transform.localPosition = pos;
                DurationRecycleObj recycleObj = new DurationRecycleObj(hit, 0.5f, EC, true);
                BuffManager.AddBuff(recycleObj);
            }

            canAttach = false;
            yield return new WaitForSeconds(0.3f);
        }
    }

    public void Skill2_BurstDamage()
    {
        base.SkillStart_2();

        Vector3 rol = direction switch
        {
            FourDirection.Right => new Vector3(0, 90, 0),
            FourDirection.Left => new Vector3(0, -90, 0),
            FourDirection.UP => new Vector3(0, 0, 0),
            _ => new Vector3(0, 180, 0)
        };
        Vector3 dir = direction switch
        {
            FourDirection.Right => new Vector3(1,0,0),
            FourDirection.Left => new Vector3(-1,0,0),
            FourDirection.UP => new Vector3(0,0,1),
            _ => new Vector3(0,0,-1)
        };
        
        float force = GetSkill2_Power();
        float dam = atk_.val * Skill2_BurstMilti[skillLevel[1]];
        ElementSlot anemoSlot = new ElementSlot(ElementType.Anemo, 1f);
        foreach (var BC in enemyList)
        {
            EnemyCore EC = (EnemyCore) BC;
            Battle(EC, dam, DamageMode.Physical, anemoSlot, true, true);
            EC.ppc_.Push(dir, force);

            GameObject hit = PoolManager.GetObj(Skill2_BurstHit);
            hit.transform.position = EC.animTransform.position + new Vector3(0, 0, 0.3f);
            hit.transform.SetParent(EC.transform);
            hit.transform.eulerAngles = rol;
            DurationRecycleObj hitRecycle = new DurationRecycleObj(hit, 0.8f, EC, true);
            BuffManager.AddBuff(hitRecycle); 
        }
    }


    public override void SkillStart_3()
    {
        base.SkillStart_3();
        AudioManager.PlayEFF(Skill3Audio);
        SkillAnimStaBuff animStaBuff = new SkillAnimStaBuff(this, anim, 2);
        BuffManager.AddBuff(animStaBuff);

        GameObject fly = PoolManager.GetObj(Skill3_FlyAnim);
        fly.transform.SetParent(transform);
        fly.transform.localPosition = Vector3.zero;
        DurationRecycleObj recycleObj = new DurationRecycleObj(fly, 1, this, true);
        BuffManager.AddBuff(recycleObj);
        
        tornado.Init();
        
        // 复苏之风
        if (eliteLevel < 2 || gameManager.knowledgeDataStrengthen.HealAnemo.num == 0) return;
        foreach (var OC in InitManager.operList)
        {
            GameObject heal = PoolManager.GetObj(HealAnim);
            heal.transform.SetParent(OC.transform);
            heal.transform.localPosition = Vector3.zero;
            DurationRecycleObj recycleObj2 = new DurationRecycleObj(heal, 1f, OC, true);
            BuffManager.AddBuff(recycleObj2);

            LumineTornadoHealBuff healBuff = new LumineTornadoHealBuff(OC, this);
            BuffManager.AddBuff(healBuff);
        }
    }

    public void Skill3_Damage(EnemyCore tarEC)
    {
        // 尝试元素转化
        if (tornado.willType == ElementType.Anemo)
        {
            if(tarEC.IsAttachElement(ElementType.Pyro))
                tornado.PreChangeType(ElementType.Pyro);
            else if(tarEC.IsAttachElement(ElementType.Hydro))
                tornado.PreChangeType(ElementType.Hydro);
            else if(tarEC.IsAttachElement(ElementType.Electro))
                tornado.PreChangeType(ElementType.Electro);
            else if (tarEC.IsAttachElement(ElementType.Cryo))
                tornado.PreChangeType(ElementType.Cryo);
        }
        
        float dam = atk_.val * Skill3_Multi[skillLevel[2]];
        ElementSlot anemoSlot = new ElementSlot(ElementType.Anemo, 1f);
        bool canAttach = skill3_timer.AttachElement(tarEC);
        Battle(tarEC, dam, DamageMode.Physical, anemoSlot, canAttach, true);
        
        // 染色附加伤害
        if (tornado.type != ElementType.Anemo)
        {
            float dyeDam = atk_.val * GetSkill3_EleMulti();
            ElementSlot slot = new ElementSlot(tornado.type, 1f);
            Battle(tarEC, dyeDam, DamageMode.Physical, slot, canAttach, true);
        }
    }
    

    public void ClearGreenShadow(BattleCore bc)
    {// 旅行者死亡时触发，删除场上所有翠绿之影效果
        greenTimer.Clear();
    }


    public float GetSkill2_Power()
    {
        float po = Skill2_BurstPower[skillLevel[1]];
        if (eliteLevel >= 2)
            po += gameManager.knowledgeDataStrengthen.PowerUpAnemo.num *
                  PushAndPullController.littleForce;
        return po;
    }
    
    public float GetSkill3_Duration()
    {
        float dur = Skill3_Duration[skillLevel[2]];
        if (eliteLevel >= 2)
            dur += gameManager.knowledgeDataStrengthen.LongDurationAnemo.num * 8;
        return dur;
    }

    public float GetSkill3_EleMulti()
    {
        float multi = Skill3_EleMulti[skillLevel[2]];
        if (eliteLevel >= 2 && gameManager.knowledgeDataStrengthen.ExtraDamIncAnemo.num > 0)
            multi *= 2;
        return multi;
    }

    public override string GetSkillDescription(int SkillID)
    {
        int lel = skillLevel[SkillID];
        string str;
        switch (SkillID)
        {
            case 0:
                str = "旅行者的下一次攻击将额外释放一道风刃，对前方的所有敌人造成";
                if (eliteLevel >= 2 && gameManager.knowledgeDataStrengthen.SuperSlashAnemo.num > 0)
                    str += CT.GetColorfulText("3") + "次" + CT.ChangeToColorfulPercentage(Skill1_Multi[lel]) +
                           CT.GetColorfulText("*0.7", CT.normalRed);
                else str += CT.ChangeToColorfulPercentage(Skill1_Multi[lel]);
                str += "的" + CT.GetColorfulText("风元素物理", CT.AnemoGreen) + "伤害";
                
                if (gameManager.knowledgeData.SharpAnemo.num > 0)
                    str += "\n\n·此次伤害无视敌人" + CT.GetColorfulText("25%") + "的防御力";
                
                return str;
            case 1:
                str = "旅行者在掌中汇聚真空涡流，在持续时间内对前方范围内的敌人造成5次" +
                      CT.ChangeToColorfulPercentage(Skill2_DurMilti[lel]) + "的" +
                      CT.GetColorfulText("风元素物理", CT.AnemoGreen) + "伤害";
                if (gameManager.knowledgeData.WhirlingAnemo.num > 0)
                    str += "，同时将敌人向中心点小力的汇聚";
                str += "\n\n持续时间结束后，真空涡流会炸裂，对所有敌人造成" +
                       CT.ChangeToColorfulPercentage(Skill2_BurstMilti[lel]) + "的" +
                       CT.GetColorfulText("风元素物理", CT.AnemoGreen) + "伤害，" +
                       "同时将他们向前方";
                str += CT.GetColorfulText(PushAndPullController.PowerInterpreter(GetSkill2_Power())) +
                       "的推开";
                return str;
            default:
                str = "旅行者唤出持续前进的龙卷风，对接触到的敌人造成" +
                      CT.ChangeToColorfulPercentage(Skill3_Multi[lel]) + "的" +
                      CT.GetColorfulText("风元素物理", CT.AnemoGreen) + "伤害，" +
                      "并尝试将他们向中心吸附。龙卷风会在" +
                      CT.GetColorfulText(GetSkill3_Duration().ToString("f0")) +
                      "秒后消失\n\n如果龙卷风在行进途中接触了" +
                      CT.GetColorfulText("水", CT.HydroBlue) + "、" +
                      CT.GetColorfulText("火", CT.PyroRed) + "、" +
                      CT.GetColorfulText("冰", CT.CryoWhite) + "、" +
                      CT.GetColorfulText("雷", CT.ElectroPurple) + "元素，" +
                      "则会获得对应元素属性，额外造成该元素" +
                      CT.ChangeToColorfulPercentage(GetSkill3_EleMulti()) +
                      "的附加伤害。元素转换只能发生一次";
                if (eliteLevel >= 2 && gameManager.knowledgeDataStrengthen.HealAnemo.num > 0)
                    str += "\n\n·释放后，立刻使全场干员每秒恢复" +
                           CT.GetColorfulText("3%") + "生命值，持续" +
                           CT.GetColorfulText("5") + "秒";
                
                return str;
        }
    }
    
    public override string GetTalentDescription(int talentID)
    {
        string str;
        if (talentID == 1)
        {
            str = "罐装知识提供的属性提升已生效";
            if (gameManager.knowledgeData.BuffAnemo.num > 0)
                str += "\n·所有干员的法术抗性" +
                       CT.GetColorfulText("+10") + "，部署冷却时间" +
                       CT.GetColorfulText("-15%");
            return str;
        }
        else
        {
            if (eliteLevel < 2) return "";
            str = "强化罐装知识提供的属性提升已生效";
            if (gameManager.knowledgeDataStrengthen.SpInitAnemo.num > 0)
                str += "\n·装备「风息激荡」时部署，会直接补满所有技力";
            if (gameManager.knowledgeDataStrengthen.GreenAnemo.num > 0)
                str += "\n·触发扩散反应时，会使目标的元素抗性" +
                       CT.GetColorfulText("-30") + "，法术抗性" +
                       CT.GetColorfulText("-20") + "，持续8秒";

            return str;
        }
    }
    
    public override string GetSkillAtkRangeName(int SkillID)
    {
        return SkillID switch
        {
            0 => Skill1_AtkRange.name,
            1 => Skill2_AtkRange.name,
            // 2 => Skill3_AtkRange.name,
            _ => ""
        };
    }
    
}

public class LumineTornadoHealBuff : BuffSlot
{// 风息激荡对全场干员产生的治疗
    private Lumine_Anemo lumine;
    private OperatorCore oc_;
    private int count;
    private float t;
    private bool isDie;

    public LumineTornadoHealBuff(OperatorCore oc, Lumine_Anemo lu)
    {
        lumine = lu;
        oc_ = oc;
        count = 5;
        t = 0;
        isDie = false;
    }

    public override void BuffStart()
    {
        oc_.DieAction += Die;
    }

    public override void BuffUpdate()
    {
        t -= Time.deltaTime;
        if (t <= 0)
        {
            t = 1;
            count--;
            oc_.GetHeal(lumine, oc_.life_.val * 0.03f);
        }
    }

    public override bool BuffEndCondition()
    {
        if (isDie) return true;
        return count <= 0;
    }

    public override void BuffEnd()
    {
        if (isDie) return;
        oc_.DieAction -= Die;
    }

    private void Die(BattleCore bc)
    {
        isDie = true;
    }
}

public class GreenShadowTimer : BuffTimer
{
    private const float MagicDefReduce = 20;
    private const float ElementResistanceReduce = 0.3f;
    private Lumine_Anemo lumine;
    
    class GreenShadowInner
    {
        public ValueBuffInner inner1;
        public ValueBuffInner inner2;
        public GameObject BuffAnim;
    }

    private Dictionary<BattleCore, GreenShadowInner> dict = new Dictionary<BattleCore, GreenShadowInner>();
    
    
    public GreenShadowTimer(Lumine_Anemo lu, float maxDurTime) : base(lu, maxDurTime)
    {
        lumine = lu;
    }

    public override void Add(BattleCore tarBC)
    {
        if (lumine.eliteLevel < 2 || 
            gameManager.knowledgeDataStrengthen.GreenAnemo.num == 0) return;
        base.Add(tarBC);
    }

    protected override void StartAction(BattleCore tarBC)
    {// 对某个敌人造成扩散后，触发翠绿之影效果
        base.StartAction(tarBC);

        ValueBuffInner magicDefInner = new ValueBuffInner(ValueBuffMode.Fixed, -MagicDefReduce);
        tarBC.magicDef_.AddValueBuff(magicDefInner);
        ValueBuffInner resistanceInner = new ValueBuffInner(ValueBuffMode.Fixed, -ElementResistanceReduce);
        tarBC.elementResistance.AddValueBuff(resistanceInner);

        // 放入翠绿之影buff动画
        GameObject shadow = PoolManager.GetObj(lumine.GreenBuffAnim);
        shadow.transform.SetParent(tarBC.transform);
        Vector3 pos = tarBC.animTransform.localPosition + new Vector3(0, 0, 0.7f);
        shadow.transform.localPosition = pos;

        GreenShadowInner inner = new GreenShadowInner
        {
            inner1 = magicDefInner,
            inner2 = resistanceInner,
            BuffAnim = shadow
        };
        dict.Add(tarBC, inner);

        tarBC.DieAction += RecycleBuff;
    }

    protected override void EndAction(BattleCore tarBC)
    {// 某个敌人持续时间到了，去除翠绿之影效果
        base.EndAction(tarBC);
        GreenShadowInner two = dict[tarBC];
        tarBC.magicDef_.DelValueBuff(two.inner1);
        tarBC.elementResistance.DelValueBuff(two.inner2);
        PoolManager.RecycleObj(two.BuffAnim);

        tarBC.DieAction -= RecycleBuff;
        dict.Remove(tarBC);
    }

    public override void Clear()
    {
        foreach (var pair in buffTimeDict)
        {// 将所有敌人身上的翠绿之影效果移除
            EndAction(pair.Key);
        }
        
        buffTimeDict.Clear();   // 这里删掉的是buffTimeDict
    }

    private void RecycleBuff(BattleCore bc)
    {
        GreenShadowInner two = dict[bc];
        bc.magicDef_.DelValueBuff(two.inner1);
        bc.elementResistance.DelValueBuff(two.inner2);
        PoolManager.RecycleObj(two.BuffAnim);
        dict.Remove(bc);
        
        buffTimeDict.Remove(bc);
    }
    
}