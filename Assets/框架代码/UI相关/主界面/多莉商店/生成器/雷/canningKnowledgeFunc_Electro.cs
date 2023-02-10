using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class canningKnowledgeFunc_Electro : baseKnowledgeGenerator
{
    public override void GenerateUniversalKnowledge(bool isShop)
    {
        if (isShop)
        {
            ckui.ClearSlots();
            ShopUIController.ShowText("客官还没有解锁这个属性哦，所以这个属性的罐装知识就先不给客官看了，" +
                                      "等后面解锁了属性再来找多莉吧");
        }
        else
        {
            cksui.ClearSlots();
        }
    }
        
    public override void GenerateUniversalKnowledge_S(bool isShop)
    {
        if (isShop)
        {
            ckui.ClearSlots();
            ShopUIController.ShowText("客官还没有解锁这个属性哦，所以这个属性的罐装知识就先不给客官看了，" +
                                      "等后面解锁了属性再来找多莉吧");
        }
        else
        {
            cksui.ClearSlots();
        }
    }
}
