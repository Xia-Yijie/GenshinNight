using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class canningKnowledgeGenerate_Nor : baseKnowledgeGenerator
{
    //********************* 通用罐装知识生成 ***************************//
    // 普通模式
    public Sprite universalCanSprite;
    
    private string[] universalKnowledgeTitle =
    {
        "单手剑战斗技巧·八",
        "不坏之金刚",
        "飞跃医院",
        "双城记",
        "清泉的猎人",
        "逃逸电子",
        "废图毁腾",
        "茂知之壳",
        "硬着陆",
    };

    private string[] universalKnowledgeDescription =
    {
        "攻击力+1%",
        "防御力+1%",
        "生命值+1%",
        "阻挡数+1",
        "元素伤害+1%",
        "充能效率+1%",
        "元素精通+1",
        "护盾强效+1%",
        "部署费用-1",
    };

    private string Dori_UniversalKnowledge = "这里都是各个领域的基础知识，" +
                                             "使用后可以强化基础属性，对所有形态的旅行者都有效！";

    

    public override void GenerateUniversalKnowledge(bool isShop)
    {
        canningKnowledgeData data = gameManager.knowledgeData;
        KnowledgeBuffer[] buffers =
        {
            data.atkInc, data.defInc, data.lifeInc, data.blockInc,
            data.damInc, data.rechargeInc, data.masteryInc,
            data.shieldStrengthInc, data.costDecInc,
        };

        if (isShop)
        {
            ckui.ClearSlots();
            for (int i = 0; i < 9; i++)
            {
                ckui.GetSlot(universalCanSprite, universalKnowledgeTitle[i],
                    universalKnowledgeDescription[i], buffers[i]);
            }
            ckui.RefreshUISlot();
            ShopUIController.ShowText(Dori_UniversalKnowledge);
        }
        else
        {
            cksui.ClearSlots();
            for (int i = 0; i < 9; i++)
            {
                cksui.GetSlot(universalCanSprite, universalKnowledgeTitle[i],
                    universalKnowledgeDescription[i], buffers[i]);
            }
            cksui.RefreshUISlot();
        }
        
    }
    
    // 强化模式
    public Sprite universalCanSprite_S;
    
    private string[] universalKnowledgeTitle_S =
    {
        "无坚不摧",
        "约等于天下无敌",
        "飞跃水疗馆",
        "三之定则",
        "破灭之时",
        "第三类永动机",
        "嬗变核素",
        "群玉临空",
        "金牌飞行执照",
    };

    private string[] universalKnowledgeDescription_S =
    {
        "攻击力+2%",
        "防御力+2%",
        "生命值+2%",
        "阻挡数+1",
        "元素伤害+2%",
        "充能效率+2%",
        "元素精通+2",
        "护盾强效+2%",
        "部署费用-2",
    };

    private string Dori_UniversalKnowledge_S = "强化罐装知识，嘿嘿，它们是教令院都买不到的好东西。" +
                                               "不过需要旅行者达到精英化2级后才有效！";

    public override void GenerateUniversalKnowledge_S(bool isShop)
    {
        canningKnowledgeData_Strengthen data = gameManager.knowledgeDataStrengthen;
        KnowledgeBuffer[] buffers =
        {
            data.atkInc, data.defInc, data.lifeInc, data.blockInc,
            data.damInc, data.rechargeInc, data.masteryInc,
            data.shieldStrengthInc, data.costDecInc,
        };

        if (isShop)
        {
            ckui.ClearSlots();
            for (int i = 0; i < 9; i++)
            {
                ckui.GetSlot(universalCanSprite_S, universalKnowledgeTitle_S[i], 
                    universalKnowledgeDescription_S[i], buffers[i]);
            }
            ckui.RefreshUISlot();
            ShopUIController.ShowText(Dori_UniversalKnowledge_S);
        }
        else
        {
            cksui.ClearSlots();
            for (int i = 0; i < 9; i++)
            {
                cksui.GetSlot(universalCanSprite_S, universalKnowledgeTitle_S[i],
                    universalKnowledgeDescription_S[i], buffers[i]);
            }
            cksui.RefreshUISlot();
        }
        
        
    }

}
