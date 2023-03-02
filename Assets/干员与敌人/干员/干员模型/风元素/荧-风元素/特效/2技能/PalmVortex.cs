using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalmVortex : MonoBehaviour
{
    public GameObject windImpact;
    public GameObject charge;
    
    private Lumine_Anemo lumine;
    private float t;
    private Vector3 tarPos;
    private bool hadImpact;

    public Vector3 Init(Lumine_Anemo la)
    {
        lumine = la;
        transform.SetParent(lumine.transform);

        Vector3 dpos = lumine.direction switch
        {
            FourDirection.Right => new Vector3(1, 0, 0.2f),
            FourDirection.UP => new Vector3(0, 0, 1),
            FourDirection.Left => new Vector3(-1, 0, 0.2f),
            FourDirection.Down => new Vector3(0, 0, -1),
            _ => new Vector3(0, 0, 0)
        };
        tarPos = lumine.direction switch
        {
            FourDirection.Right => new Vector3(1.7f, 0, 0.2f),
            FourDirection.UP => new Vector3(0, 0, 1.7f),
            FourDirection.Left => new Vector3(-1.7f, 0, 0.2f),
            FourDirection.Down => new Vector3(0, 0, -1.7f),
            _ => new Vector3(0, 0, 0)
        };
        
        transform.localPosition = dpos;
        
        DurationRecycleObj recycleObj = new DurationRecycleObj(gameObject, 2.5f, lumine, true);
        BuffManager.AddBuff(recycleObj);

        charge.SetActive(gameManager.knowledgeData.WhirlingAnemo.num != 0);

        t = 0;
        hadImpact = false;

        return transform.position;
    }


    private void Update()
    {
        t += Time.deltaTime;
        if (t > 2f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, tarPos, 0.1f);
            if (!hadImpact)
            {
                hadImpact = true;
                GameObject impact = PoolManager.GetObj(windImpact);
                impact.transform.SetParent(lumine.transform);
                impact.transform.localPosition = Vector3.zero;

                impact.transform.eulerAngles = lumine.direction switch
                {
                    FourDirection.Right => new Vector3(0, 90, 0),
                    FourDirection.UP => new Vector3(0, 0, 0),
                    FourDirection.Left => new Vector3(0, -90, 0),
                    FourDirection.Down => new Vector3(0, 180, 0),
                    _ => Vector3.zero
                };

                DurationRecycleObj recycleObj = new DurationRecycleObj(impact, 1, lumine, true);
                BuffManager.AddBuff(recycleObj);
                
                lumine.Skill2_BurstDamage();
            }
        }
    }
}
