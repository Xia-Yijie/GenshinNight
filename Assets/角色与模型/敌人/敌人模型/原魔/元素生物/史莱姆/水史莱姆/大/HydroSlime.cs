using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HydroSlime : EnemyCore
{
    [Header("水史莱姆的特效")] 
    public GameObject PPDown;
    public GameObject PP;
    public GameObject PPHit;
    public AudioClip paopaoOut;
    public AudioClip paopaoBoom;

    private float dizzyTime = 2.5f;

    protected override void Start_Core()
    {
        base.Start_Core();
        attachedElement.Add(ElementType.Hydro, 1e9f);
        
        // 水免疫
        getElementDamFuncList.Add(HydroImmune);
        
        // 给特殊抓取器设置判断函数
        // 敌人全部不计入
        SAG.enemyCheck = EC => false;
    }
    
    private float HydroImmune(ElementSlot slot, float dam)
    {
        if (slot.eleType != ElementType.Hydro) return dam;
        
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


    public void SpAtkStart()
    {
        SpCannotMove();
        sp_.ReleaseSkill();
        anim.SetBool("sp", false);

        GameObject ppdown = PoolManager.GetObj(PPDown);
        ppdown.transform.SetParent(transform);
        ppdown.transform.localPosition=Vector3.zero;
        DurationRecycleObj recycleObj = new DurationRecycleObj(ppdown, 1.5f);
        BuffManager.AddBuff(recycleObj);
        
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
        
        var arrow = PoolManager.GetObj(PP);
        TrackMove tar = arrow.GetComponent<TrackMove>();
        
        Vector3 pos = transform.position;
        pos += new Vector3(0, 0, 0.4f);
        tar.Init(pos, this, spTarOC, 3f, PPAttack, 1f,
            new Vector3(0, 0, 0.6f));
    }

    private void PPAttack(float multi, BattleCore tarBC, TrackMove tar)
    {
        GameObject hitAnim = PoolManager.GetObj(PPHit);
        hitAnim.transform.parent = tarBC.transform;
        Vector3 pos = tarBC.animTransform.localPosition + new Vector3(0, 0, 0.3f);
        hitAnim.transform.localPosition = pos;
        DurationRecycleObj recycleObj = new DurationRecycleObj(hitAnim, 1f, tarBC, true);
        BuffManager.AddBuff(recycleObj);

        ElementSlot elementSlot = new ElementSlot(ElementType.Hydro, 1f);
        Battle(tarBC, atk_.val, DamageMode.Physical, elementSlot,
            true, true);

        DurationDizzyBuff dizzyBuff = new DurationDizzyBuff(tarBC, dizzyTime);
        BuffManager.AddBuff(dizzyBuff);
        
        AudioManager.PlayEFF(paopaoBoom);
    }


    public void SpAtkEnd()
    {
        SpCannotMoveEnd();
    }

    public override void OnAttack()
    {
        base.OnAttack();
        ElementSlot elementSlot = new ElementSlot(ElementType.Hydro, 1f);
        Battle(target, atk_.val, DamageMode.Physical, elementSlot, true);
    }
    
    
    
    
}
