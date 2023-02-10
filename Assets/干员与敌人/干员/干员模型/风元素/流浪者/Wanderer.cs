using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Wanderer : OperatorCore
{
    [Header("流浪者特效")] 
    public GameObject Circle;
    public List<GameObject> CircleLight = new List<GameObject>();
    public List<GameObject> norAtkObj = new List<GameObject>();
    public GameObject norHitAnim;
    public GameObject anemoBall;
    public GameObject smallBurst;
    public GameObject skill2AtkObj;
    public GameObject skill2_AtkRange;
    public GameObject flowRol;
    public GameObject bigBurst;
    public GameObject skill3_AtkRange;
    public GameObject backAnemoAtk;
    public GameObject backAnemoHit;
    public GameObject anemoShield;
    
    public AudioClip spAtkAudio;
    public AudioClip upAudio;
    public AudioClip skill3Audio;
    public List<AudioClip> groundAtkClips = new List<AudioClip>();
    public List<AudioClip> upAtkClips = new List<AudioClip>();
    [HideInInspector] public int groundAtkPointer;
    [HideInInspector] public int upAtkPointer;


    [HideInInspector] public Animator circleAnim;

    private ElementSlot[] TalentSlot = new ElementSlot[3];
    private ValueBuffInner circleAtkBuff;
    private ValueBuffInner circleLifeBuff;
    private ValueBuffInner circleRechargeBuff;
    [HideInInspector] public float critRate = 0;

    [HideInInspector] public float[] skill1_Multi = {1.5f, 1.7f, 1.9f, 2.1f, 2.3f, 2.6f, 3f};
    [HideInInspector] public float skill1_Radius = 0.9f;
    private float[] skill2_AtkSpeedMulti = {5, 6, 7, 9, 11, 13, 15};
    private float[] skill2_AtkMulti = {0.15f, 0.18f, 0.22f, 0.26f, 0.3f, 0.33f, 0.36f};
    private float[] skill3_Multi = {3f, 3.4f, 3.8f, 4.2f, 4.7f, 5.2f, 5.8f};
    private float skill3_pyroAtkInc = 0.5f;
    private float skill3_cyroMulti = 0.35f;
    private bool[] skill3_destory = new bool[4];
    private bool haveAnemoShield;
    private bool backAtkCanAttach;

    private const float circleDuration = 8f;
    private const float hydroLifeInc = 0.2f;
    private const float pyroAtkInc = 0.2f;
    private const float cyroCritRate = 0.15f;
    private const float elecRecover = 0.200001f;
    private const float talent2Rate = 0.16f;
    private const float talent2BackRate = 0.35f;


    protected override void Awake_Core()
    {
        base.Awake_Core();
        for (int i = 0; i < 3; i++) TalentSlot[i] = new ElementSlot();
        circleAnim = Circle.GetComponent<Animator>();
    }

    protected override void Start_Core()
    {
        base.Start_Core();
        circleAtkBuff = new ValueBuffInner(ValueBuffMode.Percentage, 0);
        atk_.AddValueBuff(circleAtkBuff);
        circleLifeBuff = new ValueBuffInner(ValueBuffMode.Percentage, 0);
        life_.AddValueBuff(circleLifeBuff);
        circleRechargeBuff = new ValueBuffInner(ValueBuffMode.Fixed, 0);
        sp_.spRecharge.AddValueBuff(circleRechargeBuff);
    }

    public override void OperInit()
    {
        base.OperInit();
        foreach (var i in TalentSlot) i.eleType = ElementType.None;
        Circle.SetActive(false);
        anemoShield.SetActive(false);
        haveAnemoShield = false;
        if (getDamFuncList.Contains(AnemoShieldLastFunc)) getDamFuncList.Remove(AnemoShieldLastFunc);

        // GetAnemoShieldSure();
        // StartCoroutine(Test());
    }
    

    IEnumerator Test()
    {
        yield return new WaitForSeconds(0.5f);
        GetCircle(ElementType.Hydro);
        GetCircle(ElementType.Electro);
        GetCircle(ElementType.Pyro);
        // yield return new WaitForSeconds(5f);
        // GetCircle(ElementType.Pyro);
        // GetCircle(ElementType.Cryo);
        // GetCircle(ElementType.Electro);
        // yield return new WaitForSeconds(5f);
        // GetCircle(ElementType.Cryo);
        // GetCircle(ElementType.Hydro);
        // GetCircle(ElementType.Pyro);
        // yield return new WaitForSeconds(5f);
        // GetCircle(ElementType.Electro);
        // GetCircle(ElementType.Hydro);
        // GetCircle(ElementType.Cryo);
    }
    
    protected override void Update_Core()
    {
        base.Update_Core();
        for (int i = 2; i >= 0; i--)
        {// 元素花玉倒计时
            TalentSlot[i].eleCount -= Time.deltaTime;
            if (TalentSlot[i].eleCount > 0) continue;
            TalentSlot[i].eleType = ElementType.None;
            RefreshCircle();
        }

        
        circleAnim.speed = anim.speed;
    }

    public override void OnAttack()
    {
        sp_.GetSp_Atk();
        if (skillNum == 0 && sp_.outType == releaseType.atk && sp_.CanReleaseSkill())
        {
            anim.SetBool("sp", true);
        }
        
        int id = Random.Range(0, 2);
        float xx = Random.Range(0f, 1f);    // 暴击判断
        Archery(xx < critRate ? 2 : 1, norAtkObj[id], NorAnemoAtk);

        AudioManager.PlayEFF(groundAtkClips[groundAtkPointer], 0.6f);
        groundAtkPointer = (groundAtkPointer + 1) % groundAtkClips.Count;
    }

    public override void SkillStart_2()
    {
        base.SkillStart_2();
        AudioManager.PlayEFF(upAudio, 0.8f);

        RefreshCircle();
        Circle.SetActive(true);

        SkillAnimStaBuff animStaBuff = new SkillAnimStaBuff(this, anim, 1);
        BuffManager.AddBuff(animStaBuff);
        SkillAnimStaBuff circleAnimStaBuff = new SkillAnimStaBuff(this, circleAnim, 1);
        BuffManager.AddBuff(circleAnimStaBuff);
        WandererSkill2EndBuff skill2EndBuff = new WandererSkill2EndBuff(this);
        BuffManager.AddBuff(skill2EndBuff);

        SkillValueBuff atkValueBuff = new SkillValueBuff(
            atk_, ValueBuffMode.Percentage, skill2_AtkMulti[skillLevel[1]], this);
        BuffManager.AddBuff(atkValueBuff);

        SkillAtkSpeedBuff atkSpeedBuff = new SkillAtkSpeedBuff(
            atkSpeedController, skill2_AtkSpeedMulti[skillLevel[1]], this);
        BuffManager.AddBuff(atkSpeedBuff);

        SkillAtkRangeBuff atkRangeBuff = new SkillAtkRangeBuff(this, skill2_AtkRange);
        BuffManager.AddBuff(atkRangeBuff);
    }

    public override void SkillAtk_2()
    {
        base.SkillAtk_2();
        
        float xx = Random.Range(0f, 1f);    // 暴击判断
        Archery(xx < critRate ? 2 : 1, skill2AtkObj, NorAnemoAtk);

        AudioManager.PlayEFF(upAtkClips[upAtkPointer], 0.7f);
        upAtkPointer = (upAtkPointer + 1) % upAtkClips.Count;
    }


    public override void SkillStart_3()
    {
        base.SkillStart_3();
        AudioManager.PlayEFF(skill3Audio, 0.8f);

        SkillAnimStaBuff animStaBuff = new SkillAnimStaBuff(this, anim, 2);
        BuffManager.AddBuff(animStaBuff);

        SkillAtkRangeBuff atkRangeBuff = new SkillAtkRangeBuff(this, skill3_AtkRange);
        BuffManager.AddBuff(atkRangeBuff);

        // 摧毁所有的元素花玉
        skill3_destory[0] = skill3_destory[1] = skill3_destory[2] = skill3_destory[3] = false;
        for (int i = 0; i < 3; i++)
        {
            int id = TalentSlot[i].eleType switch
            {
                ElementType.Hydro => 0,
                ElementType.Pyro => 1,
                ElementType.Cryo => 2,
                ElementType.Electro => 3,
                _ => -1
            };
            if (id != -1) skill3_destory[id] = true;
            TalentSlot[i].eleType = ElementType.None;
        }
        RefreshCircle();
        
        // 加水火buff
        if (skill3_destory[0])      // 摧毁了水
        {
            life_.RecoverCompletely();
        }

        if (skill3_destory[1])      // 摧毁了火
        {
            SkillValueBuff pyroAtkBuff = new SkillValueBuff(atk_,
                ValueBuffMode.Percentage, skill3_pyroAtkInc, this);
            BuffManager.AddBuff(pyroAtkBuff);
        }
    }

    public override void SkillAtk_3()
    {
        base.SkillAtk_3();
        
        Vector3 pos = direction switch
        {
            FourDirection.Right => new Vector3(1, 0, 0),
            FourDirection.Left => new Vector3(-1, 0, 0),
            FourDirection.UP => new Vector3(0, 0, 1),
            FourDirection.Down => new Vector3(0, 0, -1),
            _ => Vector3.zero
        };
        GameObject burst = PoolManager.GetObj(bigBurst);
        burst.transform.position = transform.position + pos;
        DurationRecycleObj recycleObj = new DurationRecycleObj(burst, 1.2f);
        BuffManager.AddBuff(recycleObj);

        StartCoroutine(Skill3Damage());
    }

    IEnumerator Skill3Damage()
    {
        float dam = atk_.val * skill3_Multi[skillLevel[2]];
        if (skill3_destory[2]) dam *= skill3_cyroMulti;         // 加冰buff
        ElementSlot anemoSlot = new ElementSlot(ElementType.Anemo, 1f);

        yield return new WaitForSeconds(0.1f);
        Skill3_CauseDamage(dam, anemoSlot, true);
        
        yield return new WaitForSeconds(0.15f);
        Skill3_CauseDamage(dam, anemoSlot, false);
        
        yield return new WaitForSeconds(0.15f);
        Skill3_CauseDamage(dam, anemoSlot, false);
        
        yield return new WaitForSeconds(0.15f);
        if (skill3_destory[3])  // 如果有雷buff，额外一次
        {
            Skill3_CauseDamage(dam, anemoSlot, false);
        }
    }

    private void Skill3_CauseDamage(float dam, ElementSlot anemoSlot, bool attach)
    {
        foreach (var enemy in enemyList)
        {
            Battle(enemy, dam, DamageMode.Magic, anemoSlot, attach, true, true);
            GameObject hitAnim = PoolManager.GetObj(norHitAnim);
            hitAnim.transform.parent = enemy.transform;
            Vector3 pos = enemy.animTransform.localPosition + new Vector3(0, 0, 0.3f);
            hitAnim.transform.localPosition = pos;
            DurationRecycleObj recycleObj = new DurationRecycleObj(hitAnim, 0.5f, enemy, true);
            BuffManager.AddBuff(recycleObj);
        }
    }


    private void Archery(float multi, GameObject proArrow, Action<float, BattleCore, TrackMove> endAttack)
    {// 射一支箭出去，攻击倍率为multi
        
        var arrow = PoolManager.GetObj(proArrow);
        TrackMove tra = arrow.GetComponent<TrackMove>();
        
        Vector3 pos = transform.position;
        pos += new Vector3(ac_.dirRight ? 0.6f : -0.6f, 0.5f, 0.35f);
        Vector3 scale = tra.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (ac_.dirRight ? 1 : -1);
        tra.transform.localScale = scale;
        tra.Init(pos, this, target, 10f, endAttack, multi);
    }

    private void NorAnemoAtk(float multi, BattleCore tarBC, TrackMove tra)
    {
        GameObject hitAnim = PoolManager.GetObj(norHitAnim);
        hitAnim.transform.parent = tarBC.transform;
        Vector3 pos = tarBC.animTransform.localPosition + new Vector3(0, 0, 0.3f);
        hitAnim.transform.localPosition = pos;
        DurationRecycleObj recycleObj = new DurationRecycleObj(hitAnim, 0.5f, tarBC, true);
        BuffManager.AddBuff(recycleObj);

        AtkGetCircle(tarBC);
        GetAnemoShield();
        
        ElementSlot anemoSlot = new ElementSlot(ElementType.Anemo, 1f);
        Battle(tarBC, atk_.val * multi, DamageMode.Magic, anemoSlot, defaultElementTimer, true);
    }
    
    
    public void GetCircle(ElementType elementType)
    {
        int i = 0;
        for(i = 0; i < 3; i++) if (TalentSlot[i].eleType == elementType) break;
        
        if (i == 0)
        {
            TalentSlot[i].eleCount = circleDuration;
            return;
        }
        else if (i == 1)
        {
            TalentSlot[1].eleType = TalentSlot[0].eleType;
            TalentSlot[1].eleCount = TalentSlot[0].eleCount;
            TalentSlot[0].eleType = elementType;
            TalentSlot[0].eleCount = circleDuration;
            RefreshCircle();
            return;
        }
        TalentSlot[2].eleType = TalentSlot[1].eleType;
        TalentSlot[2].eleCount = TalentSlot[1].eleCount;
        TalentSlot[1].eleType = TalentSlot[0].eleType;
        TalentSlot[1].eleCount = TalentSlot[0].eleCount;
        TalentSlot[0].eleType = elementType;
        TalentSlot[0].eleCount = circleDuration;
        RefreshCircle();
    }

    public void AtkGetCircle(BattleCore tarBC)
    {// 根据攻击到的敌人身上的元素，获得元素花玉
        if (tarBC.IsAttachElement(ElementType.Hydro)) GetCircle(ElementType.Hydro);
        if (tarBC.IsAttachElement(ElementType.Pyro)) GetCircle(ElementType.Pyro);
        if (tarBC.IsAttachElement(ElementType.Cryo)) GetCircle(ElementType.Cryo);
        if (tarBC.IsAttachElement(ElementType.Electro)) GetCircle(ElementType.Electro);
    }

    public void RefreshCircle()
    {
        // 根据花玉设置buff
        int db = (skillNum == 1 && sp_.during) ? 2 : 1;         // 如果处于二技能的释放阶段，则效果翻倍
        circleLifeBuff.v = CircleIsActive(ElementType.Hydro) ? hydroLifeInc * db : 0;
        life_.RefreshValue();
        circleAtkBuff.v = CircleIsActive(ElementType.Pyro) ? pyroAtkInc * db : 0;
        atk_.RefreshValue();
        critRate = CircleIsActive(ElementType.Cryo) ? cyroCritRate * db : 0;
        circleRechargeBuff.v = CircleIsActive(ElementType.Electro) ? elecRecover * db : 0;
        sp_.spRecharge.RefreshValue();

        // 第一个元素花玉
        if (TalentSlot[0].eleType == ElementType.None)
        {
            foreach (var i in CircleLight) i.SetActive(false);
            Circle.SetActive(db == 2);  // 处于二技能释放阶段则显示圆环
            return;
        }
        Circle.SetActive(true);

        for (int i = 0; i < 4; i++) CircleLight[i].SetActive(false);
        int id = TalentSlot[0].eleType switch
        {
            ElementType.Pyro => 0,
            ElementType.Electro => 1,
            ElementType.Cryo => 2,
            ElementType.Hydro => 3,
            _ => -1
        };
        CircleLight[id].SetActive(true);
        
        // 第二个元素花玉
        if (TalentSlot[1].eleType == ElementType.None)
        {
            for (int i = 4; i < 12; i++) CircleLight[i].SetActive(false);
            return;
        }
        
        for (int i = 4; i < 8; i++) CircleLight[i].SetActive(false);
        id = TalentSlot[1].eleType switch
        {
            ElementType.Pyro => 4,
            ElementType.Electro => 5,
            ElementType.Cryo => 6,
            ElementType.Hydro => 7,
            _ => -1
        };
        CircleLight[id].SetActive(true);
        
        // 第三个元素花玉
        if (TalentSlot[2].eleType == ElementType.None)
        {
            for (int i = 8; i < 12; i++) CircleLight[i].SetActive(false);
            return;
        }
        
        for (int i = 8; i < 12; i++) CircleLight[i].SetActive(false);
        id = TalentSlot[2].eleType switch
        {
            ElementType.Pyro => 8,
            ElementType.Electro => 9,
            ElementType.Cryo => 10,
            ElementType.Hydro => 11,
            _ => -1
        };
        CircleLight[id].SetActive(true);
    }
    
    private bool CircleIsActive(ElementType elementType)
    {
        return TalentSlot.Any(i => i.eleType == elementType);
    }


    public void GetAnemoShield()
    {// 获得风盾

        if (eliteLevel < 2) return;                         // 精二后才有
        if (Random.Range(0f, 1f) > talent2Rate) return;     // 16%概率获得
        if (haveAnemoShield) return;                        // 已经有就不重复了

        haveAnemoShield = true;
        anemoShield.SetActive(true);
        getDamFuncList.Add(AnemoShieldLastFunc);
    }
    
    public void GetAnemoShieldSure()
    {// 立刻获得风盾，测试用
        haveAnemoShield = true;
        anemoShield.SetActive(true);
        getDamFuncList.Add(AnemoShieldLastFunc);
    }

    private float AnemoShieldLastFunc(float dam)
    {// 注册给伤害委托的函数，将最终伤害变为0

        // 去除风盾效果
        anemoShield.SetActive(false);
        haveAnemoShield = false;
        getDamFuncList.Remove(AnemoShieldLastFunc);
        
        // 根据元素花玉的数量发射反击弹
        if (lastAttacker != null)
        {
            int num = 0;
            for(;num < 3;num++) if (TalentSlot[num].eleType == ElementType.None) break;
            StartCoroutine(BackAtk(num));
        }
        
        // 显示闪避文字
        GameObject obj = PoolManager.GetObj(StoreHouse.instance.reactionShowText);
        obj.transform.SetParent(OperUIManager.WorldCanvas.transform);
        Text text = obj.GetComponent<Text>();
        text.text = "闪避";
        text.color = new Color32(200, 200, 200, 255);
        text.transform.position = transform.position + new Vector3(0, 0.3f, 0);

        return 0;
    }

    IEnumerator BackAtk(int num)
    {
        List<Vector3> posList = new List<Vector3>();
        int dx = ac_.dirRight ? -1 : 1;
        posList.Add(new Vector3(dx * 0.3f, 0.5f, 0.3f));
        posList.Add(new Vector3(dx * 0.3f, 0.5f, 0.6f));
        posList.Add(new Vector3(dx * 0.3f, 0.5f, 0.0f));

        backAtkCanAttach = true;
        for (int i = 0; i < num; i++)
        {
            GameObject arrow = PoolManager.GetObj(backAnemoAtk);
            TrackMove tra = arrow.GetComponent<TrackMove>();
            Vector3 pos = transform.position;
            pos += posList[i];
            tra.Init(pos, this, lastAttacker, 12f, BackAnemoAtk, talent2BackRate);

            yield return new WaitForSeconds(0.15f);
        }
    }
    
    private void BackAnemoAtk(float multi, BattleCore tarBC, TrackMove tra)
    {
        GameObject hitAnim = PoolManager.GetObj(backAnemoHit);
        hitAnim.transform.parent = tarBC.transform;
        Vector3 pos = tarBC.animTransform.localPosition + new Vector3(0, 0, 0.3f);
        hitAnim.transform.localPosition = pos;
        DurationRecycleObj recycleObj = new DurationRecycleObj(hitAnim, 1f, tarBC, true);
        BuffManager.AddBuff(recycleObj);

        AtkGetCircle(tarBC);
        
        ElementSlot anemoSlot = new ElementSlot(ElementType.Anemo, 1f);
        Battle(tarBC, atk_.val * multi, DamageMode.Magic, anemoSlot, backAtkCanAttach, true);

        backAtkCanAttach = false;
    }
    
    
    
    public override string GetSkillDescription(int SkillID)
    {
        int lel = skillLevel[SkillID];
        switch (SkillID)
        {
            case 0:
                return "流浪者的下一次攻击会在前方凝聚高压气场，在小范围内造成" +
                       CT.ChangeToColorfulPercentage(skill1_Multi[lel]) + "的" +
                       CT.GetColorfulText("风元素法术", CT.AnemoGreen) + "伤害";
            case 1:
                return "流浪者凝聚大气的力量，摆脱大地的桎梏，进入「优风倾姿」状态\n\n「优风倾姿」：\n·攻击力提升" +
                       CT.ChangeToColorfulPercentage(skill2_AtkMulti[lel]) +
                       "\n·攻击速度提升" +
                       CT.GetColorfulText(skill2_AtkSpeedMulti[lel].ToString("f0")) +
                       "\n·攻击范围" + CT.GetColorfulText("扩大") +
                       "\n·「元素花玉」带来的提升效果" + CT.GetColorfulText("翻倍") +
                       "\n·不容易成为敌人的攻击目标";
            default:
                return "流浪者将大气压缩，" + CT.GetColorfulText("摧毁", CT.normalRed) +
                       "所有的「元素花玉」，在前方大范围内造成" + CT.GetColorfulText("3") + "次" +
                       CT.ChangeToColorfulPercentage(skill3_Multi[lel]) + "的" +
                       CT.GetColorfulText("风元素法术", CT.AnemoGreen) + "伤害\n\n" +
                       "根据摧毁的「元素花玉」类型，本技能获得以下效果：\n·" +
                       CT.GetColorfulText("水", CT.HydroBlue) + "：" +
                       CT.GetColorfulText("恢复") + "所有的生命值\n·" +
                       CT.GetColorfulText("火", CT.PyroRed) + "：攻击力提升" +
                       CT.ChangeToColorfulPercentage(skill3_pyroAtkInc) + "\n·" +
                       CT.GetColorfulText("冰", CT.CryoWhite) + "：造成的伤害提升" +
                       CT.ChangeToColorfulPercentage(skill3_cyroMulti) + "\n·" +
                       CT.GetColorfulText("雷", CT.ElectroPurple) + "：额外造成" +
                       CT.GetColorfulText("1") + "次伤害";
        }
    }

    public override string GetTalentDescription(int talentID)
    {
        if (talentID == 1)
        {
            int db = (skillNum == 1 && sp_.during) ? 2 : 1;
            return "流浪者的" +
                   CT.GetColorfulText("风元素攻击", CT.AnemoGreen) +
                   "若接触到了携带有" +
                   CT.GetColorfulText("水", CT.HydroBlue) + "、" +
                   CT.GetColorfulText("火", CT.PyroRed) + "、" +
                   CT.GetColorfulText("冰", CT.CryoWhite) + "、" +
                   CT.GetColorfulText("雷", CT.ElectroPurple) +
                   "的敌人时，会获得一枚对应元素的「元素花玉」，持续" +
                   CT.GetColorfulText(circleDuration.ToString("f0")) +
                   "秒，至多同时存在" + CT.GetColorfulText("3", CT.normalRed) +
                   "个。「元素花玉」具有如下效果：\n" +
                   CT.GetColorfulText("水", CT.HydroBlue) + "：生命上限提升" +
                   CT.ChangeToColorfulPercentage(hydroLifeInc * db) +
                   (CircleIsActive(ElementType.Hydro) ? CT.GetColorfulText("\t\t\t\t\t（已激活）", CT.normalGreen) : "") +
                   "\n" +
                   CT.GetColorfulText("火", CT.PyroRed) + "：攻击力提升" +
                   CT.ChangeToColorfulPercentage(pyroAtkInc * db) +
                   (CircleIsActive(ElementType.Pyro) ? CT.GetColorfulText("\t\t\t\t\t（已激活）", CT.normalGreen) : "") +
                   "\n" +
                   CT.GetColorfulText("冰", CT.CryoWhite) + "：" +
                   CT.ChangeToColorfulPercentage(cyroCritRate * db) + "的几率造成双倍伤害" +
                   (CircleIsActive(ElementType.Cryo) ? CT.GetColorfulText("\t\t（已激活）", CT.normalGreen) : "") + 
                   "\n" +
                   CT.GetColorfulText("雷", CT.ElectroPurple) + "：充能效率提升" +
                   CT.ChangeToColorfulPercentage(elecRecover * db) +
                   (CircleIsActive(ElementType.Electro) ? CT.GetColorfulText("\t\t\t\t\t（已激活）", CT.normalGreen) : "");
        }
        else
        {
            if (eliteLevel < 2) return "";
            return "流浪者的攻击命中敌人后，有" +
                   CT.ChangeToColorfulPercentage(talent2Rate) +
                   "的几率获得「倾落」效果：\n流浪者下一次受到攻击后，" +
                   CT.GetColorfulText("闪避", CT.normalBlue) +
                   "此次攻击，并且每有一枚「元素花玉」，就会对攻击来源发射一枚风矢，造成" +
                   CT.ChangeToColorfulPercentage(talent2BackRate) + "的" +
                   CT.GetColorfulText("风元素法术", CT.AnemoGreen) + "伤害";
        }
    }
    
    public override string GetSkillAtkRangeName(int SkillID)
    {
        return SkillID switch
        {
            1 => skill2_AtkRange.name,
            2 => skill3_AtkRange.name,
            _ => ""
        };
    }
}

public class WandererSkill2EndBuff : SkillBuffSlot
{
    private Wanderer wan;
    public WandererSkill2EndBuff(Wanderer wan_) : base(wan_)
    {
        wan = wan_;
    }

    public override void BuffStart()
    {
        base.BuffStart();
        wan.tarPriority -= 10000;
        wan.upAtkPointer = 0;
    }

    public override void BuffUpdate() { }
    
    public override void BuffEnd()
    {
        wan.RefreshCircle();
        wan.tarPriority += 10000;
        wan.groundAtkPointer = 0;
        base.BuffEnd();
    }
}