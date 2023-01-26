using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ganyu : OperatorCore
{
    [Header("甘雨的特效")] 
    public GameObject norArrow;
    public GameObject norHitAnim;
    public GameObject frostFlaskArrow;
    public GameObject frostFlaskBoomAnim;
    public GameObject frostFlaskHit;
    public GameObject skill3_AtkRange;
    public GameObject PowerUp;
    
    // 这些是实例化过的，直接用
    public IceLotus Lotus;
    public GameObject IcePearl;
    public GameObject Skill3_ShowingRange;
    public GameObject Ghost;
    public List<fallingIceShard> iceShardList;
    public AudioClip norAtkAudio;
    public AudioClip chargeAudio;
    public AudioClip frostOutAudio;
    public AudioClip frostBoomAudio;
    public AudioClip lotusOutAudio;
    public AudioClip lotusBoomAudio;
    public AudioClip skill3StartAudio;


    private float[] frostFlask_firstMulti = {1, 1.1f, 1.2f, 1.35f, 1.5f, 1.65f, 1.8f, 2f};
    private float[] frostFlask_boomMulti = {2f, 2.2f, 2.4f, 2.7f, 3f, 3.3f, 3.6f, 4f};
    public float[] skill2_Multi { get; private set; } = {0.8f, 0.9f, 1f, 1.15f, 1.3f, 1.5f, 1.8f};
    public float[] iceLotusLifeMulti { get; private set; } = {0.6f, 0.7f, 0.85f, 1f, 1.15f, 1.3f, 1.5f};
    private float iceLotusDration = 15f;
    public float[] skill3_Multi { get; private set; } = {1f, 1.2f, 1.4f, 1.6f, 1.8f, 2f, 2.3f};
    public float skill3_DamInc { get; private set; } = 0.1500001f;

    private float frostFlask_Radius = 1.5f;
    private int talent1_spMin = 10;
    private int talent1_spMax = 20;
    private float talent2_rate = 0.2f;
    
    [HideInInspector] public List<Vector2> posList_3x4;
    [HideInInspector] public List<Vector2> posList_Skill3;
    
    // 随机落冰逻辑
    [HideInInspector] public ElementTimer atkTimer;
    public float iceShardDuration { get; private set; } = 0.2f;    // 落冰间隔 
    public float iceShardRadius { get; private set; } = 0.2f;       // 落冰半径

    protected override void Awake_Core()
    {
        base.Awake_Core();
        atkTimer = new ElementTimer(this, 0.9f);
    }

    protected override void Start_Core()
    {
        base.Start_Core();
        posList_3x4 = SearchAndGive.atkRangePos["3x4"];
        posList_Skill3 = SearchAndGive.atkRangePos["火山"];
    }

    public override void OnStart()
    {
        base.OnStart();
        // 天赋1，部署回复技力
        sp_.GetSp(Random.Range(talent1_spMin, talent1_spMax));
    }

    public override void OperInit()
    {
        base.OperInit();
        
        IcePearl.SetActive(false);
        Skill3_ShowingRange.SetActive(false);
        atkTimer.Clear();
    }

    public override void OnAttack()
    {
        sp_.GetSp_Atk();
        Archery(1f, norArrow, norAttack);
        AudioManager.PlayEFF(norAtkAudio, 0.6f);

        if (skillNum == 0 && sp_.outType == releaseType.atk && sp_.CanReleaseSkill())
        {
            anim.SetBool("sp", true);
        }
    }

    public override void SkillAtk_1()
    {
        base.SkillAtk_1();
        sp_.ReleaseSkill();
        anim.SetBool("sp", false);
        Archery(frostFlask_firstMulti[skillLevel[0]], frostFlaskArrow, frostFlaskAttack);
        AudioManager.PlayEFF(frostOutAudio);
    }

    private void Archery(float multi, GameObject proArrow,
        Action<float, BattleCore, parabola, bool> endAttack, bool noTarget = false, Vector3 tarPos = default)
    {// 射一支箭出去，攻击倍率为multi
        var arrow = PoolManager.GetObj(proArrow);
        parabola par = arrow.GetComponent<parabola>();
        
        Vector3 pos = transform.position;
        pos += new Vector3(ac_.dirRight ? 0.25f : -0.25f, 0.35f, 0.36f);
        if (!noTarget)
        {
            par.Init(pos, this, target, 12f, endAttack, multi);
        }
        else
        {
            par.Init(pos, this, 12f, endAttack, multi, tarPos);
        }
    }
    
    private void norAttack(float multi, BattleCore tarBC, parabola par, bool tarIsNull)
    {
        if (tarIsNull) return;
        
        GameObject hitAnim = PoolManager.GetObj(norHitAnim);
        hitAnim.transform.parent = tarBC.transform;
        Vector3 pos = tarBC.animTransform.localPosition + new Vector3(0, 0, 0.3f);
        hitAnim.transform.localPosition = pos;
        DurationRecycleObj recycleObj = new DurationRecycleObj(hitAnim, 1f, tarBC, true);
        BuffManager.AddBuff(recycleObj);
        
        Battle(tarBC, atk_.val, DamageMode.Physical);
    }
    
    private void frostFlaskAttack(float multi, BattleCore tarBC, parabola par, bool tarIsNull)
    {
        // 开霜华矢爆炸动画
        GameObject frostFlask = PoolManager.GetObj(frostFlaskBoomAnim);
        Vector3 center = par.transform.position;
        center.y = 0;
        if (!tarIsNull) center = tarBC.transform.position;
        frostFlask.transform.localPosition = center;
        DurationRecycleObj recycleObj = new DurationRecycleObj(frostFlask, 1f);
        BuffManager.AddBuff(recycleObj);
        
        StartCoroutine(frostFlaskBoom(center));

        if (tarIsNull) return;
        
        float critRate = 0;     // 暴击
        if (eliteLevel >= 2)
        {
            if (tarBC.IsAttachElement(ElementType.Frozen)) critRate = 3 * talent2_rate;
            else if (tarBC.IsAttachElement(ElementType.Cryo)) critRate = 2 * talent2_rate;
            else critRate = talent2_rate;
        }
        bool crit = Random.Range(0f, 1f) <= critRate;
        
        ElementSlot cryoSlot = new ElementSlot(ElementType.Cryo, 1f);
        Battle(tarBC, atk_.val * multi * (crit ? 2 : 1), DamageMode.Physical,
            cryoSlot, true, true, crit);
    }

    IEnumerator frostFlaskBoom(Vector3 center)
    {
        yield return new WaitForSeconds(0.3f);
        AudioManager.PlayEFF(frostBoomAudio);
        yield return new WaitForSeconds(0.2f);

        ElementSlot cryoSlot = new ElementSlot(ElementType.Cryo, 1f);
        var tars = InitManager.GetNearByEnemy(center, frostFlask_Radius);
        float dam = atk_.val * frostFlask_boomMulti[skillLevel[0]];
        foreach (var EC in tars)
        {
            GameObject hitAnim = PoolManager.GetObj(frostFlaskHit);
            hitAnim.transform.parent = EC.transform;
            Vector3 pos = EC.animTransform.localPosition + new Vector3(0, 0, 0.3f);
            hitAnim.transform.localPosition = pos;
            DurationRecycleObj recycleObj = new DurationRecycleObj(hitAnim, 1f, EC, true);
            BuffManager.AddBuff(recycleObj);
            
            float critRate = 0;     // 暴击
            if (eliteLevel >= 2)
            {
                if (EC.IsAttachElement(ElementType.Frozen)) critRate = 3 * talent2_rate;
                else if (EC.IsAttachElement(ElementType.Cryo)) critRate = 2 * talent2_rate;
                else critRate = talent2_rate;
            }
            bool crit = Random.Range(0f, 1f) <= critRate;

            Battle(EC, dam * (crit ? 2 : 1), DamageMode.Physical, cryoSlot, 
                true, true, crit);
        }
    }


    public override void SkillStart_3()
    {
        base.SkillStart_3();
        GanyuSkill3Buff skill3Buff = new GanyuSkill3Buff(this);
        BuffManager.AddBuff(skill3Buff);
        AudioManager.PlayEFF(skill3StartAudio);
    }

    public IEnumerator IcePearlAmplification()
    {
        IcePearl.SetActive(true);
        IcePearl.transform.localScale = Vector3.zero;
        Vector3 tarScale = new Vector3(0.3f, 0.3f, 0.3f);
        while (Mathf.Abs(IcePearl.transform.localScale.x - tarScale.x) > 1e-3f)
        {
            IcePearl.transform.localScale = Vector3.Lerp(
                IcePearl.transform.localScale, tarScale, Time.deltaTime * 3f);
            yield return null;
        }
    }
    
    public IEnumerator IcePearlShrink()
    {
        Vector3 preScale = IcePearl.transform.localScale;
        Vector3 tarScale = Vector3.zero;
        float startTime = Time.time;
        while (Mathf.Abs(IcePearl.transform.localScale.x - tarScale.x) > 1e-3f)
        {
            IcePearl.transform.localScale = Vector3.Lerp(
                preScale, tarScale, (Time.time - startTime) * 3f);
            yield return null;
        }
        
        IcePearl.SetActive(false);
    }


    public override void SkillStart_2()
    {
        base.SkillStart_2();
        anim.SetInteger("sta", 1);
        Ghost.SetActive(true);
        AudioManager.PlayEFF(lotusOutAudio);

        Vector3 tarPos = FindIceLotusTile();
        if (tarPos.y < -1) return;      // 范围内找不到可以部署的地块
        
        // 部署冰莲
        if (Lotus.gameObject.activeSelf) Lotus.Retreat();
        Lotus.gameObject.SetActive(true);
        Lotus.transform.position = tarPos;
        Lotus.anim.transform.localPosition = new Vector3(0, 0, -0.2f);
        Lotus.OperInit();
    }

    private Vector3 FindIceLotusTile()
    {// 在当前攻击范围内，找到一个近战部署位，放置冰莲。优先逻辑参考蜜蜡
        // 首先按优先级遍历攻击范围内的敌人，如果某个敌人脚下是可部署地块，则部署在该地块上
        Vector3 tarPos = new Vector3(-1, -10000, -1);
        foreach (var EC in enemyList)
        {
            Vector3 pos = BaseFunc.x0z(BaseFunc.FixCoordinate(EC.transform.position));
            TileSlot tileSlot = InitManager.GetMap(pos);
            if (Interpreter.canPut(tileSlot.type, false, true))
            {
                tarPos = pos;
                break;
            }
        }
        if (tarPos.y > -1) return tarPos;

        // 如果找不到敌人，则随机部署在一块可部署地块上
        List<Vector3> canPutPosList = new List<Vector3>();
        float rol_y = atkRange.transform.rotation.eulerAngles.y;
        foreach (var detaPos in posList_3x4)
        {
            Vector3 pos = new Vector3();
            if (rol_y == 0)
            {
                pos.x = detaPos.x;
                pos.z = detaPos.y;
            }
            if (rol_y == 270 || rol_y == -90)
            {
                pos.x = -detaPos.y;
                pos.z = detaPos.x;
            }
            if (rol_y == 180)
            {
                pos.x = -detaPos.x;
                pos.z = detaPos.y;
            }
            if (rol_y == 90)
            {
                pos.x = -detaPos.y;
                pos.z = -detaPos.x;
            }
            pos += transform.position;
            pos.y = 0;

            TileSlot tileSlot = InitManager.GetMap(pos);
            if (Interpreter.canPut(tileSlot.type, false, true))
                canPutPosList.Add(BaseFunc.x0z(BaseFunc.FixCoordinate(pos)));
        }

        if (canPutPosList.Count == 0) return new Vector3(0, -10000, 0);
        int id = Random.Range(0, canPutPosList.Count);
        return canPutPosList[id];
    }
    

    public override void SkillAtk_2()
    {
        base.SkillAtk_2();
        anim.SetInteger("sta", 0);

        if (tarIsNull)
        {
            Archery(frostFlask_firstMulti[skillLevel[0]], frostFlaskArrow, frostFlaskAttack,
                            true, Lotus.transform.position);
        }
        else
        {
            Archery(frostFlask_firstMulti[skillLevel[0]], frostFlaskArrow, frostFlaskAttack);
        }
    }

    public override string GetSkillDescription(int SkillID)
    {
        int lel = skillLevel[SkillID];
        switch (SkillID)
        {
            case 0:
                return "甘雨的下一次攻击会进行蓄力，蓄力结束后发射威力强大的" +
                       CT.GetColorfulText("霜华矢") + "，对目标造成" +
                       CT.ChangeToColorfulPercentage(frostFlask_firstMulti[lel]) + "的" +
                       CT.GetColorfulText("冰元素物理", CT.CryoWhite) +
                       "伤害。随后霜华绽放，对一个大范围内的敌人再次造成" +
                       CT.ChangeToColorfulPercentage(frostFlask_boomMulti[lel]) + "的" +
                       CT.GetColorfulText("冰元素物理", CT.CryoWhite) +
                       "伤害";
            case 1:
                return "甘雨施展麒麟的身法，向目标抛射出一朵" +
                       CT.GetColorfulText("冰莲", CT.CryoWhite) +
                       "，在小范围内造成" + CT.ChangeToColorfulPercentage(skill2_Multi[lel]) +
                       "的" + CT.GetColorfulText("冰元素物理", CT.CryoWhite) +
                       "伤害，随后立刻向目标发射一枚" +
                       CT.GetColorfulText("霜华矢") +
                       "\n\n" + CT.GetColorfulText("冰莲", CT.CryoWhite) +
                       "会留在原地，继承甘雨" +
                       CT.ChangeToColorfulPercentage(iceLotusLifeMulti[lel]) +
                       "的生命值，嘲讽并阻拦附近的敌人，持续" +
                       CT.GetColorfulText(iceLotusDration.ToString("f0")) + "秒\n\n" +
                       CT.GetColorfulText("冰莲", CT.CryoWhite) +
                       "在被摧毁或持续时间结束后会绽开，对周围的敌人造成" +
                       CT.ChangeToColorfulPercentage(skill2_Multi[lel]) + "的" +
                       CT.GetColorfulText("冰元素物理", CT.CryoWhite) +
                       "伤害";
            default:
                return "凝聚大气中的霜雪，召唤退魔的冰灵珠，展开" +
                       // CT.GetColorfulText("降众天华领域", CT.CryoWhite) + "\n\n" +
                       CT.GetColorfulText("降众天华领域", CT.CryoWhite) + "：\n" +
                       "·甘雨催动的冰灵珠会持续降下冰棱，在小范围内造成" +
                       CT.ChangeToColorfulPercentage(skill3_Multi[lel]) + "的" +
                       CT.GetColorfulText("冰元素物理", CT.CryoWhite) +
                       "伤害\n·领域内的所有干员获得" +
                       CT.ChangeToColorfulPercentage(skill3_DamInc) +
                       "元素伤害加成";
        }
    }
    
    public override string GetTalentDescription(int talentID)
    {
        if (talentID == 1)
        {
            return "部署后立刻回复一定的技力（在[" +
                   CT.GetColorfulText(talent1_spMin.ToString()) + "," +
                   CT.GetColorfulText(talent1_spMax.ToString()) +
                   "]之间随机取值），不会超过当前最大技力";
        }
        else
        {
            if (eliteLevel < 2) return "";
            return CT.GetColorfulText("霜华矢") +
                   "有" + CT.ChangeToColorfulPercentage(talent2_rate) +
                   "的几率造成" + CT.GetColorfulText("2") +
                   "倍伤害\n·攻击" +
                   CT.GetColorfulText("冰元素附着", CT.CryoWhite) +
                   "下的敌人时，该几率提升" +
                   CT.ChangeToColorfulPercentage(talent2_rate) +
                   "\n·攻击" + CT.GetColorfulText("冻结", CT.CryoWhite) +
                   "状态下的敌人时，该几率额外提升" +
                   CT.ChangeToColorfulPercentage(talent2_rate);
        }
    }
    
    public override string GetSkillAtkRangeName(int SkillID)
    {
        return SkillID switch
        {
            2 => skill3_AtkRange.name,
            _ => ""
        };
    }
}

public class GanyuSkill3Buff : SkillBuffSlot
{
    private Ganyu gy_;
    
    // 范围内元素伤害加成
    private Dictionary<BattleCore, ValueBuffInner> damIncBuffList =
        new Dictionary<BattleCore, ValueBuffInner>();
    private Dictionary<BattleCore, GameObject> damIncObjList =
        new Dictionary<BattleCore, GameObject>();       // 身上的提升标志
    private int preOperListCount = -1;
    
    // 落冰
    private float iceCool;  // 当前冷却
    private int icePointer; // 落冰列表中的当前指针 

    public GanyuSkill3Buff(Ganyu gy) : base(gy)
    {
        gy_ = gy;
    }
    
    public override void BuffStart()
    {
        base.BuffStart();
        
        gy_.anim.SetInteger("sta", 2);
        gy_.ChangeAtkRange(gy_.skill3_AtkRange);
        
        gy_.Skill3_ShowingRange.SetActive(true);

        if (gy_.defaultFaceRight)   // 锁定旋转，默认朝向
        {
            gy_.ac_.LockRolAndRight();
            gy_.IcePearl.transform.localPosition = new Vector3(0.4f, 1f, 1.5f);
        }
        else
        {
            gy_.ac_.LockRolAndLeft();
            gy_.IcePearl.transform.localPosition = new Vector3(-0.3f, 1f, 1.5f);
        }
        gy_.StartCoroutine(gy_.IcePearlAmplification());
        

        gy_.DieAction += Clear;     // 甘雨死时调用，清空范围buff
        gy_.atkTimer.Clear();       // 清空随机落冰计时器
        iceCool = 0.5f;             // 0.5秒后开始落冰
        icePointer = 0;
    }

    public override void BuffUpdate()
    {
        DamIncUpdate();
        FallIceShard();
    }

    private void DamIncUpdate()
    {// 实时更新范围内干员的元素伤害加成
        if (gy_.operatorList.Count == preOperListCount) return;     // 一般来说，数量没变就是没变
        
        // 扫描当前的operatorList，把多的加进去
        foreach (var bc_ in gy_.operatorList)
        {
            if (damIncBuffList.ContainsKey(bc_)) continue;
            
            ValueBuffInner damIncBuff = new ValueBuffInner(ValueBuffMode.Fixed, gy_.skill3_DamInc);
            bc_.elementDamage.AddValueBuff(damIncBuff);
            damIncBuffList.Add(bc_, damIncBuff);

            GameObject powerUp = PoolManager.GetObj(gy_.PowerUp);
            powerUp.transform.SetParent(bc_.transform);
            powerUp.transform.localPosition = new Vector3(0, 0, -0.2f);
            damIncObjList.Add(bc_, powerUp);
        }
        
        // 扫描当前damIncBuffList，把多的扔掉
        List<BattleCore> delBCList = new List<BattleCore>();
        foreach (var pp in damIncBuffList)
        {
            if(!gy_.operatorList.Contains(pp.Key))
                delBCList.Add(pp.Key);
        }
        foreach (var bc_ in delBCList)
        {
            bc_.elementDamage.DelValueBuff(damIncBuffList[bc_]);
            damIncBuffList.Remove(bc_);
            
            PoolManager.RecycleObj(damIncObjList[bc_]);
            damIncObjList.Remove(bc_);
        }
        preOperListCount = gy_.operatorList.Count;
    }

    private void FallIceShard()
    {// 随机落冰攻击
        iceCool -= Time.deltaTime;
        if (iceCool > 0) return;
        iceCool = gy_.iceShardDuration;

        int id = -1;
        for (int i = 0; i < 3 && id == -1; i++)
        {// 随机找三次
            id = RandomFindTarget();
        }

        Vector3 center = Vector3.zero;
        if (id != -1)
        {// 算出敌人在0.2秒后的位置
            EnemyCore ec_ = (EnemyCore) gy_.enemyList[id];
            center = ec_.epc_.ExpectPos(0.2f);
        }
        else
        {// 范围内随机落冰
            int pid = Random.Range(0, gy_.posList_Skill3.Count);
            Vector2 tmp = gy_.posList_Skill3[pid];
            float dx = Random.Range(-0.3f, 0.3f);
            float dy = Random.Range(-0.3f, 0.3f);
            center = new Vector3(tmp.x + dx, 0, tmp.y + dy);
            center += gy_.transform.position;
        }
        
        gy_.iceShardList[icePointer].transform.position = center;
        if (id == -1) gy_.iceShardList[icePointer].Init(null, true);
        else gy_.iceShardList[icePointer].Init(gy_.enemyList[id], false);
        icePointer = (icePointer + 1) % gy_.iceShardList.Count;
    }

    private int RandomFindTarget()
    {// 随机在enemyList里找落冰目标，如果成功返回下标，否则返回-1
        int cnt = gy_.enemyList.Count;
        if (cnt == 0) return -1;
        int id = Random.Range(0, cnt);
        // return id;
        if (gy_.atkTimer.AttachElement(gy_.enemyList[id])) return id;
        return -1;
    }
    

    public override void BuffEnd()
    {
        base.BuffEnd();
        Clear(gy_);
        gy_.DieAction -= Clear;


        if (gy_.gameObject.activeSelf) gy_.StartCoroutine(gy_.IcePearlShrink());
        else gy_.IcePearl.SetActive(false);
    }


    private void Clear(BattleCore bc_)
    {
        foreach (var pp in damIncBuffList)
        {
            pp.Key.elementDamage.DelValueBuff(pp.Value);
        }
        damIncBuffList.Clear();

        foreach (var pp in damIncObjList)
        {
            PoolManager.RecycleObj(pp.Value);
        }
        damIncObjList.Clear();
        
        foreach (var i in gy_.iceShardList)
        {
            i.gameObject.SetActive(false);
        }
        
        gy_.Skill3_ShowingRange.SetActive(false);
        gy_.ac_.UnLockRol();
        gy_.anim.SetInteger("sta", 0);
        gy_.ChangeAtkRange();
    }
}