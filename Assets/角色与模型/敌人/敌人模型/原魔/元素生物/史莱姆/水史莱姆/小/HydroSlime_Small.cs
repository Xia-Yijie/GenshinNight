using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HydroSlime_Small : EnemyCore
{
    protected override void Start_Core()
    {
        base.Start_Core();
        attachedElement.Add(ElementType.Hydro, 1e9f);
        
        // 水免疫
        getElementDamFuncList.Add(HydroImmune);
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
    
    
    public override void OnAttack()
    {
        base.OnAttack();
        ElementSlot elementSlot = new ElementSlot(ElementType.Hydro, 1f);
        Battle(target, atk_.val, DamageMode.Physical, elementSlot, true);
    }
    
    
}
