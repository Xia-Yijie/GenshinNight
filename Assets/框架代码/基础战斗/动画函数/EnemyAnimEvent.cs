using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimEvent : MonoBehaviour
{
    private EnemyCore ec_;
    
    private void Awake()
    {
        ec_ = transform.parent.GetComponent<EnemyCore>();
    }
    
    
    public void OnAttack()
    {
        ec_.OnAttack();
        ec_.NorAtkAction?.Invoke(ec_);
    }

    public void OnDie()
    {
        ec_.OnDie();
    }
    
    public void FightBegin()
    {
        if (ec_.tarIsNull) FightEnd();
        else ec_.NorAtkStartCool();
    }
    
    public void FightEnd()
    {
        ec_.anim.SetBool("fight", false);
    }
    
}
