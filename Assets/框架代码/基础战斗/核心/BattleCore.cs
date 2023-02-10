using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleCore : ElementCore
{
    [HideInInspector] public List<BattleCore> operatorList = new List<BattleCore>();
    [HideInInspector] public List<BattleCore> enemyList = new List<BattleCore>();
    [HideInInspector] public AimingMode aimingMode;
    [HideInInspector] public float tarPriority = 0;         // 在别的BattleCore队列中排序的参照，由外部维护
    public bool dieNow = false;     //调试变量，立即杀死自身
    // private int MaxSortDelay = 10;  // 每10帧排一次序
    // private int sortDelay;          
    
    public float norAtkInterval { get; protected set; } = 0;         // 到下一次攻击还需要的时间
    // [HideInInspector] public bool nxtAtkImmediately = false;      // 下一次退出攻击状态时，立刻清空冷却并进入攻击状态
    [HideInInspector] public bool fighting;
    
    // 进阶数据
    [HideInInspector] public AtkSpeedController atkSpeedController;
    [HideInInspector] public ValueBuffer maxBlock = new ValueBuffer(0); // 最大阻挡数（敌人为消耗阻挡数）
    [HideInInspector] public bool dying;
    [HideInInspector] public int dizziness;         // 眩晕计数器，>0时表示处于眩晕状态
    
    // 显示在场上的，动画/物体位置
    [HideInInspector] public Transform animTransform;
    
    // 无人机相关
    public bool isDrone { get; protected set; } = false;        // 是否是无人机
    public bool canAtkDrone { get; protected set; } = true;     // 是否可以瞄准无人机

    // 血条canvas
    [HideInInspector] public Canvas frontCanvas;
    
    // BattleCore瞄准的目标
    public BattleCore target { get; private set; } = null;
    public bool tarIsNull { get; private set; } = true;

    // 当死亡时给外界广播回调用的函数
    public Action<BattleCore> DieAction;
    // 当普通攻击时给外界广播回调的函数
    public Action<BattleCore> NorAtkAction;

    // 阻挡的BattleCore列表
    [HideInInspector] public List<BattleCore> blockList = new List<BattleCore>();
    protected int blocked = 0;        // 只有敌人时有效，表示阻挡该敌人的干员数量

    protected override void Start_Core()
    {
        base.Start_Core();
    }
    
    protected override void Update_Core()
    {
        base.Update_Core();
        ChooseTarget();
        atkSpeedController.Update();
        if (!frozen)         // 如果处于冻结状态，攻击计时器不再运作
            norAtkInterval = norAtkInterval - Time.deltaTime <= 0 ? 0 : norAtkInterval - Time.deltaTime;

        if (dieNow) // 测试用，后期删掉
            GetDamage(1e9f, DamageMode.Magic);
    }

    private void LateUpdate()
    {
        CheckDie();
    }

    private void CheckDie()
    {
        if (life_.life <= 0 && !dying)
        {
            // Debug.Log(dying);
            dying = true;
            if (DieAction != null)
            {
                DieAction(this);
                DieAction = null;
            }
            DieBegin();
        }
    }

    protected virtual void DieBegin() {}

    protected virtual int operCmp(BattleCore a, BattleCore b)
    {
        // 给攻击范围内的干员排序，默认以priority（放置顺序）从大到小排
        if (a.tarPriority > b.tarPriority) return -1;
        if (a.tarPriority < b.tarPriority) return 1;
        return 0;
    }

    protected virtual int enemyCmp(BattleCore a, BattleCore b)
    {
        // 给攻击范围内的敌人排序，默认以priority（离终点距离）从小到大排
        if (a.tarPriority > b.tarPriority) return 1;
        if (a.tarPriority < b.tarPriority) return -1;
        return 0;
    }
    
    private void ChooseTarget()
    {
        operatorList.Sort(operCmp);
        enemyList.Sort(enemyCmp);

        if (blocked > 0 && blockList.Count > 0)
        {
            target = blockList[0];
            tarIsNull = false;
            return;
        }
        
        if (aimingMode == AimingMode.enemyFirst)
        {
            if (enemyList.Count == 0)
            {
                tarIsNull = true;
            }
            else
            {
                target = enemyList[0];
                tarIsNull = false;
            }
        }
        else if (aimingMode == AimingMode.operatorFirst)
        {
            if (operatorList.Count == 0)
            {
                tarIsNull = true;
            }
            else
            {
                target = operatorList[0];
                tarIsNull = false;
            }
        }
    }
    
    public virtual bool GetDizzy()
    {// 眩晕
        if (shieldList.Count > 0) return false;
        dizziness++;
        return true;
    }

    public virtual void RevokeDizzy()
    {// 眩晕计数器-1
        dizziness--;
        NorAtkClear();
    }

    /// <summary>  
    /// 自身对tarBC造成一次伤害，结束后更新彼此数值
    /// </summary>
    public void Battle(BattleCore tarBC, float damage, DamageMode mode, // 造成伤害的基础数值，以及本次伤害类型
        ElementSlot elementSlot, ElementTimer timer,    // 元素攻击，以及使用的元素计时器
        bool haveText = false, bool isBig = false)      // 显示攻击数字                    
    {
        bool canAttachElement = CauseDamageElement(
            tarBC, ref damage, elementSlot, timer);
        tarBC.GetDamage(this, damage, mode, elementSlot,
            canAttachElement, haveText, isBig);
    }
    
    public void Battle(BattleCore tarBC, float damage, DamageMode mode, // 造成伤害的基础数值，以及本次伤害类型
        ElementSlot elementSlot, bool canAttachElement,    // 元素攻击，以及使用的元素计时器
        bool haveText = false, bool isBig = false)      // 显示攻击数字                    
    {
        CauseDamageElement(tarBC, ref damage, elementSlot);
        tarBC.GetDamage(this, damage, mode, elementSlot,
            canAttachElement, haveText, isBig);
    }

    public void Battle(BattleCore tarBC, float damage, DamageMode mode, ElementSlot elementSlot)
    {
        Battle(tarBC, damage, mode, elementSlot, defaultElementTimer);
    }

    public void Battle(BattleCore tarBC, float damage, DamageMode mode = DamageMode.Physical)
    {
        ElementSlot phy = new ElementSlot();
        Battle(tarBC, damage, mode, phy, defaultElementTimer);
    }
    
    public void Battle(BattleCore tarBC, float damage, bool haveText, bool isBig = false)
    {
        ElementSlot phy = new ElementSlot();
        Battle(tarBC, damage, DamageMode.Physical, phy, defaultElementTimer, haveText, isBig);
    }

    public static void Battle_NoAttacker(BattleCore tarBC, float damage, 
        DamageMode mode, // 造成伤害的基础数值，以及本次伤害类型
        ElementSlot elementSlot, ElementTimer timer,    // 元素攻击，以及使用的元素计时器
        bool haveText = false, bool isBig = false) // 显示攻击数字     
    {
        bool canAttachElement = elementSlot.eleType != ElementType.None &&
                                timer != null && timer.AttachElement(tarBC);
        tarBC.GetDamage(null, damage, mode, elementSlot,
            canAttachElement, haveText, isBig, true);
    }
    
    public static void Battle_NoAttacker(BattleCore tarBC, float damage, 
        DamageMode mode, // 造成伤害的基础数值，以及本次伤害类型
        ElementSlot elementSlot, bool canAttachElement,    // 元素攻击，以及是否可以挂上元素
        bool haveText = false, bool isBig = false) // 显示攻击数字     
    {
        tarBC.GetDamage(null, damage, mode, elementSlot,
            canAttachElement, haveText, isBig, true);
    }


    /// <summary>
    /// 释放一次治疗
    /// </summary>
    public void Heal(BattleCore tarBC, float count, ElementSlot elementSlot,
        bool canAttachElement, bool haveText = false, bool isBig = false,
        bool reversal = false)
    {
        tarBC.GetHeal(this, count, elementSlot, canAttachElement, haveText, isBig,
            false, reversal);
    }

    public void Heal(BattleCore tarBC, float count, bool haveText = false, bool isBig = false,
        bool reversal = false)
    {
        tarBC.GetHeal(this, count, new ElementSlot(), false, haveText, isBig,
            false, reversal);
    }
    
    
    /// <summary>  
    /// 表示该BattleCore进行了一次普攻，开始冷却
    /// </summary>
    public void NorAtkStartCool()
    {
        norAtkInterval = atkSpeedController.minAtkInterval;
    }
    
    public void NorAtkClear()
    {
        norAtkInterval = 0;
    }
    
    public void NorAtkSet(float x)
    {
        norAtkInterval = x;
    }

    /// <summary>  
    /// 表示该Core是否可以进行普攻
    /// </summary>
    protected bool CanAtk()
    {
        bool k = true;
        k = k & (norAtkInterval <= 0);
        return k;
    }

    public virtual void SkillEnd()
    {
        // 技能结束时调用的函数
    } 
    
}

public enum AimingMode : byte
{
    [EnumLabel("瞄准干员")]
    operatorFirst,
    [EnumLabel("瞄准敌人")]
    enemyFirst
}

public class AtkSpeedController
{
    public Animator anim;
    public SpineAnimController ac_;
    public BattleCore bc_;
    public ValueBuffer atkSpeed;        // 攻击速度加成，100表示最小攻击间隔减小一半
    public float minAtkInterval;        // 最小攻击间隔
    public float baseInterval;          // 基础攻击间隔
    
    private float fightAnimTime;

    public AtkSpeedController(BattleCore bc, SpineAnimController controller, float aspeed, float interval)
    {
        bc_ = bc;
        ac_ = controller;
        anim = ac_.anim;
        atkSpeed = new ValueBuffer(aspeed);
        ChangeBaseInterval(interval);
    }

    public void Update()
    {
        if (atkSpeed.val == 0 || bc_.fighting == false || bc_.frozen) return;
        var staInfo = anim.GetCurrentAnimatorStateInfo(0);
        
        if (!staInfo.IsName("Fight")) return;

        fightAnimTime = staInfo.length * anim.speed;
        if (fightAnimTime - minAtkInterval < 0.008f) return;

        float nspeed = (fightAnimTime / minAtkInterval) + 0.005f;
        if (nspeed > 10) return;    // 相当重要
        ac_.atkSpeed = nspeed;
        ac_.ChangeAnimSpeed();
    }
    
    public void ChangeBaseInterval(float interval)
    {
        baseInterval = interval;
        RefreshInterval();
    }

    public void RefreshInterval()
    {
        float tmp = 1 / (1 + atkSpeed.val / 100);
        minAtkInterval = baseInterval * tmp;
        if (atkSpeed.val == 0)
        {
            ac_.atkSpeed = 1;
            ac_.ChangeAnimSpeed();
        }
    }
}

public class SkillAtkSpeedBuff : SkillBuffSlot
{
    private AtkSpeedController atkSpeedController;
    private float atkSpeedInc;
    private ValueBuffInner buffInner;
    private float baseInterval;
    
    private float p_baseInterval;

    public SkillAtkSpeedBuff(AtkSpeedController controller, float atkSpeedInc_, BattleCore bc,
        float interval = -1) : base(bc)
    {
        atkSpeedController = controller;
        atkSpeedInc = atkSpeedInc_;
        buffInner = new ValueBuffInner(ValueBuffMode.Fixed, atkSpeedInc);
        baseInterval = interval;

        p_baseInterval = atkSpeedController.baseInterval;
    }

    public override void BuffStart()
    {
        base.BuffStart();
        atkSpeedController.atkSpeed.AddValueBuff(buffInner);
        atkSpeedController.RefreshInterval();
        ReSetIntervalTime(atkSpeedController);
        if (baseInterval > 0) atkSpeedController.ChangeBaseInterval(baseInterval);
    }

    public override void BuffEnd()
    {
        base.BuffEnd();
        atkSpeedController.atkSpeed.DelValueBuff(buffInner);
        atkSpeedController.RefreshInterval();
        ReSetIntervalTime(atkSpeedController);
        if (baseInterval > 0) atkSpeedController.ChangeBaseInterval(p_baseInterval);
    }
    
    public override void BuffUpdate() { }

    public static void ReSetIntervalTime(AtkSpeedController atkSpeedController)
    {
        Animator anim = atkSpeedController.anim;
        BattleCore bc_ = atkSpeedController.bc_;
        
        var staInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (!staInfo.IsName("Fight"))
        {
            bc_.NorAtkClear();
            return;
        }
        
        float fightAnimTime = staInfo.length;
        if (fightAnimTime - atkSpeedController.minAtkInterval < 0.008f) return;
        float nspeed = (fightAnimTime / atkSpeedController.minAtkInterval) + 0.005f;
        float conTime = (staInfo.normalizedTime - (int) staInfo.normalizedTime) * fightAnimTime / nspeed;
        bc_.NorAtkSet(atkSpeedController.minAtkInterval - conTime);
    }
    
}

public class DurationAtkSpeedBuff : DurationBuffSlot
{
    private AtkSpeedController atkSpeedController;
    private float atkSpeedInc;
    private ValueBuffInner buffInner;
    private float baseInterval;
    
    private float p_baseInterval;
    
    public DurationAtkSpeedBuff(AtkSpeedController controller, float atkSpeedInc_, 
        float t, float interval = -1) : base(t)
    {
        atkSpeedController = controller;
        atkSpeedInc = atkSpeedInc_;
        buffInner = new ValueBuffInner(ValueBuffMode.Fixed, atkSpeedInc);
        baseInterval = interval;

        p_baseInterval = atkSpeedController.baseInterval;
    }
    
    public override void BuffStart()
    {
        atkSpeedController.atkSpeed.AddValueBuff(buffInner);
        atkSpeedController.RefreshInterval();
        SkillAtkSpeedBuff.ReSetIntervalTime(atkSpeedController);
        if (baseInterval > 0) atkSpeedController.ChangeBaseInterval(baseInterval);
    }
    
    public override void BuffEnd()
    {
        atkSpeedController.atkSpeed.DelValueBuff(buffInner);
        atkSpeedController.RefreshInterval();
        SkillAtkSpeedBuff.ReSetIntervalTime(atkSpeedController);
        if (baseInterval > 0) atkSpeedController.ChangeBaseInterval(p_baseInterval);
    }
}

public class DurationDizzyBuff : DurationBuffSlot
{
    private BattleCore dizzyBC_;
    private bool dizzySuccessed;

    public DurationDizzyBuff(BattleCore bc_, float durTime) : base(durTime)
    {
        dizzyBC_ = bc_;
    }

    public override void BuffStart()
    {
        dizzySuccessed = dizzyBC_.GetDizzy();
    }

    public override void BuffEnd()
    {
        if(dizzySuccessed) dizzyBC_.RevokeDizzy();
    }
}