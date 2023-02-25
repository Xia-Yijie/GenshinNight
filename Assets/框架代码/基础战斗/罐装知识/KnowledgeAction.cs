using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnowledgeAction : MonoBehaviour
{
    public static void ActGeneral(OperatorCore oc)
    {// 应用通用罐装知识
        ValueAdd(oc.atk_, ValueBuffMode.Percentage, 0.01f * gameManager.knowledgeData.atkInc.num);
        ValueAdd(oc.def_, ValueBuffMode.Percentage, 0.01f * gameManager.knowledgeData.defInc.num);
        ValueAdd(oc.life_, ValueBuffMode.Percentage, 0.01f * gameManager.knowledgeData.lifeInc.num);
        ValueAdd(oc.maxBlock, ValueBuffMode.Fixed, 1 * gameManager.knowledgeData.blockInc.num);
        ValueAdd(oc.elementDamage, ValueBuffMode.Fixed, 0.01f * gameManager.knowledgeData.damInc.num);
        ValueAdd(oc.sp_.spRecharge, ValueBuffMode.Fixed, 0.01f * gameManager.knowledgeData.rechargeInc.num);
        ValueAdd(oc.elementMastery, ValueBuffMode.Fixed, 1 * gameManager.knowledgeData.masteryInc.num);
        ValueAdd(oc.shieldStrength, ValueBuffMode.Fixed, 0.01f * gameManager.knowledgeData.shieldStrengthInc.num);
        
    }

    public static void ActCostGeneral(OperatorCore oc)
    {
        ValueAdd(oc.costNeed, ValueBuffMode.Fixed, -1 * gameManager.knowledgeData.costDecInc.num);
    }

    public static void ActGeneral_S(OperatorCore oc)
    {// 应用通用罐装知识（强化）
        ValueAdd(oc.atk_, ValueBuffMode.Percentage, 0.02f * gameManager.knowledgeDataStrengthen.atkInc.num);
        ValueAdd(oc.def_, ValueBuffMode.Percentage, 0.02f * gameManager.knowledgeDataStrengthen.defInc.num);
        ValueAdd(oc.life_, ValueBuffMode.Percentage, 0.02f * gameManager.knowledgeDataStrengthen.lifeInc.num);
        ValueAdd(oc.maxBlock, ValueBuffMode.Fixed, 1 * gameManager.knowledgeDataStrengthen.blockInc.num);
        ValueAdd(oc.elementDamage, ValueBuffMode.Fixed, 0.02f * gameManager.knowledgeDataStrengthen.damInc.num);
        ValueAdd(oc.sp_.spRecharge, ValueBuffMode.Fixed, 0.02f * gameManager.knowledgeDataStrengthen.rechargeInc.num);
        ValueAdd(oc.elementMastery, ValueBuffMode.Fixed, 2 * gameManager.knowledgeDataStrengthen.masteryInc.num);
        ValueAdd(oc.shieldStrength, ValueBuffMode.Fixed, 0.02f * gameManager.knowledgeDataStrengthen.shieldStrengthInc.num);
    }

    public static void ActCostGeneral_S(OperatorCore oc)
    {
        ValueAdd(oc.costNeed, ValueBuffMode.Fixed, -2 * gameManager.knowledgeDataStrengthen.costDecInc.num);
    }

    
    //***************** 风主 ******************
    public static void ActAnemo(OperatorCore oc)
    {
        ValueAdd(oc.def_, ValueBuffMode.Percentage, 0.1f * gameManager.knowledgeData.ProtectedAnemo.num);
        ValueAdd(oc.magicDef_, ValueBuffMode.Fixed, 10 * gameManager.knowledgeData.ProtectedAnemo.num);
        ValueAdd(oc.sp_.spRecharge, ValueBuffMode.Fixed, 0.16f * gameManager.knowledgeData.RechargeAnemo.num);
    }
    
    
    
    
    
    
    
    

    private static void ValueAdd(ValueBuffer buffer, ValueBuffMode mode, float count)
    {
        ValueBuffInner inner = new ValueBuffInner(mode, count);
        buffer.AddValueBuff(inner);
    }
}
