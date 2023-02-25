using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuffTimer : Timer
{// 类似翠绿之影触发机制的buff计时器，重复触发刷新持续时间

    protected BattleCore bc_;
    protected float MaxDurTime;
    
    protected Dictionary<BattleCore, float> buffTimeDict = new Dictionary<BattleCore, float>();

    public BuffTimer(BattleCore bc, float maxDurTime)
    {
        bc_ = bc;
        MaxDurTime = maxDurTime;
        bc.timerList.Add(this);
    }


    public override void Update()
    {
        for (int i = 0; i < buffTimeDict.Count; i++)
        {
            var tmp = buffTimeDict.ElementAt(i);
            BattleCore battleCore = tmp.Key;
            buffTimeDict[battleCore] -= Time.deltaTime;

            if (buffTimeDict[battleCore] <= 0)         // 如果该BattleCore已完成冷却，则删除
            {
                EndAction(battleCore);
                buffTimeDict.Remove(battleCore);
                i--;
            }
        }
    }

    public override void Clear()
    {
        buffTimeDict.Clear();
    }

    public virtual void Add(BattleCore tarBC)
    {// tarBC身上触发了一次buff
        if (buffTimeDict.ContainsKey(tarBC))
        {
            buffTimeDict[tarBC] = MaxDurTime;
        }
        else
        {
            buffTimeDict.Add(tarBC, MaxDurTime);
            StartAction(tarBC);
        }
    }

    protected virtual void StartAction(BattleCore tarBC) { }
    
    protected virtual void EndAction(BattleCore tarBC) { }
    
}
