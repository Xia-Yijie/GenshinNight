using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hound : EnemyCore
{
    protected override void Awake_Core()
    {
        base.Awake_Core();
    }

    public override void EnemyInit(List<Vector3> pointList_)
    {
        base.EnemyInit(pointList_);
        speed.ChangeBaseValue(2);
        
        // ElementType type = ElementType.Pyro;
        // GameObject shield = PoolManager.GetObj(StoreHouse.GetCrystallizationShield(type));
        // NormalShield normalShield = shield.GetComponent<NormalShield>();
        // normalShield.Init(this, 500, type);
    }

    protected override void Update_Core()
    {
        base.Update_Core();
        // ElementSlot elementSlot = new ElementSlot(ElementType.Hydro, 8f);
        // Battle(this, 0, DamageMode.Physical, elementSlot, defaultElementTimer);
    }
    
    public override void OnAttack()
    {
        ElementSlot elementSlot = new ElementSlot();
        Battle(target, atk_.val, DamageMode.Physical, elementSlot, defaultElementTimer);
    }
    
}
