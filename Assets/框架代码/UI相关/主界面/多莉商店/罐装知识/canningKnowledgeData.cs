using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class canningKnowledgeData
{
    //************* 通用属性 ****************
    public KnowledgeBuffer atkInc = new KnowledgeBuffer(10, 2);
    public KnowledgeBuffer defInc = new KnowledgeBuffer(10, 2);
    public KnowledgeBuffer lifeInc = new KnowledgeBuffer(10, 2);
    public KnowledgeBuffer blockInc = new KnowledgeBuffer(1, 24);
    public KnowledgeBuffer damInc = new KnowledgeBuffer(10, 3);
    public KnowledgeBuffer rechargeInc = new KnowledgeBuffer(10, 3);
    public KnowledgeBuffer masteryInc = new KnowledgeBuffer(10, 3);
    public KnowledgeBuffer shieldStrengthInc = new KnowledgeBuffer(10, 3);
    public KnowledgeBuffer costDecInc = new KnowledgeBuffer(4, 7);
    
    //************* 风主罐装知识 ****************
    public KnowledgeBuffer SharpAnemo = new KnowledgeBuffer(1, 22);
    public KnowledgeBuffer WhirlingAnemo = new KnowledgeBuffer(1, 12);
    public KnowledgeBuffer ProtectedAnemo = new KnowledgeBuffer(1, 8);
    public KnowledgeBuffer RechargeAnemo = new KnowledgeBuffer(1, 10);
    public KnowledgeBuffer BuffAnemo = new KnowledgeBuffer(1, 28);

    public void EnableAll()
    {
        atkInc.Enable();
        defInc.Enable();
        lifeInc.Enable();
        blockInc.Enable();
        damInc.Enable();
        rechargeInc.Enable();
        masteryInc.Enable();
        shieldStrengthInc.Enable();
        costDecInc.Enable();
        
        SharpAnemo.Enable();
        WhirlingAnemo.Enable();
        ProtectedAnemo.Enable();
        RechargeAnemo.Enable();
        BuffAnemo.Enable();
    }

    public void DisableAll()
    {
        atkInc.Disable();
        defInc.Disable();
        lifeInc.Disable();
        blockInc.Disable();
        damInc.Disable();
        rechargeInc.Disable();
        masteryInc.Disable();
        shieldStrengthInc.Disable();
        costDecInc.Disable();
        
        SharpAnemo.Disable();
        WhirlingAnemo.Disable();
        ProtectedAnemo.Disable();
        RechargeAnemo.Disable();
        BuffAnemo.Disable();
    }
}

[System.Serializable]
public class canningKnowledgeData_Strengthen
{
    //************* 通用属性 ****************
    public KnowledgeBuffer atkInc = new KnowledgeBuffer(5, 4);
    public KnowledgeBuffer defInc = new KnowledgeBuffer(5, 4);
    public KnowledgeBuffer lifeInc = new KnowledgeBuffer(5, 4);
    public KnowledgeBuffer blockInc = new KnowledgeBuffer(1, 30);
    public KnowledgeBuffer damInc = new KnowledgeBuffer(5, 6);
    public KnowledgeBuffer rechargeInc = new KnowledgeBuffer(5, 6);
    public KnowledgeBuffer masteryInc = new KnowledgeBuffer(5, 6);
    public KnowledgeBuffer shieldStrengthInc = new KnowledgeBuffer(5, 6);
    public KnowledgeBuffer costDecInc = new KnowledgeBuffer(2, 16);
    
    //************* 风主罐装知识 ****************
    public KnowledgeBuffer SuperSlashAnemo = new KnowledgeBuffer(1, 32);
    public KnowledgeBuffer PowerUpAnemo = new KnowledgeBuffer(2, 16);
    public KnowledgeBuffer ExtraDamIncAnemo = new KnowledgeBuffer(1, 16);
    public KnowledgeBuffer SpInitAnemo = new KnowledgeBuffer(1, 18);
    public KnowledgeBuffer LongDurationAnemo = new KnowledgeBuffer(1, 22);
    public KnowledgeBuffer HealAnemo = new KnowledgeBuffer(1, 15);
    public KnowledgeBuffer GreenAnemo = new KnowledgeBuffer(1, 48);
    
    
    
    public void EnableAll()
    {
        atkInc.Enable();
        defInc.Enable();
        lifeInc.Enable();
        blockInc.Enable();
        damInc.Enable();
        rechargeInc.Enable();
        masteryInc.Enable();
        shieldStrengthInc.Enable();
        costDecInc.Enable();
        
        SuperSlashAnemo.Enable();
        PowerUpAnemo.Enable();
        ExtraDamIncAnemo.Enable();
        SpInitAnemo.Enable();
        LongDurationAnemo.Enable();
        HealAnemo.Enable();
        GreenAnemo.Enable();
    }

    public void DisableAll()
    {
        atkInc.Disable();
        defInc.Disable();
        lifeInc.Disable();
        blockInc.Disable();
        damInc.Disable();
        rechargeInc.Disable();
        masteryInc.Disable();
        shieldStrengthInc.Disable();
        costDecInc.Disable();
        
        SuperSlashAnemo.Disable();
        PowerUpAnemo.Disable();
        ExtraDamIncAnemo.Disable();
        SpInitAnemo.Disable();
        LongDurationAnemo.Disable();
        HealAnemo.Disable();
        GreenAnemo.Disable();
    }
}

[System.Serializable]
public class KnowledgeBuffer
{
    public int total { get; private set; }  // 表示本知识在商店中总共的售卖数量
    public int price { get; private set; }  // 表示本知识的价格
    
    public int num;                         // 表示本知识当前激活的数量
    public int maxNum;                      // 表示本知识当前可激活的最大数量

    public KnowledgeBuffer(int total_, int price_)
    {
        total = total_;
        price = price_;
        num = 0;
        maxNum = 0;
    }

    public void Enable()
    {
        num = maxNum;
    }

    public void Disable()
    {
        num = 0;
    }

    public bool isEnabled()
    {
        return maxNum != 0 && num == maxNum;
    }

    public void SwitchEnable()
    {
        if (isEnabled()) Disable();
        else Enable();
    }

    public void Buy()
    {
        if (maxNum >= total) return;
        if (isEnabled() || maxNum == 0) num++;
        maxNum++;
    }
    
}