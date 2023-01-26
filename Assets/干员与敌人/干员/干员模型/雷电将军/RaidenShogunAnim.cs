using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaidenShogunAnim : MonoBehaviour
{
    public GameObject VeryGreatCut;

    private RaidenShogun raiden;

    private void Awake()
    {
        raiden = transform.parent.GetComponent<RaidenShogun>();
    }

    public void GreatCut()
    {
        GameObject veryGreatCut = PoolManager.GetObj(VeryGreatCut);
        Vector3 pos = transform.parent.position;
        Vector3 rol = new Vector3(60, 0, 0);
        Vector3 scale = new Vector3(1, 1, 1);

        switch (raiden.direction)
        {
            case FourDirection.Right:
                pos += new Vector3(2, 0, 0.4f);
                break;
            case FourDirection.Left:
                pos += new Vector3(-2, 0, 0.4f);
                scale = new Vector3(-1, 1, 1);
                break;
            case FourDirection.UP:
                pos += new Vector3(0.1f, 0f, 2.2f);
                rol = new Vector3(90, 0, 90);
                scale = new Vector3(0.9f, 1, 1);
                break;
            case FourDirection.Down:
                pos += new Vector3(-0.1f, 0, -2.2f);
                rol = new Vector3(90, 0, -90);
                scale = new Vector3(0.9f, 1, 1);
                break;
        }

        veryGreatCut.transform.position = pos;
        veryGreatCut.transform.eulerAngles = rol;
        veryGreatCut.transform.localScale = scale;
        DurationRecycleObj recycleObj = new DurationRecycleObj(veryGreatCut, 1f);
        BuffManager.AddBuff(recycleObj);
        
        raiden.Skill3_Burst();
    }

    public void Skill3_AtkAudio()
    {
        raiden.Skill3_AtkAudio();
    }
    
}
