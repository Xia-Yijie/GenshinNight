using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeoSlime : EnemyCore
{
    [Header("岩史莱姆的特效")] 
    public GameObject geoShield;
    public Slider shieldSlider;

    private GeoShieldEnemy gse_;
    private bool haveShield;

    private float reTime = 10f;
    private float t = 0;
    
    protected override void Start_Core()
    {
        base.Start_Core();
        
        // 岩免疫
        getElementDamFuncList.Add(GeoImmune);
    }

    private float GeoImmune(ElementSlot slot, float dam)
    {
        if (slot.eleType != ElementType.Geo) return dam;
        
        // 显示免疫文字
        GameObject obj = PoolManager.GetObj(StoreHouse.instance.reactionShowText);
        obj.transform.SetParent(OperUIManager.WorldCanvas.transform);
        Text text = obj.GetComponent<Text>();
        text.text = "免疫";
        text.color = new Color32(200, 200, 200, 255);
        text.transform.position = transform.position + new Vector3(0, 0.3f, 0);
        
        return 0;
    }

    public override void EnemyInit(List<Vector3> posList_)
    {
        base.EnemyInit(posList_);

        // 起始带有岩盾
        anim.SetInteger("sta", 1);
        GameObject shiled = PoolManager.GetObj(geoShield);
        gse_ = shiled.GetComponent<GeoShieldEnemy>();
        gse_.Init(this, life_.val, ElementType.Geo, ShieldBreak);
        haveShield = true;
    }

    private void ShieldBreak(NormalShield x)
    {
        haveShield = false;
        anim.SetBool("shieldDown", true);
        SpCannotMove();
        t = reTime;
        shieldSlider.gameObject.SetActive(false);
    }

    private void ShieldGetBegin()
    {
        anim.SetBool("shieldUp", true);
        SpCannotMove();
    }
    
    public void ShieldGet()
    {
        GameObject shiled = PoolManager.GetObj(geoShield);
        gse_ = shiled.GetComponent<GeoShieldEnemy>();
        gse_.Init(this, life_.val, ElementType.Geo, ShieldBreak);
        haveShield = true;
        shieldSlider.gameObject.SetActive(true);
    }

    protected override void Update_Core()
    {
        base.Update_Core();
        shieldSlider.value = haveShield ? gse_.life_.life / gse_.life_.val : 0;

        if (!haveShield)
        {
            if (t > 0 && t - Time.deltaTime <= 0) ShieldGetBegin();
            t -= Time.deltaTime;
        }
        
    }
    
    public override void OnAttack()
    {
        base.OnAttack();
        ElementSlot elementSlot = new ElementSlot(ElementType.Geo, 1f);
        Battle(target, atk_.val, DamageMode.Physical, elementSlot, true);
    }

    
    public void ShieldUpEnd()
    {
        anim.SetBool("shieldUp", false);
        anim.SetInteger("sta", 1);
        SpCannotMoveEnd();
    }
    
    public void ShieldDownEnd()
    {
        anim.SetBool("shieldDown", false);
        anim.SetInteger("sta", 0);
        SpCannotMoveEnd();
    }
    
    
    
}
