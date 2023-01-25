using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JeanHealRange : MonoBehaviour
{
    private Jean jean;
    
    void Awake()
    {
        if (transform.parent != null) 
            jean = transform.parent.GetComponent<Jean>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("operator")) return;
        OperatorCore oc_ = other.GetComponent<OperatorCore>();
        jean.aroundOperList.Add(oc_);
        oc_.DieAction += DelBattleCore_Oper;
    }

    private void OnTriggerExit(Collider other)
    {
        pretendExit(other.gameObject);
    }
    
    
    /// <summary>
    /// 调用后，将模拟other物体离开该范围的事件
    /// </summary>
    public void pretendExit(GameObject other)
    {
        if (!other.CompareTag("operator")) return;
        OperatorCore oc_ = other.GetComponent<OperatorCore>();
        jean.aroundOperList.Remove(oc_);
        oc_.DieAction -= DelBattleCore_Oper;
    }


    private void DelBattleCore_Oper(BattleCore dying_bc)
    {
        // 用于死亡的回调函数，在List里删掉要死的这个BattleCore
        jean.aroundOperList.Remove(dying_bc);
    }

}
