using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CryoSlime_Small : EnemyCore
{
    protected override void Start_Core()
    {
        base.Start_Core();
        attachedElement.Add(ElementType.Cryo, 1e9f);
        
        // 冰免疫
        getElementDamFuncList.Add(CryoImmune);
        // 冻结免疫
        frozenImmune = true;
    }
    
    
    private float CryoImmune(ElementSlot slot, float dam)
    {
        if (slot.eleType != ElementType.Cryo) return dam;
        
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
        ElementSlot elementSlot = new ElementSlot(ElementType.Cryo, 1f);
        Battle(target, atk_.val, DamageMode.Physical, elementSlot, true);
    }
}
