using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PyroSlime_Small : EnemyCore
{
    protected override void Start_Core()
    {
        base.Start_Core();
        attachedElement.Add(ElementType.Pyro, 1e9f);
        
        // 火免疫
        getElementDamFuncList.Add(PyroImmune);
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
    
    public override void OnAttack()
    {
        base.OnAttack();
        ElementSlot elementSlot = new ElementSlot(ElementType.Pyro, 1f);
        Battle(target, atk_.val, DamageMode.Physical, elementSlot, true);
    }
}
