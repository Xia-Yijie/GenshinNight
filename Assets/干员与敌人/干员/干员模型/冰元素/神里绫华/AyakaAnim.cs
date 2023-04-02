using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AyakaAnim : MonoBehaviour
{
    private Ayaka ayaka;

    private void Awake()
    {
        ayaka = transform.parent.GetComponent<Ayaka>();
    }


    public void OnAttackSp()
    {
        ayaka.OnAttackSp();
    }

    public void Skill2_End()
    {
        ayaka.Skill2_End();
    }

    public void PlaySkill1Audio()
    {
        AudioManager.PlayEFF(ayaka.eStartAudio, 0.6f);
    }

    public void PlaySkill3Audio()
    {
        AudioManager.PlayEFF(ayaka.qStartAudio);
    }

    public void PlayTalent1Audio()
    {
        AudioManager.PlayEFF(ayaka.talent1Audio, 0.5f);
    }

    public void PlayCutAudio()
    {
        AudioManager.PlayEFF(ayaka.cutAudio);
    }
    
}
