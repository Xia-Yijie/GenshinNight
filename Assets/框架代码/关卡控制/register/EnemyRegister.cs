using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRegister : MonoBehaviour
{
    [Header("敌人信息")]
    public enemyInfo ei_;
    
    [Header("波次")]
    public int wave;

    [Header("出现时间")] 
    public float appearTime;

    [Header("数量")] 
    public int count;

    [Header("间隔时间")] 
    public float duration;

    [Header("路径锚点(x,y,delay)")] 
    public List<Vector3> pointList = new List<Vector3>();   //该敌人的路径锚点(x,y,time)


    private void Awake()
    {
        float delay = appearTime;

        for (int i = 0; i < count; i++)
        {
            EnemyWaveInfoSlot slot = new EnemyWaveInfoSlot(ei_, wave, delay, pointList);
            InitManager.Register(slot);
            delay += duration;
        }

        Destroy(gameObject);
    }
}

public class EnemyWaveInfoSlot
{
    // 单个敌人出现信息
    public enemyInfo ei_;
    public int wave;
    public float appearTime;
    public List<Vector3> pointList = new List<Vector3>();

    public EnemyWaveInfoSlot(enemyInfo a, int b, float c, List<Vector3> d)
    {
        ei_ = a;
        wave = b;
        appearTime = c;
        pointList = new List<Vector3>(d);
    }

}