using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class exquisiteThrow : MonoBehaviour
{
    private Yelan yelan;
    private OperatorCore hostOC;
    private float MaxDuration;
    private float multi;
    private float damIncPerSecond;
    private float talent2_inc;
    private float minAtkInterval = 2f;
    
    private ValueBuffInner damIncInner;
    private GameObject underCircle;
    private float duration;
    private float incTime;
    private float intervalTime;
    private bool dirRight;
    bool canAttatch;

    private void Awake()
    {
        yelan = transform.parent.parent.GetComponent<Yelan>();
    }

    public void Init(OperatorCore tarOC, float maxDuration, float multi_, 
        float damIncInit_, float damIncPerSecond_, float talent2_inc_)
    {
        hostOC = tarOC;
        MaxDuration = duration = maxDuration;
        incTime = intervalTime = 0;
        multi = multi_;
        damIncPerSecond = damIncPerSecond_;
        talent2_inc = talent2_inc_;
        gameObject.SetActive(true);
        
        // 移动到指定位置
        Vector3 scale = new Vector3(hostOC.ac_.dirRight ? 1 : -1, 1, 1);
        transform.position = hostOC.transform.position;
        transform.localScale = scale;
        dirRight = hostOC.ac_.dirRight;
        
        // 初始化元素伤害加成
        damIncInner = new ValueBuffInner(ValueBuffMode.Fixed, damIncInit_);
        hostOC.elementDamage.AddValueBuff(damIncInner);

        // 给宿主加上脚底光环
        underCircle = PoolManager.GetObj(yelan.Skill3_UnderCircle);
        underCircle.transform.SetParent(hostOC.transform);
        underCircle.transform.localPosition = new Vector3(0, 0.01f, 1.33f);
        
        // 给宿主普通攻击加回调函数
        hostOC.NorAtkAction += SynergyAttack;
        
        // 加上死亡监听函数
        hostOC.DieAction += Retreat;
        yelan.DieAction += Retreat;
    }

    private void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0)
        {
            Retreat(null);
            return;
        }

        intervalTime -= Time.deltaTime;
        incTime += Time.deltaTime;      // 每秒增加元素伤害
        if (incTime >= 1)
        {
            incTime -= 1;
            damIncInner.v += damIncPerSecond;
            hostOC.elementDamage.RefreshValue();
        }

        if (hostOC.animTransform.localScale.x >= 0 && !dirRight)
        {// 根据宿主的朝向方向而改变位置
            dirRight = true;
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (hostOC.animTransform.localScale.x < 0 && dirRight)
        {
            dirRight = false;
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void SynergyAttack(BattleCore bc)
    {// 协同攻击
        if (intervalTime > 0) return;
        intervalTime = minAtkInterval;
        AudioManager.PlayEFF(yelan.qAtkStartAudio);
        StartCoroutine(SynergyAttackIEnumerator());
    }

    IEnumerator SynergyAttackIEnumerator()
    {
        float dam = yelan.atk_.val * multi;
        dam += yelan.life_.life * talent2_inc;
        canAttatch = true;
        
        for (int i = 0; i < 3; i++)
        {
            GameObject atkAnim = PoolManager.GetObj(yelan.Skill3_AtkAnim);
            Vector3 pos = new Vector3(dirRight ? -0.37f : 0.37f, 0.5f, 0.64f);
            pos += hostOC.transform.position;
            TrackMove trackMove = atkAnim.GetComponent<TrackMove>();
            trackMove.Init(pos, yelan, hostOC.target, 12f, Attack,
                dam, new Vector3(0, 0.15f, 0.3f));
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void Attack(float multi, BattleCore tarBC, TrackMove tm)
    {
        GameObject hitAnim = PoolManager.GetObj(yelan.Skill3_HitAnim);
        hitAnim.transform.SetParent(tarBC.transform);
        Vector3 pos = tarBC.animTransform.localPosition + new Vector3(0, 0.1f, 0.4f);
        hitAnim.transform.localPosition = pos;
        DurationRecycleObj recycleObj = new DurationRecycleObj(hitAnim, 1f, tarBC, true);
        BuffManager.AddBuff(recycleObj);
        
        ElementSlot hydroSlot = new ElementSlot(ElementType.Hydro, 1f);
        yelan.Battle(tarBC, multi, DamageMode.Physical, hydroSlot, canAttatch,
            true);

        if(canAttatch) AudioManager.PlayEFF(yelan.qAtkHitAudio);
        canAttatch = false;
    }
    
    
    public void Retreat(BattleCore bc)
    {
        // 移除元素伤害加成
        hostOC.elementDamage.DelValueBuff(damIncInner);
        
        // 移除委托函数
        hostOC.NorAtkAction -= SynergyAttack;
        hostOC.DieAction -= Retreat;
        yelan.DieAction -= Retreat;
        
        // 关闭物体
        PoolManager.RecycleObj(underCircle);
        gameObject.SetActive(false);
    }
    
    
    
    
}
