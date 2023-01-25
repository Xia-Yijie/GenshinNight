using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WandererAnim : MonoBehaviour
{
    private Wanderer wan;
    private Vector3 skill1Pos;

    private void Awake()
    {
        wan = transform.parent.GetComponent<Wanderer>();
    }

    public void Wander_FightBegin()
    {
        wan.circleAnim.SetBool("fight", true);
        Invoke(nameof(SetCircleFightFalse), 0.5f);
    }

    private void SetCircleFightFalse()
    {
        wan.circleAnim.SetBool("fight", false);
    }

    public void Skill1_Start()
    {
        skill1Pos = wan.target.transform.position;
        GameObject smallBall = PoolManager.GetObj(wan.anemoBall);
        smallBall.transform.position = skill1Pos;
        DurationRecycleObj recycleObj = new DurationRecycleObj(smallBall, 0.4f);
        BuffManager.AddBuff(recycleObj);

        wan.sp_.ReleaseSkill();
        wan.anim.SetBool("sp", false);
        wan.spAtkAudio.Play();
    }

    public void Skill1_Atk()
    {
        GameObject smallBurst = PoolManager.GetObj(wan.smallBurst);
        smallBurst.transform.position = skill1Pos;
        DurationRecycleObj recycleObj = new DurationRecycleObj(smallBurst, 1f);
        BuffManager.AddBuff(recycleObj);

        var tar = InitManager.GetNearByEnemy(skill1Pos, wan.skill1_Radius);
        float dam = wan.atk_.val * wan.skill1_Multi[wan.skillLevel[0]];
        dam *= Random.Range(0f, 1f) < wan.critRate ? 2 : 1;
        ElementSlot anemoSlot = new ElementSlot(ElementType.Anemo, 1f);
        foreach (var enemy in tar)
        {
            wan.AtkGetCircle(enemy);
            wan.GetAnemoShield();
            wan.Battle(enemy, dam, DamageMode.Magic, anemoSlot, true, true);
            
            GameObject hitAnim = PoolManager.GetObj(wan.norHitAnim);
            hitAnim.transform.parent = enemy.transform;
            Vector3 pos = enemy.animTransform.localPosition + new Vector3(0, 0, 0.3f);
            hitAnim.transform.localPosition = pos;
            DurationRecycleObj recycleObj2 = new DurationRecycleObj(hitAnim, 0.5f, enemy, true);
            BuffManager.AddBuff(recycleObj2);
        }
    }
    
    public void Skill3_EarlyStart()
    {
        Vector3 pos = wan.direction switch
        {
            FourDirection.Right => new Vector3(1, 0, 0),
            FourDirection.Left => new Vector3(-1, 0, 0),
            FourDirection.UP => new Vector3(0, 0, 1),
            FourDirection.Down => new Vector3(0, 0, -1),
            _ => Vector3.zero
        };
        
        GameObject flow = PoolManager.GetObj(wan.flowRol);
        flow.transform.position = wan.transform.position + pos;
        DurationRecycleObj recycleObj2 = new DurationRecycleObj(flow, 1f);
        BuffManager.AddBuff(recycleObj2);
    }

    public void Skill3_Start()
    {
        int dx = wan.ac_.dirRight ? 1 : -1;
        
        GameObject smallBall = PoolManager.GetObj(wan.anemoBall);
        smallBall.transform.position = wan.transform.position + new Vector3(0.35f * dx, 0, 0.2f);
        DurationRecycleObj recycleObj = new DurationRecycleObj(smallBall, 0.67f);
        BuffManager.AddBuff(recycleObj);
    }
    
    
}
