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
        tartaglia.norAtkAudio.Play();
    }

    public void eAtkAudio()
    {
        tartaglia.eAtkAudio.Play();
    }

    public void hydroBoomAudio()
    {
        tartaglia.hydroBoomAudio.Play();
    }
    
}
