using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnemoSlime_Small : EnemyCore
{
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
}
