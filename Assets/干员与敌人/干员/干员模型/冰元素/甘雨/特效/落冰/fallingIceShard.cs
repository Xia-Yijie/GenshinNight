using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fallingIceShard : MonoBehaviour
{
    private Ganyu gy_;
    
    private float ActiveFalseDelay;
    private float boomDelay;
    private BattleCore tarBC;
    private bool isDie;

    private void Awake()
    {
        gy_ = transform.parent.parent.GetComponent<Ganyu>();
    }


    public void Init(BattleCore tar, bool noTar)
    {
        gameObject.SetActive(true);
        ActiveFalseDelay = 1f;
        boomDelay = 0.21f;
        
        tarBC = tar;
        isDie = noTar;
        if (!noTar) tar.DieAction += TarDie;
    }
    
    private void Update()
    {
        ActiveFalseDelay -= Time.deltaTime;
        if (ActiveFalseDelay <= 0)
        {
            gameObject.SetActive(false);
            if(!isDie) tarBC.DieAction -= TarDie;
        }

        
        if (boomDelay > 0)
        {
            if (!isDie)
            {
                transform.position = tarBC.transform.position;
            }

            if (boomDelay - Time.deltaTime <= 0)
            {// 造成伤害
                var tars = InitManager.GetNearByEnemy(
                    transform.position, gy_.iceShardRadius);
                float dam = gy_.atk_.val * gy_.skill3_Multi[gy_.skillLevel[2]];
                ElementSlot cryoSlot = new ElementSlot(ElementType.Cryo, 1f);
                foreach (var EC in tars)
                {
                    gy_.Battle(EC, dam, DamageMode.Physical, cryoSlot,
                        gy_.defaultElementTimer, true);
                }
            }
        }
        
        boomDelay -= Time.deltaTime;
    }


    private void TarDie(BattleCore bbc)
    {
        isDie = true;
    }
    
    
}
