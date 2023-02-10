using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class canningKnowledgeFunc_Nor
{
    public static void AtkInc()
    {
        if (gameManager.knowledgeData.atkIncNum == gameManager.knowledgeData.maxAtkIncNum)
            gameManager.knowledgeData.atkIncNum++;
        gameManager.knowledgeData.maxAtkIncNum++;
    }

    public static void AtkInc_S()
    {
        if (gameManager.knowledgeDataStrengthen.atkIncNum == gameManager.knowledgeDataStrengthen.maxAtkIncNum)
            gameManager.knowledgeDataStrengthen.atkIncNum++;
        gameManager.knowledgeDataStrengthen.maxAtkIncNum++;
    }
    
    public static void DefInc()
    {
        if (gameManager.knowledgeData.defIncNum == gameManager.knowledgeData.maxDefIncNum)
            gameManager.knowledgeData.defIncNum++;
        gameManager.knowledgeData.maxDefIncNum++;
    }

    public static void DefInc_S()
    {
        if (gameManager.knowledgeDataStrengthen.defIncNum == gameManager.knowledgeDataStrengthen.maxDefIncNum)
            gameManager.knowledgeDataStrengthen.defIncNum++;
        gameManager.knowledgeDataStrengthen.maxDefIncNum++;
    }
    
    public static void LifeInc()
    {
        if (gameManager.knowledgeData.lifeIncNum == gameManager.knowledgeData.maxLifeIncNum)
            gameManager.knowledgeData.lifeIncNum++;
        gameManager.knowledgeData.maxLifeIncNum++;
    }

    public static void LifeInc_S()
    {
        if (gameManager.knowledgeDataStrengthen.lifeIncNum == gameManager.knowledgeDataStrengthen.maxLifeIncNum)
            gameManager.knowledgeDataStrengthen.lifeIncNum++;
        gameManager.knowledgeDataStrengthen.maxLifeIncNum++;
    }
    
    public static void BlockInc()
    {
        if (gameManager.knowledgeData.blockIncNum == gameManager.knowledgeData.maxBlockIncNum)
            gameManager.knowledgeData.blockIncNum++;
        gameManager.knowledgeData.maxBlockIncNum++;
    }

    public static void BlockInc_S()
    {
        if (gameManager.knowledgeDataStrengthen.blockIncNum == gameManager.knowledgeDataStrengthen.maxBlockIncNum)
            gameManager.knowledgeDataStrengthen.blockIncNum++;
        gameManager.knowledgeDataStrengthen.maxBlockIncNum++;
    }
    
    public static void DamInc()
    {
        if (gameManager.knowledgeData.damIncNum == gameManager.knowledgeData.maxDamIncNum)
            gameManager.knowledgeData.damIncNum++;
        gameManager.knowledgeData.maxDamIncNum++;
    }

    public static void DamInc_S()
    {
        if (gameManager.knowledgeDataStrengthen.damIncNum == gameManager.knowledgeDataStrengthen.maxDamIncNum)
            gameManager.knowledgeDataStrengthen.damIncNum++;
        gameManager.knowledgeDataStrengthen.maxDamIncNum++;
    }
    
    public static void RechargeInc()
    {
        if (gameManager.knowledgeData.rechargeIncNum == gameManager.knowledgeData.maxRechargeIncNum)
            gameManager.knowledgeData.rechargeIncNum++;
        gameManager.knowledgeData.maxRechargeIncNum++;
    }

    public static void RechargeInc_S()
    {
        if (gameManager.knowledgeDataStrengthen.rechargeIncNum == gameManager.knowledgeDataStrengthen.maxRechargeIncNum)
            gameManager.knowledgeDataStrengthen.rechargeIncNum++;
        gameManager.knowledgeDataStrengthen.maxRechargeIncNum++;
    }
    
    public static void MasteryInc()
    {
        if (gameManager.knowledgeData.masteryIncNum == gameManager.knowledgeData.maxMasteryIncNum)
            gameManager.knowledgeData.masteryIncNum++;
        gameManager.knowledgeData.maxMasteryIncNum++;
    }

    public static void MasteryInc_S()
    {
        if (gameManager.knowledgeDataStrengthen.masteryIncNum == gameManager.knowledgeDataStrengthen.maxMasteryIncNum)
            gameManager.knowledgeDataStrengthen.masteryIncNum++;
        gameManager.knowledgeDataStrengthen.maxMasteryIncNum++;
    }
    
    public static void ShieldStrengthInc()
    {
        if (gameManager.knowledgeData.shieldStrengthIncNum == gameManager.knowledgeData.maxShieldStrengthIncNum)
            gameManager.knowledgeData.shieldStrengthIncNum++;
        gameManager.knowledgeData.maxShieldStrengthIncNum++;
    }

    public static void ShieldStrengthInc_S()
    {
        if (gameManager.knowledgeDataStrengthen.shieldStrengthIncNum == gameManager.knowledgeDataStrengthen.maxShieldStrengthIncNum)
            gameManager.knowledgeDataStrengthen.shieldStrengthIncNum++;
        gameManager.knowledgeDataStrengthen.maxShieldStrengthIncNum++;
    }
    
    public static void CostDec()
    {
        if (gameManager.knowledgeData.costDecNum == gameManager.knowledgeData.maxCostDecNum)
            gameManager.knowledgeData.costDecNum++;
        gameManager.knowledgeData.maxCostDecNum++;
    }

    public static void CostDec_S()
    {
        if (gameManager.knowledgeDataStrengthen.costDecNum == gameManager.knowledgeDataStrengthen.maxCostDecNum)
            gameManager.knowledgeDataStrengthen.costDecNum++;
        gameManager.knowledgeDataStrengthen.maxCostDecNum++;
    }
    
    
    //******************************** 激活函数 ************************************
    //******************************** 激活函数 ************************************
    //******************************** 激活函数 ************************************
    
    
    public static void AtkSetting()
    {
        gameManager.knowledgeData.atkIncNum =
            gameManager.knowledgeData.atkIncNum == gameManager.knowledgeData.maxAtkIncNum
                ? 0
                : gameManager.knowledgeData.maxAtkIncNum;
    }

    public static void AtkSetting_S()
    {
        gameManager.knowledgeDataStrengthen.atkIncNum =
            gameManager.knowledgeDataStrengthen.atkIncNum == gameManager.knowledgeDataStrengthen.maxAtkIncNum
                ? 0
                : gameManager.knowledgeDataStrengthen.maxAtkIncNum;
    }
    
    public static void DefSetting()
    {
        gameManager.knowledgeData.defIncNum =
            gameManager.knowledgeData.defIncNum == gameManager.knowledgeData.maxDefIncNum
                ? 0
                : gameManager.knowledgeData.maxDefIncNum;
    }

    public static void DefSetting_S()
    {
        gameManager.knowledgeDataStrengthen.defIncNum =
            gameManager.knowledgeDataStrengthen.defIncNum == gameManager.knowledgeDataStrengthen.maxDefIncNum
                ? 0
                : gameManager.knowledgeDataStrengthen.maxDefIncNum;
    }
    
    public static void LifeSetting()
    {
        gameManager.knowledgeData.lifeIncNum =
            gameManager.knowledgeData.lifeIncNum == gameManager.knowledgeData.maxLifeIncNum
                ? 0
                : gameManager.knowledgeData.maxLifeIncNum;
    }

    public static void LifeSetting_S()
    {
        gameManager.knowledgeDataStrengthen.lifeIncNum =
            gameManager.knowledgeDataStrengthen.lifeIncNum == gameManager.knowledgeDataStrengthen.maxLifeIncNum
                ? 0
                : gameManager.knowledgeDataStrengthen.maxLifeIncNum;
    }
    
    public static void BlockSetting()
    {
        gameManager.knowledgeData.blockIncNum =
            gameManager.knowledgeData.blockIncNum == gameManager.knowledgeData.maxBlockIncNum
                ? 0
                : gameManager.knowledgeData.maxBlockIncNum;
    }

    public static void BlockSetting_S()
    {
        gameManager.knowledgeDataStrengthen.blockIncNum =
            gameManager.knowledgeDataStrengthen.blockIncNum == gameManager.knowledgeDataStrengthen.maxBlockIncNum
                ? 0
                : gameManager.knowledgeDataStrengthen.maxBlockIncNum;
    }
    
    public static void DamSetting()
    {
        gameManager.knowledgeData.damIncNum =
            gameManager.knowledgeData.damIncNum == gameManager.knowledgeData.maxDamIncNum
                ? 0
                : gameManager.knowledgeData.maxDamIncNum;
    }

    public static void DamSetting_S()
    {
        gameManager.knowledgeDataStrengthen.damIncNum =
            gameManager.knowledgeDataStrengthen.damIncNum == gameManager.knowledgeDataStrengthen.maxDamIncNum
                ? 0
                : gameManager.knowledgeDataStrengthen.maxDamIncNum;
    }
    
    public static void RechargeSetting()
    {
        gameManager.knowledgeData.rechargeIncNum =
            gameManager.knowledgeData.rechargeIncNum == gameManager.knowledgeData.maxRechargeIncNum
                ? 0
                : gameManager.knowledgeData.maxRechargeIncNum;
    }

    public static void RechargeSetting_S()
    {
        gameManager.knowledgeDataStrengthen.rechargeIncNum =
            gameManager.knowledgeDataStrengthen.rechargeIncNum == gameManager.knowledgeDataStrengthen.maxRechargeIncNum
                ? 0
                : gameManager.knowledgeDataStrengthen.maxRechargeIncNum;
    }
    
    public static void MasterySetting()
    {
        gameManager.knowledgeData.masteryIncNum =
            gameManager.knowledgeData.masteryIncNum == gameManager.knowledgeData.maxMasteryIncNum
                ? 0
                : gameManager.knowledgeData.maxMasteryIncNum;
    }

    public static void MasterySetting_S()
    {
        gameManager.knowledgeDataStrengthen.masteryIncNum =
            gameManager.knowledgeDataStrengthen.masteryIncNum == gameManager.knowledgeDataStrengthen.maxMasteryIncNum
                ? 0
                : gameManager.knowledgeDataStrengthen.maxMasteryIncNum;
    }
    
    public static void ShieldStrengthSetting()
    {
        gameManager.knowledgeData.shieldStrengthIncNum =
            gameManager.knowledgeData.shieldStrengthIncNum == gameManager.knowledgeData.maxShieldStrengthIncNum
                ? 0
                : gameManager.knowledgeData.maxShieldStrengthIncNum;
    }

    public static void ShieldStrengthSetting_S()
    {
        gameManager.knowledgeDataStrengthen.shieldStrengthIncNum =
            gameManager.knowledgeDataStrengthen.shieldStrengthIncNum == gameManager.knowledgeDataStrengthen.maxShieldStrengthIncNum
                ? 0
                : gameManager.knowledgeDataStrengthen.maxShieldStrengthIncNum;
    }
    
    public static void CostSetting()
    {
        gameManager.knowledgeData.costDecNum =
            gameManager.knowledgeData.costDecNum == gameManager.knowledgeData.maxCostDecNum
                ? 0
                : gameManager.knowledgeData.maxCostDecNum;
    }

    public static void CostSetting_S()
    {
        gameManager.knowledgeDataStrengthen.costDecNum =
            gameManager.knowledgeDataStrengthen.costDecNum == gameManager.knowledgeDataStrengthen.maxCostDecNum
                ? 0
                : gameManager.knowledgeDataStrengthen.maxCostDecNum;
    }
    
}
