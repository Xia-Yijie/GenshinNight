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
        "攻击提升",
        "防御提升",
        "生命提升",
        "阻挡数提升",
        "元素伤害提升",
        "充能效率提升",
        "元素精通提升",
        "护盾强效提升",
        "部署费用下降",
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

    private int[] universalKnowledgePrice = {1, 2, 3, 4, 5, 6, 7, 8, 90};

    private Action[] universalKnowledgeActions =
    {
        canningKnowledgeFunc_Nor.AtkInc,
        canningKnowledgeFunc_Nor.DefInc,
        canningKnowledgeFunc_Nor.LifeInc,
        canningKnowledgeFunc_Nor.BlockInc,
        canningKnowledgeFunc_Nor.DamInc,
        canningKnowledgeFunc_Nor.RechargeInc,
        canningKnowledgeFunc_Nor.MasteryInc,
        canningKnowledgeFunc_Nor.ShieldStrengthInc,
        canningKnowledgeFunc_Nor.CostDec,
    };
    
    private Action[] universalKnowledgeSettingActions =
    {
        canningKnowledgeFunc_Nor.AtkSetting,
        canningKnowledgeFunc_Nor.DefSetting,
        canningKnowledgeFunc_Nor.LifeSetting,
        canningKnowledgeFunc_Nor.BlockSetting,
        canningKnowledgeFunc_Nor.DamSetting,
        canningKnowledgeFunc_Nor.RechargeSetting,
        canningKnowledgeFunc_Nor.MasterySetting,
        canningKnowledgeFunc_Nor.ShieldStrengthSetting,
        canningKnowledgeFunc_Nor.CostSetting,
    };

    private string Dori_UniversalKnowledge = "这里都是各个领域的基础知识，" +
                                             "使用后可以强化基础属性，对所有形态的旅行者都有效！";

    

    public override void GenerateUniversalKnowledge(bool isShop)
    {
        canningKnowledgeData data = gameManager.knowledgeData;
        int[] maxNum =
        {
            data.maxAtkIncNum, data.maxDefIncNum, data.maxLifeIncNum, data.maxBlockIncNum,
            data.maxDamIncNum, data.maxRechargeIncNum, data.maxMasteryIncNum,
            data.maxShieldStrengthIncNum, data.maxCostDecNum,
        };
        int[] hadNum =
        {
            data.atkIncNum, data.defIncNum, data.lifeIncNum, data.blockIncNum,
            data.damIncNum, data.rechargeIncNum, data.masteryIncNum,
            data.shieldStrengthIncNum, data.costDecNum,
        };
        int[] totalNum =
        {
            canningKnowledgeData.atkIncTotal, canningKnowledgeData.defIncTotal, canningKnowledgeData.lifeIncTotal,
            canningKnowledgeData.blockIncTotal, canningKnowledgeData.damIncTotal, canningKnowledgeData.rechargeIncTotal,
            canningKnowledgeData.masteryIncTotal, canningKnowledgeData.shieldStrengthIncTotal,
            canningKnowledgeData.costDecTotal,
        };

        if (isShop)
        {
            ckui.ClearSlots();
            for (int i = 0; i < 9; i++)
            {
                ckui.GetSlot(universalCanSprite, universalKnowledgeTitle[i],
                    universalKnowledgeDescription[i], universalKnowledgePrice[i],
                    universalKnowledgeActions[i], totalNum[i] - maxNum[i], totalNum[i]);
            }
            ShopUIController.ShowText(Dori_UniversalKnowledge);
        }
        else
        {
            cksui.ClearSlots();
            for (int i = 0; i < 9; i++)
            {
                cksui.GetSlot(universalCanSprite, universalKnowledgeTitle[i],
                    universalKnowledgeDescription[i], universalKnowledgeSettingActions[i],
                    hadNum[i], maxNum[i], hadNum[i] == maxNum[i]);
            }
        }
        
    }
    
    // 强化模式
    public Sprite universalCanSprite_S;
    
    private string[] universalKnowledgeTitle_S =
    {
        "攻击提升_S",
        "防御提升_S",
        "生命提升_S",
        "阻挡数提升_S",
        "元素伤害提升_S",
        "充能效率提升_S",
        "元素精通提升_S",
        "护盾强效提升_S",
        "部署费用下降_S",
    };

    private string[] universalKnowledgeDescription_S =
    {
        "攻击力+2%",
        "防御力+2%",
        "生命值+2%",
        "阻挡数+2",
        "元素伤害+2%",
        "充能效率+2%",
        "元素精通+2",
        "护盾强效+2%",
        "部署费用-2",
    };

    private int[] universalKnowledgePrice_S = {1, 2, 3, 4, 5, 6, 7, 8, 90};

    private Action[] universalKnowledgeActions_S =
    {
        canningKnowledgeFunc_Nor.AtkInc_S,
        canningKnowledgeFunc_Nor.DefInc_S,
        canningKnowledgeFunc_Nor.LifeInc_S,
        canningKnowledgeFunc_Nor.BlockInc_S,
        canningKnowledgeFunc_Nor.DamInc_S,
        canningKnowledgeFunc_Nor.RechargeInc_S,
        canningKnowledgeFunc_Nor.MasteryInc_S,
        canningKnowledgeFunc_Nor.ShieldStrengthInc_S,
        canningKnowledgeFunc_Nor.CostDec_S,
    };
    
    private Action[] universalKnowledgeSettingActions_S =
    {
        canningKnowledgeFunc_Nor.AtkSetting_S,
        canningKnowledgeFunc_Nor.DefSetting_S,
        canningKnowledgeFunc_Nor.LifeSetting_S,
        canningKnowledgeFunc_Nor.BlockSetting_S,
        canningKnowledgeFunc_Nor.DamSetting_S,
        canningKnowledgeFunc_Nor.RechargeSetting_S,
        canningKnowledgeFunc_Nor.MasterySetting_S,
        canningKnowledgeFunc_Nor.ShieldStrengthSetting_S,
        canningKnowledgeFunc_Nor.CostSetting_S,
    };

    private string Dori_UniversalKnowledge_S = "强化罐装知识，嘿嘿，它们是教令院都买不到的好东西。" +
                                               "不过需要旅行者达到精英化2级后才有效！";

    public override void GenerateUniversalKnowledge_S(bool isShop)
    {
        canningKnowledgeData_Strengthen data = gameManager.knowledgeDataStrengthen;
        int[] maxNum =
        {
            data.maxAtkIncNum, data.maxDefIncNum, data.maxLifeIncNum, data.maxBlockIncNum,
            data.maxDamIncNum, data.maxRechargeIncNum, data.maxMasteryIncNum,
            data.maxShieldStrengthIncNum, data.maxCostDecNum,
        };
        int[] hadNum =
        {
            data.atkIncNum, data.defIncNum, data.lifeIncNum, data.blockIncNum,
            data.damIncNum, data.rechargeIncNum, data.masteryIncNum,
            data.shieldStrengthIncNum, data.costDecNum,
        };
        int[] totalNum =
        {
            canningKnowledgeData_Strengthen.atkIncTotal, canningKnowledgeData_Strengthen.defIncTotal, canningKnowledgeData_Strengthen.lifeIncTotal,
            canningKnowledgeData_Strengthen.blockIncTotal, canningKnowledgeData_Strengthen.damIncTotal, canningKnowledgeData_Strengthen.rechargeIncTotal,
            canningKnowledgeData_Strengthen.masteryIncTotal, canningKnowledgeData_Strengthen.shieldStrengthIncTotal,
            canningKnowledgeData_Strengthen.costDecTotal,
        };

        if (isShop)
        {
            ckui.ClearSlots();
            for (int i = 0; i < 9; i++)
            {
                ckui.GetSlot(universalCanSprite_S, universalKnowledgeTitle_S[i], 
                    universalKnowledgeDescription_S[i], universalKnowledgePrice_S[i], 
                    universalKnowledgeActions_S[i], totalNum[i] - maxNum[i], totalNum[i]);
            }
            ShopUIController.ShowText(Dori_UniversalKnowledge_S);
        }
        else
        {
            cksui.ClearSlots();
            for (int i = 0; i < 9; i++)
            {
                cksui.GetSlot(universalCanSprite_S, universalKnowledgeTitle_S[i],
                    universalKnowledgeDescription_S[i], universalKnowledgeSettingActions_S[i],
                    hadNum[i], maxNum[i], hadNum[i] == maxNum[i]);
            }
        }
        
        
    }

}
