using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SAG_EnemySpecial : MonoBehaviour
{
    // 专门给EnemyCore里的es队列抓取目标的类
    private EnemyCore _ec;

    public Func<OperatorCore, bool> operCheck;
    public Func<EnemyCore, bool> enemyCheck;
    
    void Awake()
    {
        if (transform.parent != null)
        {
            _ec = transform.parent.GetComponent<EnemyCore>();
            _ec.SAG = this;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("operator"))
        {
            OperatorCore OC = other.GetComponent<OperatorCore>();
            if (operCheck != null && !operCheck(OC)) return;
            
            _ec.operatorList_es.Add(OC);
            OC.DieAction += DelBattleCore_Oper;
        }
        else if (other.CompareTag("enemy"))
        {
            EnemyCore EC = other.GetComponent<EnemyCore>();
            if (enemyCheck != null && !enemyCheck(EC)) return;
            
            _ec.enemyList_es.Add(EC);
            EC.DieAction += DelBattleCore_Enemy;
        }
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
        if (other.CompareTag("operator"))
        {
            OperatorCore OC = other.GetComponent<OperatorCore>();
            if (!_ec.operatorList_es.Contains(OC)) return;

            _ec.operatorList_es.Remove(OC);
            OC.DieAction -= DelBattleCore_Oper;
        }
        else if (other.CompareTag("enemy"))
        {
            EnemyCore EC = other.GetComponent<EnemyCore>();
            if (!_ec.enemyList_es.Contains(EC)) return;

            _ec.enemyList_es.Remove(EC);
            EC.DieAction -= DelBattleCore_Enemy;
        }
    }


    private void DelBattleCore_Oper(BattleCore dying_bc)
    {
        // 用于死亡的回调函数，在List里删掉要死的这个BattleCore
        _ec.operatorList_es.Remove((OperatorCore) dying_bc);
    }
    
    private void DelBattleCore_Enemy(BattleCore dying_bc)
    {
        _ec.enemyList_es.Remove((EnemyCore) dying_bc);
    }
}
