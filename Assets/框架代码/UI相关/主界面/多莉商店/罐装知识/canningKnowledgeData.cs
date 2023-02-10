using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class canningKnowledgeData
{
    //************* 通用属性 ****************
    // 攻击提升，每个1%
    public const int atkIncTotal = 10;
    public const double atkIncRate = 0.01d;
    public int maxAtkIncNum = 0;
    public int atkIncNum = 0;
    // 防御提升，每个1%
    public const int defIncTotal = 10;
    public const double defIncRate = 0.01d;
    public int maxDefIncNum = 0;
    public int defIncNum = 0;
    // 生命提升，每个1%
    public const int lifeIncTotal = 10;
    public const double lifeIncRate = 0.01d;
    public int maxLifeIncNum = 0;
    public int lifeIncNum = 0;
    // 阻挡数提升，每个1
    public const int blockIncTotal = 0;
    public int maxBlockIncNum = 0;
    public int blockIncNum = 0;
    // 元素伤害提升，每个1%
    public const int damIncTotal = 10;
    public const double damIncRate = 0.01d;
    public int maxDamIncNum = 0;
    public int damIncNum = 0;
    // 充能效率提升，每个1%
    public const int rechargeIncTotal = 10;
    public const double rechargeIncRate = 0.01d;
    public int maxRechargeIncNum = 0;
    public int rechargeIncNum = 0;
    // 元素精通提升，每个1
    public const int masteryIncTotal = 10;
    public const int masteryIncRate = 1;
    public int maxMasteryIncNum = 0;
    public int masteryIncNum = 0;
    // 护盾强效提升，每个1%
    public const int shieldStrengthIncTotal = 10;
    public const double shieldStrengthIncRate = 0.01d;
    public int maxShieldStrengthIncNum = 0;
    public int shieldStrengthIncNum = 0;
    // 部署费用下降，每个下降1
    public const int costDecTotal = 4;
    public const int costDecRate = 1;
    public int maxCostDecNum = 0;
    public int costDecNum = 0;
    
    //************* 风主罐装知识 ****************






    public void EnableAll()
    {
        atkIncNum = maxAtkIncNum;
        defIncNum = maxDefIncNum;
        lifeIncNum = maxLifeIncNum;
        blockIncNum = maxBlockIncNum;
        damIncNum = maxDamIncNum;
        rechargeIncNum = maxRechargeIncNum;
        masteryIncNum = maxMasteryIncNum;
        shieldStrengthIncNum = maxShieldStrengthIncNum;
        costDecNum = maxCostDecNum;
    }

    public void DisableAll()
    {
        atkIncNum = 0;
        defIncNum = 0;
        lifeIncNum = 0;
        blockIncNum = 0;
        damIncNum = 0;
        rechargeIncNum = 0;
        masteryIncNum = 0;
        shieldStrengthIncNum = 0;
        costDecNum = 0;
    }
}

[System.Serializable]
public class canningKnowledgeData_Strengthen
{
    //************* 通用属性 ****************
    // 攻击提升，每个2%
    public const int atkIncTotal = 5;
    public const double atkIncRate = 0.02d;
    public int maxAtkIncNum = 0;
    public int atkIncNum = 0;
    // 防御提升，每个2%
    public const int defIncTotal = 5;
    public const double defIncRate = 0.02d;
    public int maxDefIncNum = 0;
    public int defIncNum = 0;
    // 生命提升，每个2%
    public const int lifeIncTotal = 5;
    public const double lifeIncRate = 0.02d;
    public int maxLifeIncNum = 0;
    public int lifeIncNum = 0;
    // 阻挡数提升，每个1
    public const int blockIncTotal = 1;
    public int maxBlockIncNum = 0;
    public int blockIncNum = 0;
    // 元素伤害提升，每个2%
    public const int damIncTotal = 5;
    public const double damIncRate = 0.02d;
    public int maxDamIncNum = 0;
    public int damIncNum = 0;
    // 充能效率提升，每个2%
    public const int rechargeIncTotal = 5;
    public const double rechargeIncRate = 0.02d;
    public int maxRechargeIncNum = 0;
    public int rechargeIncNum = 0;
    // 元素精通提升，每个2
    public const int masteryIncTotal = 5;
    public const int masteryIncRate = 2;
    public int maxMasteryIncNum = 0;
    public int masteryIncNum = 0;
    // 护盾强效提升，每个2%
    public const int shieldStrengthIncTotal = 5;
    public const double shieldStrengthIncRate = 0.02d;
    public int maxShieldStrengthIncNum = 0;
    public int shieldStrengthIncNum = 0;
    // 部署费用下降，每个下降2
    public const int costDecTotal = 3;
    public const int costDecRate = 2;
    public int maxCostDecNum = 0;
    public int costDecNum = 0;
    
    
    
    
    public void EnableAll()
    {
        atkIncNum = maxAtkIncNum;
        defIncNum = maxDefIncNum;
        lifeIncNum = maxLifeIncNum;
        blockIncNum = maxBlockIncNum;
        damIncNum = maxDamIncNum;
        rechargeIncNum = maxRechargeIncNum;
        masteryIncNum = maxMasteryIncNum;
        shieldStrengthIncNum = maxShieldStrengthIncNum;
        costDecNum = maxCostDecNum;
    }

    public void DisableAll()
    {
        atkIncNum = 0;
        defIncNum = 0;
        lifeIncNum = 0;
        blockIncNum = 0;
        damIncNum = 0;
        rechargeIncNum = 0;
        masteryIncNum = 0;
        shieldStrengthIncNum = 0;
        costDecNum = 0;
    }
}