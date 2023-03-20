using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTimer : Timer
{// 简易计时器，只有一个时间，不会受其他影响
    private float t;
    private float maxT;
    private ElementCore elc_;

    public SimpleTimer(ElementCore elementCore, float maxDuringTime)
    {
        t = 0;
        maxT = maxDuringTime;
        elc_ = elementCore;
        if (elc_ != null) elc_.timerList.Add(this);
    }
    
    public override void Update()
    {
        t -= Time.deltaTime;
    }

    public override void Clear()
    {
        t = 0;
    }

    public bool Valid()
    {
        return t <= 0;
    }
 
    public bool TryToReset()
    {
        if (t > 0) return false;
        t = maxT;
        return true;
    }
    
    
}
