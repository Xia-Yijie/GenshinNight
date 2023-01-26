using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class thunderEye : MonoBehaviour
{
    public GameObject atkAnim;

    private const float radius = 0.6f;
    
    private Animator anim;
    private SpriteRenderer sr_;
    private RaidenShogun raiden;
    private BattleCore target;
    private ValueBuffInner spRechargeBuff;

    private bool canAttack = true;
    private float coolTime = 0;
    private float durTime;

    private Color32 lightColor = new Color32(255, 255, 255, 220);
    private Color32 darkColor = new Color32(200, 200, 200, 220);
    private Color32 tarColor;
    
    
    private void Awake()
    {
        anim = GetComponent<Animator>();
        sr_ = GetComponent<SpriteRenderer>();
        raiden = transform.parent.GetComponent<RaidenShogun>();
        spRechargeBuff = new ValueBuffInner(ValueBuffMode.Fixed, 1);
    }

    public void Init(float t)
    {
        gameObject.SetActive(true);
        durTime = t;
        canAttack = true;
        coolTime = 0;
        tarColor = lightColor;
        
        raiden.sp_.spRecharge.AddValueBuff(spRechargeBuff);
        raiden.DieAction += Retreat;
    }

    private void Update()
    {
        if (coolTime > 0)
        {
            if (coolTime - Time.deltaTime <= 0)
            {
                canAttack = true;
                tarColor = lightColor;
            }
            coolTime -= Time.deltaTime;
        }

        if (durTime > 0 && durTime - Time.deltaTime <= 0)
        {
            anim.SetBool("die", true);
        }
        durTime -= Time.deltaTime;

        sr_.color = Color.Lerp(sr_.color, tarColor, 6 * Time.deltaTime);
    }

    /// <summary>
    /// 雷电将军进行了攻击，可以协同攻击了
    /// </summary>
    public void RaidenAttack(BattleCore tar)
    {
        if (!gameObject.activeSelf || !canAttack || raiden.tarIsNull) return;
        anim.SetBool("fight", true);
        target = tar;
    }

    public void FightBegin()
    {
        anim.SetBool("fight", false);
        canAttack = false;
        coolTime = 3f;
    }

    public void Fighting()
    {
        GameObject thunder = PoolManager.GetObj(atkAnim);
        Vector3 pos = target.transform.position;
        thunder.transform.position = pos;
        DurationRecycleObj recycleObj = new DurationRecycleObj(thunder, 1f);
        BuffManager.AddBuff(recycleObj);

        var tar = InitManager.GetNearByEnemy(pos, radius);
        float dam = raiden.atk_.val * raiden.skill2_CoorMulti[raiden.skillLevel[1]];
        ElementSlot slot = new ElementSlot(ElementType.Electro, 1f);
        foreach (var enemy in tar)
        {
            raiden.Battle(enemy, dam, DamageMode.Magic, slot, true, true);
        }
    }

    public void FightEnd()
    {
        tarColor = darkColor;
    }

    public void DieEnd()
    {
        anim.SetBool("die", false);
        raiden.sp_.spRecharge.DelValueBuff(spRechargeBuff);
        raiden.DieAction -= Retreat;
        gameObject.SetActive(false);
    }
    
    public void Retreat(BattleCore bc_)
    {
        DieEnd();
    }
}
