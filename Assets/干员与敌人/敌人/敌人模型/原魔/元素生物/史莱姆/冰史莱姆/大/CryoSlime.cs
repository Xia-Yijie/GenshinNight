using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CryoSlime : EnemyCore
{
    [Header("冰史莱姆的特效")]
    public GameObject IceFog;

    private float fogMulti = 0.5f;
    
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

        GameObject fog = PoolManager.GetObj(IceFog);
        IceFogEnemy ife = fog.GetComponent<IceFogEnemy>();
        ife.Init(transform.position, atk_.val * fogMulti, 1.6f, 1.6f, 0.4f,
            this, true);
    }
    
    public void SpAtkEnd()
    {
        SpCannotMoveEnd();
    }


    public override void OnAttack()
    {
        base.OnAttack();
        ElementSlot elementSlot = new ElementSlot(ElementType.Cryo, 1f);
        Battle(target, atk_.val, DamageMode.Physical, elementSlot, true);
    }
    
    
    
    
    
}
