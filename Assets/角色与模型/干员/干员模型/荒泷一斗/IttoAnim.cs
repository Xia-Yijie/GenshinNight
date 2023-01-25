using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IttoAnim : MonoBehaviour
{
    private Itto itto;
    private Animator anim;
    
    private void Awake()
    {
        itto = transform.parent.GetComponent<Itto>();
        anim = GetComponent<Animator>();
    }
    
    public void spAttackStart()
    {
        itto.atkSpeedController.ChangeBaseInterval(3f);
    }
    
    public void skill3_spAttackStart()
    {
        itto.atkSpeedController.ChangeBaseInterval(2.67f);
    }

    public void spAttackEnd()
    {
        anim.SetBool("sp", false);
        itto.atkSpeedController.ChangeBaseInterval(itto.maxAtkInterval);
    }
    
    public void Skill3_spAttack()
    {
        itto.Skill3_spAttack();
    }

    public void SkillAtk_3_GhostFront()
    {
        itto.SkillAtk_3_GhostFront();
    }
    
    public void SkillAtk_3_GhostBack()
    {
        itto.SkillAtk_3_GhostBack();
    }

    public void Skill3_End()
    {
        // itto.maxAtkInterval = itto.od_.maxAtkInterval;
    }

    public void PlayNorAtk()
    {
        itto.norAtkAudio.Play();
    }

    public void PlayspAtk2()
    {
        itto.spAtk2Audio.Play();
    }

    public void PlayspAtkDown()
    {
        itto.spAtk3Audio.Play();
    }

    public void PlaySkill3NorAtk()
    {
        itto.norAtkAudio.Play();
    }

}
