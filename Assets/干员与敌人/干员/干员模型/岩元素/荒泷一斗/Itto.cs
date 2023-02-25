using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Itto : OperatorCore
{
    [Header("荒泷一斗的特效")] 
    public GameObject spAttackBoom;
    public GameObject spAttackGeoBoom;
    public GameObject skill3Ghost;
    public GameObject skill3AtkRange;
    public GameObject achouThrow;
    public GameObject achouHit;
    public operData od_achou;
    public GameObject achou_StartAnim;

    public AudioClip norAtkAudio;
    public AudioClip spAtk2Audio;
    public AudioClip spAtk3Audio;
    public AudioClip achouOutAudio;
    public AudioClip skill3OutAudio;

    private Achou achou;
    private GameObject achou_anim;
    
    private Vector3 ghostRightFront_localPos = new Vector3(0f, 0, 0.2f);
    private Vector3 ghostRightFront_localRol = new Vector3(-30, -30, 30);
    private Vector3 ghostRightFront_localScale = new Vector3(1f, 0.75f, -0.75f);
    
    private Vector3 ghostLeftFront_localPos = new Vector3(0f, 0, 0.2f);
    private Vector3 ghostLeftFront_localRol = new Vector3(-30, 30, 150);
    private Vector3 ghostLeftFront_localScale = new Vector3(1f, 0.75f, -0.75f);
    
    private Vector3 ghostRightBack_localPos = new Vector3(0f, 0, 0.2f);
    private Vector3 ghostRightBack_localRol = new Vector3(-30, 30, 150);
    private Vector3 ghostRightBack_localScale = new Vector3(1f, 0.75f, -0.75f);
    
    private Vector3 ghostLeftBack_localPos = new Vector3(0f, 0, 0.2f);
    private Vector3 ghostLeftBack_localRol = new Vector3(-30, -30, 30);
    private Vector3 ghostLeftBack_localScale = new Vector3(1f, 0.75f, -0.75f);
    
    
    private float[] spAttackMulti = {1.6f, 1.7f, 1.8f, 2f, 2.2f, 2.4f, 2.6f};
    private float[] skill_2_Multi = {3f, 3.3f, 3.6f, 3.9f, 4.2f, 4.6f, 5f};
    private float[] skill_3_atkBuff = {0.6f, 0.7f, 0.8f, 0.9f, 1f, 1.2f,1.4f};

    private float spAtkRadius = 1.3f;
    private float talent1_atkSpeedRate = 50f;
    private float talent1_duration = 8f;

    private int skill3_atkcnt = 0;
    [HideInInspector] public float maxAtkInterval;

    private ValueBuffer talent2buffer;
    private bool bufferActive;

    protected override void Start_Core()
    {
        achou = Instantiate(od_achou.operPrefab, transform, true).GetComponent<Achou>();
        achou.itto = this;
        achou_anim = Instantiate(achou_StartAnim, transform,true);
        achou_anim.SetActive(false);
        base.Start_Core();
        maxAtkInterval = od_.maxAtkInterval;
    }

    public override void OperInit()
    {
        base.OperInit();
        DieAction += achouRetreat;
    }

    protected override void Update_Core()
    {
        base.Update_Core();
        if (eliteLevel < 2) return;
        atk_.RefreshValue();
    }


    public override void OnAttack()
    {
        if (skillNum == 0) 
        {
            base.OnAttack();
            if (anim.GetBool("sp")) 
            { // 如果正在sp攻击时
                sp_.ReleaseSkill();
                return;
            }
            sp_.GetSp_Atk();
            if (sp_.CanReleaseSkill()) anim.SetBool("sp", true);
        }
        else
        {
            base.OnAttack();
        }
    }

    public override void SkillAtk_1()
    {
        Vector3 center = transform.position;
        List<EnemyCore> tars = InitManager.GetNearByEnemy(center, spAtkRadius);

        foreach (var enemy in tars)
        {
            Battle(enemy, atk_.val * spAttackMulti[skillLevel[0]], true);
        }
        
        // 特效
        GameObject spBoom = PoolManager.GetObj(spAttackBoom);
        spBoom.transform.position = center;
        DurationRecycleObj recycleObj = new DurationRecycleObj(spBoom, 1f);
        BuffManager.AddBuff(recycleObj);
        
        // 天赋加攻击速度
        DurationAtkSpeedBuff atkSpeedBuff = new DurationAtkSpeedBuff(atkSpeedController,
            talent1_atkSpeedRate, talent1_duration);
        BuffManager.AddBuff(atkSpeedBuff);
    }


    public override void SkillStart_2()
    {
        base.SkillStart_2();
        SkillAnimStaBuff animStaBuff = new SkillAnimStaBuff(this, anim, 1);
        BuffManager.AddBuff(animStaBuff);
        if(achou.gameObject.activeSelf) achou.Retreat();
        AudioManager.PlayEFF(achouOutAudio);
    }

    public override void SkillAtk_2()
    {
        GameObject athrow = PoolManager.GetObj(achouThrow);
        Vector3 pos = transform.position;
        int defaultTurnDir_X = defaultFaceRight ? 1 : -1;
        pos += new Vector3(0.3f * defaultTurnDir_X, 0, 0.3f);
        athrow.transform.position = pos;
        Vector3 rol = athrow.transform.localEulerAngles;
        rol.x = defaultTurnDir_X > 0 ? 0 : 180;
        athrow.transform.localEulerAngles = rol;
        DurationRecycleObj recycleObj = new DurationRecycleObj(athrow, 1f);
        BuffManager.AddBuff(recycleObj);
        
        achou_anim.SetActive(true);
        achou_anim.transform.localPosition = new Vector3(0, 0.15f, 0);

        anim.SetBool("sp", true);
        
        StartCoroutine(achou_animControl());
    }

    IEnumerator achou_animControl()
    {
        Vector3 scale = achou_anim.transform.localScale;
        int defaultTurnDir_X = defaultFaceRight ? 1 : -1;
        scale.x = Mathf.Abs(scale.x) * defaultTurnDir_X;
        achou_anim.transform.localScale = scale;
        
        Vector3 tarPos = transform.position;
        Vector3 hitRol = new Vector3(0, -90, 0);
        switch (direction)
        {
            case FourDirection.Right: 
                tarPos.x += 1;
                hitRol.x = 180;
                break;
            case FourDirection.Left: 
                tarPos.x -= 1;
                hitRol.x = 0;
                break;
            case FourDirection.UP:
                tarPos.z += 1;
                hitRol.x = -90;
                break;
            case FourDirection.Down:
                tarPos.z -= 1;
                hitRol.x = 90;
                break;
        }
        tarPos.y = 0;
        tarPos.z -= 0.2f;

        StartCoroutine(achouHitDelay(tarPos, hitRol));

        while (Vector3.Distance(achou_anim.transform.position, tarPos) > 1e-2)
        {

            achou_anim.transform.position = Vector3.MoveTowards(
                achou_anim.transform.position, tarPos,
                Time.deltaTime * 6f);
            yield return null;
        }

        yield return new WaitForSeconds(0.45f);
        achou_anim.SetActive(false);

        if (pdFront())
        {
            tarPos = BaseFunc.x0z(BaseFunc.FixCoordinate(achou_anim.transform.position));
            achou.gameObject.SetActive(true);
            achou.anim.transform.localScale = scale;
            achou.anim.transform.localPosition = new Vector3(0, 0, -0.2f);
            achou.transform.position = tarPos;
            achou.OperInit();
        }
    }

    IEnumerator achouHitDelay(Vector3 tarPos, Vector3 hitRol)
    {
        yield return new WaitForSeconds(0.2f);
        
        if (enemyList.Count>0) // 表示攻击范围内有敌人，牛牛击中了
        {
            // 特效
            GameObject ahit = PoolManager.GetObj(achouHit);
            tarPos += new Vector3(0, 0, 0.5f);
            ahit.transform.position = tarPos;
            ahit.transform.localEulerAngles = hitRol;
            DurationRecycleObj recycleObj = new DurationRecycleObj(ahit, 1f);
            BuffManager.AddBuff(recycleObj);
            
            // 造成伤害
            ElementSlot geoElement = new ElementSlot(ElementType.Geo, 1f);
            foreach (var enemy in enemyList)
            {
                Battle(enemy, atk_.val * skill_2_Multi[skillLevel[1]], DamageMode.Physical,
                    geoElement, defaultElementTimer, true, true);
            }
        }
    }

    private bool pdFront()
    {
        Vector3 frontPos = transform.position;
        frontPos += direction switch
        {
            FourDirection.Right => new Vector3(1, 0, 0),
            FourDirection.Left => new Vector3(-1, 0, 0),
            FourDirection.Down => new Vector3(0, 0, -1),
            FourDirection.UP => new Vector3(0, 0, 1),
            _ => Vector3.zero
        };
        Vector2 pos = BaseFunc.FixCoordinate(frontPos);
        TileSlot tile = InitManager.GetMap(pos);
        return Interpreter.canPut(tile.type, false, true);
    }

    private void achouRetreat(BattleCore bc_)
    {
        achou.Retreat();
    }


    public override void SkillStart_3()
    {
        base.SkillStart_3();
        SkillAtkSpeedBuff atkSpeedBuff = new SkillAtkSpeedBuff(atkSpeedController
            , 0, this, 1.334f);
        BuffManager.AddBuff(atkSpeedBuff);
        maxAtkInterval = 1.334f;
        
        SkillAnimStaBuff animStaBuff = new SkillAnimStaBuff(this, anim, 2);
        BuffManager.AddBuff(animStaBuff);

        SkillValueBuff atkValueBuff = new SkillValueBuff(atk_,
            ValueBuffMode.Percentage, skill_3_atkBuff[skillLevel[2]], this);
        BuffManager.AddBuff(atkValueBuff);

        SkillValueBuff defValueBuff = new SkillValueBuff(def_,
            ValueBuffMode.Percentage, -0.25f, this);
        BuffManager.AddBuff(defValueBuff);

        SkillAtkRangeBuff atkRangeBuff = new SkillAtkRangeBuff(this, skill3AtkRange);
        BuffManager.AddBuff(atkRangeBuff);

        skill3_atkcnt = 0;
        
        AudioManager.PlayEFF(skill3OutAudio);
    }

    public override void SkillAtk_3()
    {
        
        ElementSlot geoSlot = new ElementSlot(ElementType.Geo, 1f);
        foreach (var enemy in enemyList)
        {
            Battle(enemy, atk_.val, DamageMode.Physical, geoSlot, defaultElementTimer, true);
        }

        if (anim.GetBool("sp")) return;
        skill3_atkcnt++;
        if (skill3_atkcnt >= 4) anim.SetBool("sp", true);
    }
    
    public void SkillAtk_3_GhostFront()
    {
        Vector3 localpos, localRol, localScale;
        if (ac_.dirRight)
        {
            localpos = ghostRightFront_localPos;
            localRol = ghostRightFront_localRol;
            localScale = ghostRightFront_localScale;
        }
        else
        {
            localpos = ghostLeftFront_localPos;
            localRol = ghostLeftFront_localRol;
            localScale = ghostLeftFront_localScale;
        }
        GameObject ghost = PoolManager.GetObj(skill3Ghost);
        ghost.transform.parent = transform;
        ghost.transform.localPosition = localpos;
        ghost.transform.localEulerAngles = localRol;
        ghost.transform.localScale = localScale;
        DurationRecycleObj recycleObj = new DurationRecycleObj(ghost, 1f, this, true);
        BuffManager.AddBuff(recycleObj);
    }

    public void SkillAtk_3_GhostBack()
    {
        Vector3 localpos, localRol, localScale;
        if (ac_.dirRight)
        {
            localpos = ghostRightBack_localPos;
            localRol = ghostRightBack_localRol;
            localScale = ghostRightBack_localScale;
        }
        else
        {
            localpos = ghostLeftBack_localPos;
            localRol = ghostLeftBack_localRol;
            localScale = ghostLeftBack_localScale;
        }
        GameObject ghost = PoolManager.GetObj(skill3Ghost);
        ghost.transform.parent = transform;
        ghost.transform.localPosition = localpos;
        ghost.transform.localEulerAngles = localRol;
        ghost.transform.localScale = localScale;
        DurationRecycleObj recycleObj = new DurationRecycleObj(ghost, 1f, this, true);
        BuffManager.AddBuff(recycleObj);
    }

    public void Skill3_spAttack()
    {
        skill3_atkcnt = 0;
        
        ElementSlot geoSlot = new ElementSlot(ElementType.Geo, 1f);
        Vector3 center = transform.position;
        List<EnemyCore> tars = InitManager.GetNearByEnemy(center, spAtkRadius);

        foreach (var enemy in tars)
        {
            Battle(enemy, atk_.val * spAttackMulti[skillLevel[0]], 
                DamageMode.Physical, geoSlot, defaultElementTimer, true, true);
        }
        
        // 特效
        GameObject spGeoBoom = PoolManager.GetObj(spAttackGeoBoom);
        spGeoBoom.transform.position = center;
        DurationRecycleObj recycleObj = new DurationRecycleObj(spGeoBoom, 1f);
        BuffManager.AddBuff(recycleObj);
        
        // 天赋加攻击速度
        DurationAtkSpeedBuff atkSpeedBuff = new DurationAtkSpeedBuff(atkSpeedController,
            talent1_atkSpeedRate, talent1_duration);
        BuffManager.AddBuff(atkSpeedBuff);
    }


    public override void ElitismAction1_2()
    {
        atk_.AddFunc(talen2AtkFunc);
    }

    private float talen2AtkFunc(float val)
    {
        if (CrystallizationNum <= 0) return val;
        return val + def_.val * 0.35f;
    }
    

    public override string GetSkillDescription(int SkillID)
    {
        int lel = skillLevel[SkillID];
        switch (SkillID)
        {
            case 0:
                return "一斗的下一次攻击将施展“" +
                       CT.GetColorfulText("荒泷逆袈裟", CT.normalBlue) +
                       "”：在进行2次快速的普通攻击后，一斗高高跃起，举起武器砸向地面" +
                       "，对周围敌人造成" +
                       CT.ChangeToColorfulPercentage(spAttackMulti[lel]) +
                       "攻击力的范围物理伤害。\n\n若处于三技能“怒目鬼王”的状态下，则会造成" +
                       CT.GetColorfulText("岩元素物理", CT.GeoYellow) +
                       "伤害";
            case 1:
                return "一斗将荒泷派编外成员小赤牛" +
                       CT.GetColorfulText("「阿丑」", CT.GeoYellow) +
                       "高速投掷出去，对命中的敌人造成" +
                       CT.ChangeToColorfulPercentage(skill_2_Multi[lel]) +
                       "攻击力的" +
                       CT.GetColorfulText("岩元素物理", CT.GeoYellow) +
                       "伤害。若命中敌人，则一斗的下一次攻击将变为“" +
                       CT.GetColorfulText("荒泷逆袈裟", CT.normalBlue) +
                       "”\n\n若一斗正前方一格没有障碍，" +
                       CT.GetColorfulText("「阿丑」", CT.GeoYellow) +
                       "将在一斗前方停留，阻挡并嘲讽敌人，持续" +
                       CT.GetColorfulText(od_achou.duration0[skillLevel[1]].ToString("f0"), CT.normalBlue) +
                       "秒。阿丑可以被敌人攻击，且拥有较高的受击优先级" +
                       "和较小的碰撞体积。在持续时间结束或生命值耗尽后退场";
            default:
                return "一斗化身“怒目鬼王”，召唤鬼王金碎棒进行战斗。\n在该状态下，一斗的所有伤害变为" +
                       CT.GetColorfulText("岩元素物理", CT.GeoYellow) +
                       "伤害，且攻击范围" +
                       CT.GetColorfulText("扩大", CT.normalBlue) +
                       "，攻击间隔" +
                       CT.GetColorfulText("缩短", CT.normalBlue) +
                       "，攻击力+" +
                       CT.ChangeToColorfulPercentage(skill_3_atkBuff[lel]) +
                       "，防御力" +
                       CT.GetColorfulText("-25%", CT.normalRed) +
                       "，对攻击范围内的所有敌人造成伤害\n\n" +
                       "“怒目鬼王”状态下，一斗每进行4次普通攻击，下一次攻击将变为“" +
                       CT.GetColorfulText("荒泷逆袈裟", CT.normalBlue) +
                       "”";
        }
    }
    
    public override string GetTalentDescription(int talentID)
    {
        if (talentID == 1)
        {
            return "一斗释放“" +
                   CT.GetColorfulText("荒泷逆袈裟", CT.normalBlue) +
                   "”后，将获得攻击速度+" +
                   CT.GetColorfulText(talent1_atkSpeedRate.ToString("f0")) +
                   "的效果，持续" +
                   CT.GetColorfulText(talent1_duration.ToString("f0")) +
                   "秒。该效果可以叠加，每层效果单独计时";
        }
        else
        {
            if (eliteLevel < 2) return "";
            return "当一斗处于结晶盾庇护状态下时，一斗的攻击力固定提升，数值等于防御力的" +
                   CT.GetColorfulText("35%", CT.normalBlue);
        }
    }
    
    public override string GetSkillAtkRangeName(int SkillID)
    {
        return SkillID switch
        {
            2 => skill3AtkRange.name,
            _ => ""
        };
    }
    
}

public class SkillAtkRangeBuff : SkillBuffSlot
{
    private GameObject atkRange;
    private OperatorCore oc_;
    private bool SwitchAtkDrone;

    public SkillAtkRangeBuff(OperatorCore oc, GameObject range, bool switchAtkDrone = false) : base(oc)
    {
        atkRange = range;
        oc_ = oc;
        SwitchAtkDrone = switchAtkDrone;
    }

    public override void BuffStart()
    {
        base.BuffStart();
        oc_.ChangeAtkRange(atkRange, SwitchAtkDrone);
    }

    public override void BuffUpdate() { }

    public override void BuffEnd()
    {
        base.BuffEnd();
        oc_.ChangeAtkRange(SwitchAtkDrone);
    }
}

