using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnemoSlime : EnemyCore
{
    [Header("风史莱姆的特效")]
    public GameObject AnemoArrow;
    public GameObject AnemoHit;
    
    protected override void Start_Core()
    {
        base.Start_Core();
        // 风免疫
        getElementDamFuncList.Add(AnemoImmune);
    }
    
    
    private float AnemoImmune(ElementSlot slot, float dam)
    {
        if (slot.eleType != ElementType.Anemo) return dam;
        
        // 显示免疫文字
        GameObject obj = PoolManager.GetObj(StoreHouse.instance.reactionShowText);
        obj.transform.SetParent(OperUIManager.WorldCanvas.transform);
        Text text = obj.GetComponent<Text>();
        text.text = "免疫";
        text.color = new Color32(200, 200, 200, 255);
        text.transform.position = transform.position + new Vector3(0, 0.3f, 0);
        
        return 0;
    }

    public override void OnAttack()
    {
        base.OnAttack();
        
        var arrow = PoolManager.GetObj(AnemoArrow);
        parabola par = arrow.GetComponent<parabola>();
        
        Vector3 pos = transform.position;
        pos += new Vector3(0, 0, 1f);
        par.Init(pos, this, target, 10f, AnemoAttack, 1f,
            new Vector3(0, 0, 0.6f));
    }
    
    private void AnemoAttack(float multi, BattleCore tarBC, parabola par, bool tarIsNull)
    {
        if (tarIsNull) return;
        GameObject hitAnim = PoolManager.GetObj(AnemoHit);
        hitAnim.transform.parent = tarBC.transform;
        Vector3 pos = tarBC.animTransform.localPosition + new Vector3(0, 0, 0.5f);
        hitAnim.transform.localPosition = pos;
        DurationRecycleObj recycleObj = new DurationRecycleObj(hitAnim, 1f, tarBC, true);
        BuffManager.AddBuff(recycleObj);

        ElementSlot elementSlot = new ElementSlot(ElementType.Anemo, 1f);
        Battle(tarBC, atk_.val, DamageMode.Magic, elementSlot,
            true, true);
    }
    
    
}
