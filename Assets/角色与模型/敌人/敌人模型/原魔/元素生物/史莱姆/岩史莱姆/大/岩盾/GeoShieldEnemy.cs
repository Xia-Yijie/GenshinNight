using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeoShieldEnemy : NormalShield
{
    public override float GetDamage(float damage, ElementType type)
    {
        if (type == ElementType.Geo)
            damage *= 1.5f;

        damage /= (1 + bc_.shieldStrength.val);
        
        float ret = damage - life_.life <= 0 ? 0 : damage - life_.life;
        life_.GetDamage(damage);
        if (life_.life <= 0)
        {
            bc_.DieAction -= Die;
            ShieldBreak?.Invoke(this);
            Die(null);
        }
        return ret;
    }
}
