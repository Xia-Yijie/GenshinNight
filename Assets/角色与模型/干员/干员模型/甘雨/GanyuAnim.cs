using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GanyuAnim : MonoBehaviour
{
    private Ganyu gy_;

    public GameObject charge;
    public GameObject fcircle_1;
    public GameObject fcircle_out;
    public GameObject fcircle_2;

    private void Awake()
    {
        gy_ = transform.parent.GetComponent<Ganyu>();
    }

    public void FrostFlask_StartCharge()
    {
        GameObject cir = PoolManager.GetObj(fcircle_1);
        cir.transform.SetParent(gy_.transform);
        Vector3 pos = new Vector3(gy_.ac_.dirRight ? 0.25f : -0.25f, 0.35f, 0.36f);
        cir.transform.localPosition = pos;
        DurationRecycleObj recycleObj = new DurationRecycleObj(cir, 1f, gy_, true);
        BuffManager.AddBuff(recycleObj);
        
        GameObject chargg = PoolManager.GetObj(charge);
        chargg.transform.SetParent(gy_.transform);
        chargg.transform.localPosition = pos;
        DurationRecycleObj recycleObj2 = new DurationRecycleObj(chargg, 1f, gy_, true);
        BuffManager.AddBuff(recycleObj2);
        
        gy_.chargeAudio.Play();
    }

    public void FrostFlask_OutCharge()
    {
        GameObject cir = PoolManager.GetObj(fcircle_out);
        cir.transform.SetParent(gy_.transform);
        Vector3 pos = new Vector3(gy_.ac_.dirRight ? 0.25f : -0.25f, 0.35f, 0.36f);
        cir.transform.localPosition = pos;
        DurationRecycleObj recycleObj = new DurationRecycleObj(cir, 1f, gy_, true);
        BuffManager.AddBuff(recycleObj);
    }

    public void FrostFlask_Charge2()
    {
        GameObject cir = PoolManager.GetObj(fcircle_2);
        cir.transform.SetParent(gy_.transform);
        Vector3 pos = new Vector3(gy_.ac_.dirRight ? 0.25f : -0.25f, 0.35f, 0.36f);
        cir.transform.localPosition = pos;
        DurationRecycleObj recycleObj = new DurationRecycleObj(cir, 1f, gy_, true);
        BuffManager.AddBuff(recycleObj);
    }

    public void FrostFlask_End()
    {
        gy_.NorAtkClear();
    }


    public void GhostEnd()
    {
        gy_.Ghost.SetActive(false);
    }

    public void FastFrostFlask()
    {
        
    }
    
    
}
