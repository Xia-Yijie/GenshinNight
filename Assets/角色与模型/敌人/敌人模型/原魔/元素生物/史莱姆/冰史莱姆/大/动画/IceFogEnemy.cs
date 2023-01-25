using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceFogEnemy : MonoBehaviour
{
    private ParticleSystem ps_;
    private float durTime;
    private ElementTimer timer = new ElementTimer(null);
    private Vector3 center;
    private float damage;
    private float radius;
    private float interval;
    
    private float t;
    private float endt;
    private bool started;
    private EnemyCore attacker;
    private bool attackerIsAvailable;
    
    private void Awake()
    {
        ps_ = GetComponent<ParticleSystem>();
        durTime = ps_.main.duration;
    }
    

    public void Init(Vector3 pos, float dam, float r, float interval_, 
        float scale = 0, EnemyCore attacker_ = null, bool haveAttacker = false)
    {
        if (scale != 0) transform.localScale = new Vector3(scale, scale, scale);
        damage = dam;
        radius = r;
        interval = interval_;
        center = pos;
        transform.position = pos;
        t = 1.5f;
        endt = durTime;
        started = false;

        attacker = attacker_;
        attackerIsAvailable = haveAttacker;
        if (haveAttacker) attacker.DieAction += AttackerDie;
    }

    void AttackerDie(BattleCore bc)
    {
        attackerIsAvailable = false;
    }

    
    void Update()
    {
        timer.Update();

        t -= Time.deltaTime;
        endt -= Time.deltaTime;
        if (endt <= 0)
        {
            if (attackerIsAvailable) attacker.DieAction -= AttackerDie;
            PoolManager.RecycleObj(gameObject);
            return;
        }
        
        if (!started)
        {
            if (t <= 0)
            {
                t = 0;
                started = true;
            }
        }
        else
        {
            if (t <= 0)
            {
                t = interval;
                if (endt < 1.5f) return;
                
                var tars = InitManager.GetNearByOper(center, radius);
                ElementSlot cryoSlot = new ElementSlot(ElementType.Cryo, 1f);
                foreach (var OC in tars)
                {
                    if (attackerIsAvailable)
                        attacker.Battle(OC, damage, DamageMode.Magic,
                            cryoSlot, timer, true);
                    else
                        BattleCore.Battle_NoAttacker(OC, damage, DamageMode.Magic,
                            cryoSlot, timer, true);
                }
            }
        }
    }
}
