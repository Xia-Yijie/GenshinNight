using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class canningKnowledgeFunc_Anemo : baseKnowledgeGenerator
{
    // 普通模式
    public Sprite canSprite;
    
    private string[] knowledgeTitle =
    {
        "锋利的刀风",
        "回转的怒风",
        "眷护的和风",
        "革新的旋风",
        "愿风神忽悠你",
    };

    private string[] knowledgeDescription =
    {
        "「裂空之风」产生的风刃能无视敌人25%的防御力",
        "「风涡剑」持续期间造成伤害的同时，会产生一次较小的吸力，将敌人向中心点聚集",
        "风元素旅行者的防御力+10%，法术抗性+10",
        "风元素旅行者的充能效率+16%",
        "风元素旅行者在场上时，所有干员的法术抗性+10，部署冷却时间-15%"
    };

    private string Dori_Talk1 = "这里售卖的是风元素旅行者的专属知识，" +
                                "虽然价格稍贵，但绝对值得！";
    
    
    
    public override void GenerateUniversalKnowledge(bool isShop)
    {

        canningKnowledgeData data = gameManager.knowledgeData;
        KnowledgeBuffer[] buffers =
        {
            data.SharpAnemo, data.WhirlingAnemo, data.ProtectedAnemo, data.RechargeAnemo,
            data.BuffAnemo,
        };

        if (isShop)
        {
            ckui.ClearSlots();
            for (int i = 0; i < knowledgeTitle.Length; i++)
            {
                ckui.GetSlot(canSprite, knowledgeTitle[i],
                    knowledgeDescription[i], buffers[i]);
            }
            ckui.RefreshUISlot();
            ShopUIController.ShowText(Dori_Talk1);
        }
        else
        {
            cksui.ClearSlots();
            for (int i = 0; i < knowledgeTitle.Length; i++)
            {
                cksui.GetSlot(canSprite, knowledgeTitle[i],
                    knowledgeDescription[i], buffers[i]);
            }
            cksui.RefreshUISlot();
        }
        
        
    }
        
    
    // 强化模式
    public Sprite canSprite_S;
    
    private string[] knowledgeTitle_S =
    {
        "超级派蒙旋风",
        "反弹！",
        "风带来了故事的种子…",
        "金风玉露一相逢",
        "随风而去吧",
        "复苏之风",
        "翠绿之影",
    };

    private string[] knowledgeDescription_S =
    {
        "「裂空之风」产生的风刃变成超级风刃，能对范围内的敌人造成3次伤害，但每次伤害只有原来的70%",
        "「风涡剑」最后的推力提高1级",
        "「风息激荡」的染色伤害额外提升100%",
        "装备了「风息激荡」的旅行者在部署时，会直接补满所有技力",
        "「风息激荡」产生的龙卷风持续时间延长8秒",
        "释放「风息激荡」后，场上的所有干员将获得如下效果\n·每秒回复3%生命值，持续5秒",
        "风元素旅行者在触发扩散反应时，会使目标的元素抗性-30，法术抗性-20，持续8秒",
    };
    
    
    private string Dori_Talk2 = "风元素旅行者的强化罐装知识，能极大幅度的提高输出" +
                                "以及给队友最好的辅助！";
    
    
    public override void GenerateUniversalKnowledge_S(bool isShop)
    {
        canningKnowledgeData_Strengthen data = gameManager.knowledgeDataStrengthen;
        KnowledgeBuffer[] buffers =
        {
            data.SuperSlashAnemo, data.PowerUpAnemo, data.ExtraDamIncAnemo,
            data.SpInitAnemo, data.LongDurationAnemo, data.HealAnemo, data.GreenAnemo,
        };

        if (isShop)
        {
            ckui.ClearSlots();
            for (int i = 0; i < knowledgeTitle_S.Length; i++)
            {
                ckui.GetSlot(canSprite_S, knowledgeTitle_S[i], 
                    knowledgeDescription_S[i], buffers[i]);
            }
            ckui.RefreshUISlot();
            ShopUIController.ShowText(Dori_Talk2);
        }
        else
        {
            cksui.ClearSlots();
            for (int i = 0; i < knowledgeTitle_S.Length; i++)
            {
                cksui.GetSlot(canSprite_S, knowledgeTitle_S[i],
                    knowledgeDescription_S[i], buffers[i]);
            }
            cksui.RefreshUISlot();
        }
    }
}
