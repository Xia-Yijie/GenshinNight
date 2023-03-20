using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DehyaField : MonoBehaviour
{
    public GameObject fieldAnim;
    private Dictionary<BattleCore, int> CountList = new Dictionary<BattleCore, int>();
    private Dehya dehya;


    public void Clear()
    {
        CountList.Clear();
    }
    
    void Awake()
    {
        dehya = transform.parent.GetComponent<Dehya>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("operator"))
        {
            OperatorCore oc = other.GetComponent<OperatorCore>();
            if (oc.od_.Name == "迪希雅") return;
            if (CountList.ContainsKey(oc))
                CountList[oc]++;
            
            else
            {
                CountList.Add(oc, 1);
                dehya.FieldOperList.Add(oc);
                dehya.OperIntoField(oc);
                oc.DieAction += DelBattleCore_Oper;
            }
        }
        else if (other.CompareTag("enemy"))
        {
            EnemyCore ec = other.GetComponent<EnemyCore>();
            if (!dehya.canAtkDrone && ec.isDrone) return;     // 如果不能打无人机
            if (CountList.ContainsKey(ec))
                CountList[ec]++;
            
            else
            {
                CountList.Add(ec, 1);
                dehya.FieldEnemyList.Add(ec);
                dehya.EnemyIntoField(ec);
                ec.DieAction += DelBattleCore_Enemy;
            }
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
            OperatorCore oc = other.GetComponent<OperatorCore>();
            if (oc.od_.Name == "迪希雅") return;
            if (!CountList.ContainsKey(oc)) return;
            CountList[oc]--;
            if (CountList[oc] > 0) return;
            CountList.Remove(oc);
            dehya.FieldOperList.Remove(oc);
            dehya.OperOutField(oc);
            oc.DieAction -= DelBattleCore_Oper;
        }
        else if (other.CompareTag("enemy"))
        {
            EnemyCore ec = other.GetComponent<EnemyCore>();
            if (!dehya.canAtkDrone && ec.isDrone) return;     // 如果不能打无人机
            if (!CountList.ContainsKey(ec)) return;
            CountList[ec]--;
            if (CountList[ec] > 0) return;
            CountList.Remove(ec);
            dehya.FieldEnemyList.Remove(ec);
            dehya.EnemyOutField(ec);
            ec.DieAction -= DelBattleCore_Enemy;
        }
    }


    private void DelBattleCore_Oper(BattleCore dying_bc)
    {
        // 用于死亡的回调函数，在List里删掉要死的这个BattleCore
        OperatorCore oc = (OperatorCore) dying_bc;
        dehya.FieldOperList.Remove(oc);
        dehya.OperOutField(oc);
        CountList.Remove(dying_bc);
    }
    
    private void DelBattleCore_Enemy(BattleCore dying_bc)
    {
        EnemyCore ec = (EnemyCore) dying_bc;
        dehya.FieldEnemyList.Remove(ec);
        dehya.EnemyOutField(ec);
        CountList.Remove(dying_bc);
    }

    public void DehyaDie(BattleCore bc)
    {// 迪希雅死亡时调用，清空所有单位的委托
        foreach (var oc in dehya.FieldOperList)
        {
            dehya.OperOutField(oc);
        }
        foreach (var ec in dehya.FieldEnemyList)
        {
            dehya.EnemyOutField(ec);
        }
        
        dehya.FieldOperList.Clear();
        dehya.FieldEnemyList.Clear();
        CountList.Clear();
    }

    public void OpenField()
    {
        fieldAnim.SetActive(true);
        gameObject.SetActive(true);
    }

    public void CloseField()
    {// 关闭净焰剑狱
        DehyaDie(null);
        fieldAnim.SetActive(false);
        gameObject.SetActive(false);
    }
    
}
