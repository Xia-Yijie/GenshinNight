using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using Spine;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class OperatorCore : BattleCore
{
    [Header("干员数据")]
    public operData od_;

    [HideInInspector] public int level;         // 干员等级
    [HideInInspector] public int eliteLevel;    // 干员精英化等级
    [HideInInspector] public int[] skillLevel = new int[3];    // 干员技能等级[0:6]
    [HideInInspector] public ValueBuffer costNeed = new ValueBuffer();      // 干员当前部署费用
    [HideInInspector] private int costIncCount = 2;    // 再部署增加费用最多两次
    [HideInInspector] public int operID;        // 在InitManager的offOperList里的编号
    [HideInInspector] public ValueBuffer recoverTime = new ValueBuffer(0);   // 干员再部署时间
    [HideInInspector] public int skillNum;      // 该干员选择的技能编号[0,2]
    [HideInInspector] public FourDirection direction;       // 默认的放置方向
    [HideInInspector] public Vector3 directionVector;       // 默认放置方向的正前方方向向量
    [HideInInspector] public bool defaultFaceRight;
    
    public bool prePutOn;           // 一开始就在场上的
    public bool isSummoner;         // 是召唤物
    
    // spine动画相关
    [HideInInspector] public GameObject animObject;
    [HideInInspector] public Animator anim;
    public SpineAnimController ac_;
    private int fightingContinue = 0;       // fight激活后延续几帧

    // atkRange相关
    [HideInInspector] public SearchAndGive atkRange;        // 当前atkRange的脚本
    private Action defaultTurn;         // 干员朝默认方向旋转的函数
    private GameObject underArrow;

    // 阻挡相关
    public int block { get; private set; }      // 当前剩余可用阻挡数
    private Dictionary<EnemyCore, bool> alreadyBlockSet = new Dictionary<EnemyCore, bool>();
    
    // 眩晕相关
    private GameObject dizzyStarts;

    // 入场委托函数，在OperInit中被调用，不会清空
    [HideInInspector] public Action<OperatorCore> OperInitAction;
    
    // 低血量语音
    private float lowHP_DeadLine = 0.2f;
    private float lowHP_MaxCoolTime = 10f;
    private bool notLowHP = true;
    private float lowHP_CoolTime;
    
    protected override void Awake_Core()
    {
        base.Awake_Core();
        animObject = transform.Find("anim").gameObject;
        anim = animObject.GetComponent<Animator>();
        animTransform = anim.transform;
        ac_ = new SpineAnimController(anim, this);

        aimingMode = od_.aimingMode;
        InitCalculation();
        costNeed.ChangeBaseValue(od_.cost);
        ChangeAtkRange();       // 生成atkRange
    }
    

    protected override void Start_Core()
    {
        base.Start_Core();
        
        // 干员脚下箭头初始化
        underArrow = Instantiate(StoreHouse.instance.underArrow, transform);
        underArrow.transform.localPosition = new Vector3(0, 0.001f, 0);
        
        ac_.ChangeDefaultColorImmediately();
        frozen_Inc_DecSpeed = 0;            // 干员的被冻结时间不会随之延长

        if (prePutOn && !isSummoner)
        {
            OperInit();
            return;
        }
        
        DieAction?.Invoke(this);
        DieAction = null;
        gameObject.SetActive(false);
    }

    protected override void Update_Core()
    {
        base.Update_Core();
        
        Dizzy();
        if (!dying) Fight();
        CheckBlock();
        AutoSkill();
        LowHpMonitor();

        ac_.Update();
    }

    public virtual void OperInit()
    {// 在每次登场时的初始化函数，用于初始化本OperatorCore

        // 重设生命值
        life_.RecoverCompletely();
        // 消除属性们的buff
        ClearCalculation();      
        
        // 初始化元素附着相关
        foreach (var timer in timerList) timer.Clear();
        attachedElement.Clear();
        reactionController.Init();
        
        // 根据当前朝向设定默认朝向
        if (anim.transform.localScale.x > 0)
        {
            defaultTurn = ac_.TurnRight;
            defaultFaceRight = true;
        }
        else
        {
            defaultTurn = ac_.TurnLeft;
            defaultFaceRight = false;
        }
        underArrow.SetActive(!isSummoner);
        underArrow.transform.localEulerAngles = new Vector3(90f, 0, -atkRange.transform.localEulerAngles.y);
        
        switch (atkRange.transform.localEulerAngles.y)
        {// 初始化默认方向
            case 0:
                direction = FourDirection.Right;
                directionVector = new Vector3(1, 0, 0);
                break;
            case 90:
                direction = FourDirection.Down;
                directionVector = new Vector3(0, 0, -1);
                break;
            case 180:
                direction = FourDirection.Left;
                directionVector = new Vector3(-1, 0, 0);
                break;
            case -90:
            case 270:
                direction = FourDirection.UP;
                directionVector = new Vector3(0, 0, 1);
                break;
            default:
                direction = FourDirection.None;
                directionVector = Vector3.zero;
                break;
        }
        
        ac_.UnLockRol();

        // 让Anim开始播放动画
        anim.SetBool("start", true);
        
        // 初始化死亡调用函数
        DieAction += DelFromOperList;       // 将自身从InitManager的OperList中移除
        DieAction += DelAllBlockedEnemy;    // 释放阻挡的所有敌人
        
        // 初始化阻挡相关
        block = (int) maxBlock.val;
        blockList.Clear();
        
        // 初始化被攻击优先级
        tarPriority = InitManager.GetAndAddOperPriority();
        
        // 初始化是否可以打无人机
        canAtkDrone = od_.canAtkDrone;
        
        // 像InitManager里注册
        InitManager.operList.Add(this);
        
        // 改变脚下tile的类型
        TileSlot tile = InitManager.GetMap(transform.position);
        if (tile != null)
        {
            OperPut_TileType_Buff tileBuff = new OperPut_TileType_Buff(this, tile); 
            BuffManager.AddBuff(tileBuff);
        }
        
        // 入场委托函数
        OperInitAction?.Invoke(this);
        
        // 根据选择的技能设置spController
        int lel = skillLevel[skillNum];
        switch (skillNum)
        {
            case 0:
                sp_.Init(this, od_.initSP0[lel], od_.maxSP0[lel], od_.duration0[lel],
                    od_.skill0_recoverType, od_.skill0_releaseType, od_.spRecharge);
                break;
            case 1:
                sp_.Init(this, od_.initSP1[lel], od_.maxSP1[lel], od_.duration1[lel],
                    od_.skill1_recoverType, od_.skill1_releaseType, od_.spRecharge);
                break;
            case 2:
                sp_.Init(this, od_.initSP2[lel], od_.maxSP2[lel], od_.duration2[lel],
                    od_.skill2_recoverType, od_.skill2_releaseType, od_.spRecharge);
                break;
        }
    }

    private void InitCalculation()
    {
        atk_.Init(od_.atk);
        def_.Init(od_.def);
        magicDef_.Init(od_.magicDef);
        life_.InitBaseLife(od_.life);
        maxBlock.Init(od_.maxBlock);
        atkSpeedController = new AtkSpeedController(this, ac_, 0, od_.maxAtkInterval);

        elementMastery.Init(od_.elementalMastery);
        elementDamage.Init(od_.elementalDamage);
        elementResistance.Init(od_.elementalResistance);
        shieldStrength.Init(od_.shieldStrength);
        sp_.spRecharge.Init(1);
        
        recoverTime.Init(od_.reTime);
    }
    
    private void ClearCalculation()
    {
        atk_.ClearBuff();
        def_.ClearBuff();
        magicDef_.ClearBuff();
        life_.ClearBuff();
        maxBlock.ClearBuff();
        atkSpeedController = new AtkSpeedController(this, ac_, 0, od_.maxAtkInterval);

        elementMastery.ClearBuff();
        elementDamage.ClearBuff();
        elementResistance.ClearBuff();
        shieldStrength.ClearBuff();
        sp_.spRecharge.ClearBuff();
        
        recoverTime.ClearBuff();
    }

    private void Dizzy()
    {
        if (dizziness > 0)
        {
            anim.SetBool("down", true);
        }
        else
        {
            anim.SetBool("down", false);
        }
    }

    private void LowHpMonitor()
    {
        lowHP_CoolTime -= Time.deltaTime;       // 每帧调用，冷却时间正常减少
        
        if (life_.life / life_.val > lowHP_DeadLine || dying)
        {// 如果不在低血量线下
            notLowHP = true;
            return;
        }
        
        if (!notLowHP) return;                  // 如果一直在低血量线下
        notLowHP = false;
        
        if (lowHP_CoolTime > 0) return;         // 如果还在冷却

        // 此时可播放低血量语音
        if (od_.LowHP.Count == 0) return;
        int id = Random.Range(0, od_.LowHP.Count);
        AudioManager.OperatorTalk(od_.LowHP[id]);
    }
    
    private void Fight()
    {
        if (dizziness > 0) return;
        var staInfo = anim.GetCurrentAnimatorStateInfo(0);
        // fighting = anim.GetBool("fight");
        if (staInfo.IsName("Fight"))
        {
            fighting = true;
        }
        else fighting = false;
        
        if (!tarIsNull)
        {
            if (CanAtk() && !fighting)
            {
                anim.SetBool("fight", true);
                // fightingContinue = (int) (3 / Time.timeScale);
                // NorAtkStartCool();
                
                // 根据目标位置转变干员朝向
                Vector2 detaPos = BaseFunc.xz(transform.position) - BaseFunc.xz(target.transform.position);
                if (detaPos.x < 0) ac_.TurnRight();
                else ac_.TurnLeft();
            }
            // else if (fightingContinue > 0)
            // {
            //     fightingContinue--;
            // }
            // else
            // {
            //     anim.SetBool("fight", false);
            // }
        }
        else
        {
            // anim.SetBool("fight", false);
            defaultTurn();
        }
    }

    private void AutoSkill()
    {// 自动触发技能判定
        if (sp_.outType != releaseType.auto || !sp_.CanReleaseSkill()) return;
        sp_.ReleaseSkill();
        switch (skillNum)
        {
            case 0: SkillStart_1();
                break;
            case 1:SkillStart_2();
                break;
            case 2:SkillStart_3();
                break;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("enemy")) return;
        BattleCore battleCore = other.GetComponent<BattleCore>();
        blockList.Add(battleCore);
        battleCore.DieAction += DelBattleCore_OperBlock;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("enemy")) return;
        BattleCore battleCore = other.GetComponent<BattleCore>();
        battleCore.DieAction -= DelBattleCore_OperBlock;
        DelBattleCore_OperBlock(battleCore);
    }

    // 敌人死亡时，处理block相关的回调函数
    private void DelBattleCore_OperBlock(BattleCore bc_)
    {
        blockList.Remove(bc_);
        EnemyCore ec_ = (EnemyCore)bc_;
        if (alreadyBlockSet.ContainsKey(ec_) && alreadyBlockSet[ec_])
        {
            block += (int) ec_.maxBlock.val;
            alreadyBlockSet[ec_] = false;
            actuallyBlockList.Remove(ec_);
            ec_.UnBlocked(this);
        }
    }

    // 释放所有被自己阻挡的敌人，自身阻挡数更新为最大阻挡数
    private void DelAllBlockedEnemy(BattleCore bc_)
    {
        for (int i = 0; i < alreadyBlockSet.Count; i++)
        {
            var tmp = alreadyBlockSet.ElementAt(i);
            EnemyCore ec_ = tmp.Key;
            if (alreadyBlockSet.ContainsKey(ec_) && alreadyBlockSet[ec_])
            {
                alreadyBlockSet[ec_] = false;
                actuallyBlockList.Remove(ec_);
                ec_.UnBlocked(this);
            }
        }
        alreadyBlockSet.Clear();
        actuallyBlockList.Clear();

        block = (int) maxBlock.val;
    }

    void CheckBlock()
    {
        foreach (var i in blockList)
        {
            EnemyCore ec_ = (EnemyCore)i;
            if (alreadyBlockSet.ContainsKey(ec_) && alreadyBlockSet[ec_]) continue;
            
            if (ec_.maxBlock.val <= block)
            {
                ec_.BeBlocked(this);
                block -= (int) ec_.maxBlock.val;
                if (!alreadyBlockSet.ContainsKey(ec_)) actuallyBlockList.Add(ec_);
                alreadyBlockSet[ec_] = true;
            }
        }
    }


    /// <summary>
    /// 摧毁旧的atkRange，生成一个新的atkRange，同时更新block的值
    /// </summary>
    public void ChangeAtkRange(GameObject NewRange, bool switchAtkDrone = false)
    {
        // 记录当前的rotation，更换后保持不变
        Quaternion rol;
        rol = Quaternion.Euler(0, 0, 0);
        if (atkRange != null) rol = atkRange.transform.rotation;

        while (operatorList.Count > 0)
        {
            atkRange.pretendExit(operatorList[0].gameObject);
        }
        while (enemyList.Count > 0)
        {
            atkRange.pretendExit(enemyList[0].gameObject);
        }

        if (atkRange != null) Destroy(atkRange.gameObject);
        if (switchAtkDrone) canAtkDrone = !canAtkDrone;

        GameObject newRange = Instantiate(NewRange, transform, true);
        newRange.transform.localPosition=Vector3.zero;
        newRange.transform.rotation = rol;
        atkRange = newRange.GetComponent<SearchAndGive>();

        // 初始化当前阻挡数
        DelAllBlockedEnemy(null);
    }
    
    /// <summary>
    /// 生成当前精英化等级的atkRange
    /// </summary>
    public void ChangeAtkRange(bool switchAtkDrone = false)
    {
        ChangeAtkRange(od_.atkRange[eliteLevel], switchAtkDrone);
    }
    
    /// <summary>
    /// 干员撤退函数（普通撤退，不是死亡）
    /// </summary>
    public void Retreat()
    {
        
        if (DieAction != null)
        {
            DieAction(this);
            DieAction = null;
        }

        if (costIncCount > 0)
        {
            costIncCount--;
            ValueBuffInner costBuff = new ValueBuffInner(ValueBuffMode.Percentage, 0.5f);
            costNeed.AddValueBuff(costBuff);
        }

        OperUIManager.CloseOperUI();
        if (!prePutOn)
        {
            InitManager.offOperList[operID].Add(this);
            InitManager.dragSlotController.RefreshDragSlot();
            InitManager.resourceController.CostIncrease(od_.cost / 2);
            InitManager.resourceController.RemainPlaceIncrease(od_.consumPlace);
        }
        
        Clears();

        transform.position = new Vector3(999, 999, 999);
        anim.CrossFade("default", 0, 0, 0);
        underArrow.SetActive(false);
        gameObject.SetActive(false);
        
        // 播放撤退音效
        if (!isSummoner) OperUIElements.RetreatAudio.Play();
    }

    protected override void DieBegin()
    {// 死亡撤退函数
        anim.transform.parent = null;
        transform.position = new Vector3(999, 999, 999);
        if (costNeed.val + costNeed.baseVal / 2 <= costNeed.baseVal * 2)
        {
            ValueBuffInner costBuff = new ValueBuffInner(ValueBuffMode.Percentage, 0.5f);
            costNeed.AddValueBuff(costBuff);
        }
        // costNeed = costNeed + od_.cost / 2 > od_.cost * 2 ? od_.cost * 2 : costNeed + od_.cost / 2;
        
        OperUIManager.CloseOperUI();
        if (!prePutOn)
        {
            InitManager.offOperList[operID].Add(this);
            InitManager.dragSlotController.RefreshDragSlot();
            InitManager.resourceController.RemainPlaceIncrease(od_.consumPlace);
        }

        Clears();

        underArrow.SetActive(false);
        anim.SetBool("die", true);
        ac_.ChangeColor(Color.black);
        
        // 播放死亡语音
        if (isSummoner) return;
        OperUIElements.DieAudio.Play();
        if (od_.DieAudio.Count == 0) return;
        int id = Random.Range(0, od_.DieAudio.Count);
        AudioManager.OperatorTalk(od_.DieAudio[id]);
    }

    private void Clears()
    {
        operatorList.Clear();
        enemyList.Clear();
        atkRange.Clear();
        InitCalculation();

        ac_.TurnRightImm();
    }
    
    private void DelFromOperList(BattleCore bc)
    {
        InitManager.operList.Remove(this);
    }

    public override bool GetDizzy()
    {
        if (!base.GetDizzy()) return false;
        if (dizziness > 1) return true;
        Vector3 pos = new Vector3(0, 0, 1);
        dizzyStarts = PoolManager.GetObj(StoreHouse.instance.dizzyStarts);
        dizzyStarts.transform.SetParent(transform);
        dizzyStarts.transform.localPosition = pos;
        return true;
    }

    public override void RevokeDizzy()
    {
        base.RevokeDizzy();
        if (dizziness <= 0) PoolManager.RecycleObj(dizzyStarts);
    }

    /// <summary>
    /// 点击干员时调用的函数
    /// </summary>
    public void OnClick()
    {
        InitManager.TimeSlowPick(transform);
        OperUIManager.OpenOperUI(UIstate.UP, this);
        
        // 播放选中声音
        if (od_.Selected.Count == 0) return;
        int id = Random.Range(0, od_.Selected.Count);
        AudioManager.OperatorTalk(od_.Selected[id]);
    }
    
    public virtual void OnStart() {}
    
    public virtual void OnAttack()
    {
        Battle(target, atk_.val);
    }

    public void OnDie()
    {
        ac_.ChangeDefaultColorImmediately();
        anim.transform.parent = transform;
        dying = false;
        gameObject.SetActive(false);
    }


    public virtual void SkillStart_1()
    {// 默认播放声音
        if (od_.Skill1_Release.Count > 0)
        {
            int num = Random.Range(0, od_.Skill1_Release.Count);
            AudioManager.OperatorTalk(od_.Skill1_Release[num]);
        }
    }

    public virtual void SkillStart_2()
    {// 默认播放声音
        if (od_.Skill2_Release.Count > 0)
        {
            int num = Random.Range(0, od_.Skill2_Release.Count);
            AudioManager.OperatorTalk(od_.Skill2_Release[num]);
        }
    }

    public virtual void SkillStart_3()
    {// 默认播放声音
        if (od_.Skill3_Release.Count > 0)
        {
            int num = Random.Range(0, od_.Skill3_Release.Count);
            AudioManager.OperatorTalk(od_.Skill3_Release[num]);
        }
    }
    
    public virtual void SkillAtk_1() { }
    public virtual void SkillAtk_2() { }
    public virtual void SkillAtk_3() { }

    public virtual string GetTalentDescription(int talentID)
    {// 返回天赋的描述
        return "";
    }
    
    public virtual string GetSkillDescription(int SkillID)
    {// 返回各个技能的描述
        return SkillID == 0 ? od_.description0[skillLevel[0]] :
            SkillID == 1 ? od_.description1[skillLevel[1]] : od_.description2[skillLevel[2]];
    }
    
    public virtual string GetSkillAtkRangeName(int SkillID)
    {// 返回各个技能的范围名称，可在SearchAndGive.atkRangePos中找到List
        return "";
    }
    
    public virtual void ElitismAction0_1() { }      // 精0到精1时会调用的函数 
    
    public virtual void ElitismAction1_2() { }      // 精1到精2时会调用的函数
}

public class SpineAnimController
{
    private const float turnSpeed = 10;
    private const float colorSpeed = 5;

    public Animator anim;
    private BattleCore prtBattleCore;
    private MeshRenderer meshRenderer;
    
    public bool dirRight { get; private set; }   //模型是否朝右
    private Vector3 tarScale;
    private Vector3 leftScale;
    private Vector3 rightScale;
    
    private Action colorAction;
    private bool colorChanging = false;
    private Color tarColor = new Color(1, 1, 1, 1);
    private Color nowColor = new Color(1, 1, 1, 1);
    private static readonly Color defaultColor = new Color(1, 1, 1, 1);
    
    // anim速度相关
    public float atkSpeed = 1;      // 攻速相关的anim速度改变
    public float slowSpeed = 1;     // 减速相关的anim速度改变（敌人）

    // 锁定旋转
    public bool lockRol;


    public SpineAnimController(Animator animator, BattleCore bc_)
    {
        anim = animator;
        prtBattleCore = bc_;
        meshRenderer = anim.GetComponent<MeshRenderer>();
        Vector3 scale = anim.transform.lossyScale;
        leftScale = new Vector3(-scale.x, scale.y, scale.z);
        rightScale = new Vector3(scale.x, scale.y, scale.z);
        tarScale = rightScale;
    }

    public void Update()
    {
        // 该函数需在其它函数的Update里调用才会生效

        // 旋转
        if(!prtBattleCore.dying)
            anim.transform.localScale =
                Vector3.Lerp(anim.transform.localScale, tarScale, turnSpeed * Time.deltaTime);
        
        ColorAnim();
        ChangeAnimSpeed();
    }
    
    

    private void ColorAnim()
    {
        if (colorChanging)
        {
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            
            nowColor = Color.Lerp(nowColor, tarColor, colorSpeed * Time.deltaTime);
            mpb.SetColor("_Color", nowColor);
            meshRenderer.SetPropertyBlock (mpb);
            
            float det = nowColor.r - tarColor.r + nowColor.b - tarColor.b + 
                nowColor.g - tarColor.g + nowColor.a - tarColor.a;
            if (Math.Abs(det) < 0.01)
            {
                colorChanging = false;
                colorAction?.Invoke();
            }
        }
    }

    /// <summary>
    /// 根据当前状态决定animspeed
    /// </summary>
    public void ChangeAnimSpeed()
    {
        if (prtBattleCore.frozen) anim.speed = 1e-6f;
        else if (prtBattleCore.fighting) anim.speed = atkSpeed;
        else anim.speed = slowSpeed;
    }

    public float GetAnimSpeed()
    {
        return anim.speed;
    }
    
    /// <summary>
    /// 调用一次，动画将旋转面向左边
    /// </summary>
    public void TurnLeft()
    {
        if (lockRol) return;
        dirRight = false;
        tarScale = leftScale;
    }

    /// <summary>
    /// 调用一次，动画将旋转面向右边
    /// </summary>
    public void TurnRight()
    {
        if (lockRol) return;
        dirRight = true;
        tarScale = rightScale;
    }
    
    public void TurnRightImm()
    {
        dirRight = true;
        anim.transform.localScale = rightScale;
    }

    public void LockRolAndRight()
    {
        lockRol = true;
        dirRight = true;
        tarScale = rightScale;
    }
    
    public void LockRolAndLeft()
    {
        lockRol = true;
        dirRight = false;
        tarScale = leftScale;
    }

    public void UnLockRol()
    {
        lockRol = false;
    }
    
    
    
    /// <summary>
    /// 调用一次，人物将进行一次闪烁，目标颜色为color
    /// </summary>
    public void TwinkColor(Color color)
    {
        tarColor = color;
        colorAction += colorAction_Recover;
        colorChanging = true;
    }

    private void colorAction_Recover()
    {
        tarColor = defaultColor;
        colorAction -= colorAction_Recover;
        colorChanging = true;
    }
    
    /// <summary>
    /// 调用一次，人物颜色将渐变为目标颜色为color
    /// </summary>
    public void ChangeColor(Color color)
    {
        tarColor = color;
        colorChanging = true;
    }
    
    /// <summary>
    /// 调用一次，人物颜色将渐变为默认颜色
    /// </summary>
    public void ChangeDefaultColor()
    {
        tarColor = defaultColor;
        colorChanging = true;
    }
    
    /// <summary>
    /// 调用一次，人物颜色将立刻恢复为默认颜色
    /// </summary>
    public void ChangeDefaultColorImmediately()
    {
        tarColor = defaultColor;
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        mpb.SetColor("_Color", tarColor);
        meshRenderer.SetPropertyBlock (mpb);
    }
}


