using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Timer
{
    public abstract void Update();
    public abstract void Clear();
}

public class ElementTimer : Timer
{
    private float maxDuring;        // 最大元素附着间隔
    private ElementCore elc_;       // 可能为null，表示无源计时器，此时需要手动调用Update
    
    private Dictionary<ElementCore, float> elementTimeDict;


    public ElementTimer(ElementCore elementCore, float maxDuringTime = 3)
    {
        elc_ = elementCore;
        maxDuring = maxDuringTime;
        elementTimeDict = new Dictionary<ElementCore, float>();

        if (elc_ != null) elc_.timerList.Add(this);
        
    }

    public override void Update()
    {// 每帧更新，需要在ElementCore内注册调用

        for (int i = 0; i < elementTimeDict.Count; i++)
        {
            var tmp = elementTimeDict.ElementAt(i);
            ElementCore elementCore = tmp.Key;
            elementTimeDict[elementCore] -= Time.deltaTime;

            if (elementTimeDict[elementCore] <= 0)         // 如果该ElementCore已完成冷却，则删除
            {
                elementTimeDict.Remove(elementCore);
                i--;
            }

        }
    }

    /// <summary>
    /// 判断目标能否被挂上元素，如果可以，让目标进入元素附着冷却
    /// 也可以作为其他冷却时间的触发函数
    /// </summary>
    public bool AttachElement(ElementCore elementCore)
    {
        if (maxDuring < 0) return true;
        if (elementTimeDict.ContainsKey(elementCore)) return false;
        elementTimeDict.Add(elementCore, maxDuring);
        return true;
    }

    public override void Clear()
    {
        elementTimeDict.Clear();
    }
    
}
