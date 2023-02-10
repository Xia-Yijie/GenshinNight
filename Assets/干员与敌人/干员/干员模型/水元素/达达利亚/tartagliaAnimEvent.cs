using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tartagliaAnimEvent : MonoBehaviour
{
    private Tartaglia tartaglia;

    private void Awake()
    {
        tartaglia = transform.parent.GetComponent<Tartaglia>();
    }

    public void ClearAtkInterval()
    {
        tartaglia.NorAtkClear();
    }

    public void Skill2_Begin()
    {
        tartaglia.Skill2_Begin();
    }

    public void Skill2_CauseDamage()
    {
        tartaglia.Skill2CauseDamage();
    }

    public void NorAtkAudio()
    {
        AudioManager.PlayEFF(tartaglia.norAtkAudio, 0.2f);
    }

    public void eAtkAudio()
    {
        AudioManager.PlayEFF(tartaglia.eAtkAudio, 0.6f);
    }

    public void hydroBoomAudio()
    {
        AudioManager.PlayEFF(tartaglia.hydroBoomAudio, 0.6f);
    }
    
}
