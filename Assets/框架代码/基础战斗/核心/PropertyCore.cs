using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Spine;

public class PropertyCore : MonoBehaviour
{
    // 基础数据
    public ValueBuffer atk_ = new ValueBuffer(0);
    public ValueBuffer def_ = new ValueBuffer(0);
    public ValueBuffer magicDef_ = new ValueBuffer(0);
    public LifeController life_ = new LifeController();
    public SPController sp_ = new SPController();
    
    // 附着在身上的护盾列表
    [HideInInspector] public List<NormalShield> shieldList = new List<NormalShield>();
    [HideInInspector] public int CrystallizationNum = 0;        // 身上的结晶盾数量

    // 造成伤害增加buff，val值表示百分比增加幅度（不可小于-1）
    public ValueBuffer causeDamInc_ = new ValueBuffer(0);
    // 造成伤害改变委托列表，由其他类注册，前委托的输出会作为后委托的输入
    public List<Func<float, float>> causeDamFuncList = new List<Func<float, float>>();
    // 受到伤害增加buff，val值表示百分比增加幅度（不可小于-1）
    public ValueBuffer getDamInc_ = new ValueBuffer(0);
    // 受到伤害改变委托列表，由其他类注册，前委托的输出会作为后委托的输入
    public List<Func<float, float>> getDamFuncList = new List<Func<float, float>>();

    private void Awake()
    {
        Awake_Core();
    }
    
    protected virtual void Awake_Core(){}

    void Start()
    {
        Start_Core();
    }
    
    protected virtual void Start_Core(){}
    
    void Update()
    {
        sp_.Update();
        Update_Core();
    }
    
    protected virtual void Update_Core(){}
    
    
    /// <summary>  
    /// 基础输出伤害
    /// </summary>
    public float CauseDamageProperty(ValueBuffer baseValue, float mul = 1f)
    {
        float dam = baseValue.val * mul;
        dam *= causeDamInc_.val < -1 ? 0 : 1 + causeDamInc_.val;
        foreach (var damFunc in causeDamFuncList)
            dam = damFunc(dam);
        return dam;
    }

    /// <summary>  
    /// 计算受到防御和法抗，以及伤害委托后的伤害值
    /// </summary>
    public float GetDamageProperty(float baseDamage, DamageMode mode = DamageMode.Physical,
        BattleArgs args = default)
    {
        args ??= new BattleArgs();
        float dam = baseDamage;
        dam *= getDamInc_.val < -1 ? 0 : 1 + getDamInc_.val;
        
        if (getDamFuncList.Count > 0)   // 如果有伤害委托
        {// 复制一份委托并遍历
            List<Func<float, float>> tmp = new List<Func<float, float>>(getDamFuncList);
            foreach (var damFunc in tmp) dam = damFunc(dam);
        }


        if (mode == DamageMode.Physical)
        {
            float def = def_.val;
            def *= (1 - args.ignoreDefPercentage);
            def = def - args.ignoreDefFixed < 0 ? 0 : def - args.ignoreDefFixed;    // 防御穿透
            dam = dam - def < 0 ? 0 : dam - def;
        }
            
        else if (mode == DamageMode.Magic){
            float mdef = magicDef_.val;
            mdef = mdef - args.ignoreMagicDefFixed < 0 ? 0 : mdef - args.ignoreMagicDefFixed;// 法术穿透
            dam = dam * (1 - (mdef / 100));
        }

        return dam;
    }
    
    
}

public enum DamageMode
{
    Physical,
    Magic
}

public enum ValueBuffMode
{
    Fixed,
    Percentage,
    Multi
}

public class ValueBuffInner
{
    public ValueBuffMode mode;
    public float v;

    public ValueBuffInner() { }

    public ValueBuffInner(ValueBuffMode m, float vv)
    {
        mode = m;
        v = vv;
    }
    
}

public class ValueBuffer
{
    // 给buff使用的数值缓冲区

    public float val { get; private set; }
    public float baseVal { get; private set; }
    
    // 两种模式，固定数值增加，百分比增加（加算），百分比基于基础数值
    private List<ValueBuffInner> valueBuffList = new List<ValueBuffInner>();
    // 特殊数值增减，最后结算
    private List<Func<float, float>> valueFuncList = new List<Func<float, float>>();

    public ValueBuffer()
    {
        val = baseVal = 0;
    }
    public ValueBuffer(float v)
    {
        val = baseVal = v;
    }

    public virtual void RefreshValue()
    {
        val = baseVal;
        foreach (var buffSlot in valueBuffList)
        {
            if (buffSlot.mode == ValueBuffMode.Fixed)
                val += buffSlot.v;
            else if(buffSlot.mode == ValueBuffMode.Percentage)
                val += buffSlot.v * baseVal;
        }
        
        // Multi 为最终乘算
        foreach (var buffSlot in valueBuffList.Where(buffSlot => buffSlot.mode == ValueBuffMode.Multi))
        {
            val *= buffSlot.v;
        }
        
        // 特殊数值计算
        foreach (var func in valueFuncList)
        {
            val = func(val);
        }
    }
    
    /// <summary>  
    /// 初始化
    /// </summary>
    public void Init(float newBaseVal)
    {
        valueBuffList.Clear();
        valueFuncList.Clear();
        ChangeBaseValue(newBaseVal);
    }
    
    /// <summary>  
    /// 更改基础数值
    /// </summary>
    public void ChangeBaseValue(float newBaseVal)
    {
        baseVal = newBaseVal;
        RefreshValue();
    }
    
    /// <summary>  
    /// 增加基础数值
    /// </summary>
    public void AddBaseValue(float addBaseVal)
    {
        ChangeBaseValue(baseVal + addBaseVal);
    }
    
    
    /// <summary>  
    /// 加入新的数值buff
    /// </summary>
    public void AddValueBuff(ValueBuffInner buff)
    {
        valueBuffList.Add(buff);
        RefreshValue();
    }
    
    /// <summary>  
    /// 移除已有的数值buff
    /// </summary>
    public void DelValueBuff(ValueBuffInner buff)
    {
        valueBuffList.Remove(buff);
        RefreshValue();
    }

    /// <summary>
    /// 增加一个特殊数值计算
    /// </summary>
    public void AddFunc(Func<float, float> func)
    {
        valueFuncList.Add(func);
    }

    public void DelFunc(Func<float, float> func)
    {
        valueFuncList.Remove(func);
    }
    
}

public class LifeController : ValueBuffer
{
    // lifeController父类的val为最大生命值，子类的life为当前生命值
    public float life { get; private set; }

    public void InitBaseLife(float v)
    {
        Init(v);
        life = v;
    }

    public void RecoverCompletely()
    {
        life = val;
    }

    public void GetDamage(float damage)
    {
        life = life - damage < 0 ? 0 : life - damage;
    }

    public void GetHeal(float heal)
    {
        life = life + heal > val ? val : life + heal;
    }

    public override void RefreshValue()
    {
        float preMaxLife = val;
        base.RefreshValue();

        if (preMaxLife >= val) life = Mathf.Min(life, val); // 如果生命上限减少了
        else life = life / preMaxLife * val;    // 等比例增加
    }

    public float Percentage()
    {
        return life / val;
    }
    
}

public class SPController
{
    public static Action<SPController> releaseAction;

    public BattleCore bc_;
    
    public float sp;
    public float maxSp;
    public ValueBuffer spRecharge = new ValueBuffer(1);
    
    public bool during;         // 技能是否开启
    public float remainingTime; // 技能剩余持续时间
    public float maxTime;       // 技能最大持续时间
    public recoverType reType;  // 技力恢复模式
    public releaseType outType;    // 技能释放模式

    public void Init(BattleCore bc__, float ssp, float maxxSp, float maxxTime,
        recoverType type1, releaseType type2, float recharge)
    {
        bc_ = bc__;
        sp = ssp > maxxSp ? maxxSp : ssp;
        maxSp = maxxSp;
        maxTime = maxxTime;
        reType = type1;
        outType = type2;
        spRecharge.ChangeBaseValue(recharge);
        during = false;
        remainingTime = 0;
    }

    public void Update()
    {
        if (during)
        {
            if (remainingTime <= 0)
            {
                EndSkillWithFunc();
            }
            else remainingTime -= Time.deltaTime;
        }
        else
        {
            if (reType == recoverType.auto)
            {
                float getSP = Time.deltaTime * spRecharge.val;
                sp = sp + getSP > maxSp ? maxSp : sp + getSP;
            }
        }
        
    }

    public void GetSp_Atk(float getSP = 1)
    {
        if (during || reType!=recoverType.atk) return;
        getSP *= spRecharge.val;
        sp = sp + getSP > maxSp ? maxSp : sp + getSP;
    }
    
    public void GetSp_BeAtk(float getSP = 1)
    {
        if (during || reType!=recoverType.beAtk) return;
        getSP *= spRecharge.val;
        sp = sp + getSP > maxSp ? maxSp : sp + getSP;
    }

    public void GetSp(float getSP)
    {
        getSP *= spRecharge.val;
        sp = sp + getSP > maxSp ? maxSp : sp + getSP;
    }

    public bool CanReleaseSkill()
    {
        return sp >= maxSp && !during;
    }

    public void ReleaseSkill(bool noReleaseAction = false)
    {
        if (!noReleaseAction) releaseAction?.Invoke(this);      // 执行释放技能回调函数

        during = true;
        sp = 0;
        remainingTime = maxTime;
    }

    public void EndSkill()
    {
        if (!during) return;
        during = false;
        remainingTime = 0;
    }

    public void EndSkillWithFunc()
    {
        bc_.SkillEnd();
        EndSkill();
    }
    
}

public class DurationValueBuff : DurationBuffSlot
{
    private ValueBuffer valueBuffer;
    private ValueBuffInner buffInner;
    public DurationValueBuff(ValueBuffer buffer, ValueBuffMode buffMode, float v, float durTime) : base(durTime)
    {
        valueBuffer = buffer;
        buffInner = new ValueBuffInner(buffMode, v);
    }

    public override void BuffStart()
    {
        valueBuffer.AddValueBuff(buffInner);
    }

    public override void BuffEnd()
    {
        valueBuffer.DelValueBuff(buffInner);
    }
}

public class SkillValueBuff : SkillBuffSlot
{
    private ValueBuffer valueBuffer;
    private ValueBuffInner buffInner;
    public SkillValueBuff(ValueBuffer buffer, ValueBuffMode buffMode, float v, BattleCore bc) : base(bc)
    {
        valueBuffer = buffer;
        buffInner = new ValueBuffInner();
        buffInner.mode = buffMode;
        buffInner.v = v;
    }

    public override void BuffStart()
    {
        valueBuffer.AddValueBuff(buffInner);
    }

    public override void BuffEnd()
    {
        valueBuffer.DelValueBuff(buffInner);
    }

    public override void BuffUpdate() { }
}