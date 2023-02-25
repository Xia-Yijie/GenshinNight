using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravellerCore : OperatorCore
{
    protected override void Awake_Core()
    {
        base.Awake_Core();
        KnowledgeAction.ActCostGeneral(this);
    }

    public override void OperInit()
    {
        base.OperInit();
        KnowledgeAction.ActGeneral(this);
        if (eliteLevel >= 2) KnowledgeAction.ActGeneral_S(this);
    }
    
    public override void ElitismAction1_2()
    {
        base.ElitismAction1_2();
        KnowledgeAction.ActGeneral_S(this); // 精二的时候加上强化属性
        KnowledgeAction.ActCostGeneral_S(this);
        InitManager.dragSlotController.RefreshDragSlot();
    }
    
}
