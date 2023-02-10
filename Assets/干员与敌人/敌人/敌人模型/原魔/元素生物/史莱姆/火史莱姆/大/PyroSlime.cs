using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PyroSlime : EnemyCore
{
    [Header("火史莱姆的特效")] 
    public GameObject fireBall;
    public GameObject fireHitAnim;

    public AudioClip fireAudio;
    
    protected override void Awake_Core()
    {
        base.Awake_Core();
    }
    
    protected override void Start_Core()
    {
        base.Start_Core();
        attachedElement.Add(ElementType.Pyro, 1e9f);
        
        // 火免疫
        getElementDamFuncList.Add(PyroImmune);
        
        // 给特殊抓取器设置判断函数
        // 敌人全部不计入
        SAG.enemyCheck = EC => false;
    }
    
    private float PyroImmune(ElementSlot slot, float dam)
    {
        if (slot.eleType != ElementType.Pyro) return dam;
        
        // 显示免疫文字
        GameObject obj = PoolManager.GetObj(StoreHouse.instance.reactionShowText);
        obj.transform.SetParent(OperUIManager.WorldCanvas.transform);
        Text text = obj.GetComponent<Text>();
        text.text = "免疫";
        text.color = new Color32(200, 200, 200, 255);
        text.transform.position = transform.position + new Vector3(0, 0.3f, 0);
        
        return 0;
    }
    
    
    protected override void Update_Core()
    {
        base.Update_Core();

        if (sp_.CanReleaseSkill() && operatorList_es.Count > 0)
        {
            anim.SetBool("sp", true);
        }
    }

    private bool fireBallAttach = false;
    public void SpAtkStart()
    {
        SpCannotMove();
        sp_.ReleaseSkill();
        fireBallAttach = true;
        anim.SetBool("sp", false);
        
        
        // 找到范围内优先级最大的目标干员，用于转向
        float maxPrio = -1e9f;
        OperatorCore spTarOC = operatorList_es[0];
        foreach (var OC in operatorList_es)
        {
            if (OC.tarPriority > maxPrio)
            {
                maxPrio = OC.tarPriority;
                spTarOC = OC;
            }
        }
        if (maxPrio < 0) return;

        if (spTarOC.transform.position.x - transform.position.x >= 0) ac_.TurnRight();
        else ac_.TurnLeft();
    }

    
    public void SpAtkOut()
    {
        // 找到范围内优先级最大的目标干员
        if (operatorList_es.Count == 0) return;
        float maxPrio = -1e9f;
        OperatorCore spTarOC = operatorList_es[0];
        foreach (var OC in operatorList_es)
        {
            if (OC.tarPriority > maxPrio)
            {
                maxPrio = OC.tarPriority;
                spTarOC = OC;
            }
        }

        if (maxPrio < 0) return;
        
        var arrow = PoolManager.GetObj(fireBall);
        parabola par = arrow.GetComponent<parabola>();
        
        Vector3 pos = transform.position;
        pos += new Vector3(0, 0.289f, 0.5f);
        par.Init(pos, this, spTarOC, 10f, FireBallAttack, 1f,
            new Vector3(0, 0, 0.6f));
    }
    
    private void FireBallAttack(float multi, BattleCore tarBC, parabola par, bool tarIsNull)
    {
        if (tarIsNull) return;
        
        GameObject hitAnim = PoolManager.GetObj(fireHitAnim);
        hitAnim.transform.parent = tarBC.transform;
        Vector3 pos = tarBC.animTransform.localPosition + new Vector3(0, 0, 0.6f);
        hitAnim.transform.localPosition = pos;
        DurationRecycleObj recycleObj = new DurationRecycleObj(hitAnim, 1f, tarBC, true);
        BuffManager.AddBuff(recycleObj);

        ElementSlot elementSlot = new ElementSlot(ElementType.Pyro, 1f);
        Battle(tarBC, atk_.val, DamageMode.Physical, elementSlot,
            fireBallAttach, true);

        fireBallAttach = false;
    }
    
    public void SpAtkEnd()
    {
        SpCannotMoveEnd();
    }
    
    
    public override void OnAttack()
    {
        base.OnAttack();
        ElementSlot elementSlot = new ElementSlot(ElementType.Pyro, 1f);
        Battle(target, atk_.val, DamageMode.Physical, elementSlot, true);
    }
}
