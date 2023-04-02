using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickHilichurl : EnemyCore
{
    public override void OnAttack()
    {
        ElementSlot slot = new ElementSlot();
        Battle(target, atk_.val, DamageMode.Physical, slot, false);
    }
}
