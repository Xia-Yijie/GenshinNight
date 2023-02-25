using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor.MPE;
using UnityEngine;

public class LumineTornado : MonoBehaviour
{
    public ParticleSystem ps1;
    public ParticleSystem ps2;
    public ParticleSystem ps3; 
    public ParticleSystem ps4;
    
    private Lumine_Anemo lumine;
    private List<EnemyCore> innerList = new List<EnemyCore>();
    private ElementTimer timer;
    [HideInInspector] public ElementType willType;
    [HideInInspector] public ElementType type;
    private float typeChangeDelay;
    private float delay;
    private float remainTime;
    private float speed = 1f;

    private void Awake()
    {
        lumine = transform.parent.GetComponent<Lumine_Anemo>();
        timer = new ElementTimer(lumine, 0.8f);
    }


    public void Init()
    {
        gameObject.SetActive(true);
        
        remainTime = lumine.GetSkill3_Duration();
        delay = 0.9f;
        transform.localPosition = Vector3.zero;
        innerList.Clear();
        timer.Clear();

        transform.eulerAngles = lumine.direction switch
        {
            FourDirection.Right => new Vector3(0, -90, 0),
            FourDirection.Left => new Vector3(0, 90, 0),
            FourDirection.UP => new Vector3(0, 180, 0),
            _ => new Vector3(0, 0, 0)
        };
        
        PreChangeType(ElementType.Anemo);
        ChangeType();
    }

    private void Update()
    {
        if (delay > 0)
        {
            delay -= Time.deltaTime;
            return;
        }
        
        remainTime -= Time.deltaTime;
        if (remainTime <= 0)
        {
            innerList.Clear();
            timer.Clear();
            gameObject.SetActive(false);
            return;
        }
        transform.Translate(Vector3.back * (speed * Time.deltaTime));

        foreach (var tarEC in innerList)
        {
            if (!timer.AttachElement(tarEC)) continue;
            lumine.Skill3_Damage(tarEC);
            
        }
        
    }

    public void PreChangeType(ElementType t)
    {
        if (t != ElementType.Anemo && t != ElementType.Pyro && t != ElementType.Electro &&
            t != ElementType.Hydro && t != ElementType.Cryo) return;
        willType = t;
        switch (t)
        {
            case ElementType.Anemo:
            default:
                ChangePSColor(ps1, new Color32(121, 255, 200, 45), new Color32(174, 255, 241, 45));
                ChangePSColor(ps2, new Color32(0, 255, 218, 50), new Color32(165, 255, 233, 50));
                ChangePSColor(ps3, new Color32(0, 255, 216, 150), new Color32(172, 255, 255, 150));
                ChangePSColor(ps4, new Color32(0, 255, 231, 150), new Color32(162, 255, 236, 150));
                break;
            case ElementType.Pyro:
                ChangePSColor(ps1, new Color32(255, 0, 0, 45), new Color32(255, 195, 0, 45));
                ChangePSColor(ps2, new Color32(255, 0, 0, 50), new Color32(255, 195, 0, 50));
                ChangePSColor(ps3, new Color32(255, 0, 0, 150), new Color32(255, 195, 0, 150));
                ChangePSColor(ps4, new Color32(255, 0, 0, 150), new Color32(255, 195, 0, 150));
                break;
            case ElementType.Electro:
                ChangePSColor(ps1, new Color32(75, 0, 255, 45), new Color32(166, 49, 204, 45));
                ChangePSColor(ps2, new Color32(75, 0, 255, 50), new Color32(166, 49, 204, 50));
                ChangePSColor(ps3, new Color32(75, 0, 255, 150), new Color32(166, 49, 204, 150));
                ChangePSColor(ps4, new Color32(75, 0, 255, 150), new Color32(166, 49, 204, 150));
                break;
            case ElementType.Hydro:
                ChangePSColor(ps1, new Color32(0, 55, 255, 255), new Color32(25, 110, 222, 45));
                ChangePSColor(ps2, new Color32(0, 55, 255, 50), new Color32(25, 110, 222, 50));
                ChangePSColor(ps3, new Color32(0, 55, 255, 150), new Color32(25, 110, 222, 150));
                ChangePSColor(ps4, new Color32(0, 55, 255, 150), new Color32(25, 110, 222, 150));
                break;
            case ElementType.Cryo:
                ChangePSColor(ps1, new Color32(255, 255, 255, 45), new Color32(75, 216, 255, 45));
                ChangePSColor(ps2, new Color32(255, 255, 255, 50), new Color32(75, 216, 255, 50));
                ChangePSColor(ps3, new Color32(255, 255, 255, 150), new Color32(75, 216, 255, 150));
                ChangePSColor(ps4, new Color32(255, 255, 255, 150), new Color32(75, 216, 255, 150));
                break;
        }

        Invoke(nameof(ChangeType), 0.5f);
    }

    private void ChangeType()
    {
        type = willType;
    }

    private void ChangePSColor(ParticleSystem ps, Color32 color1, Color32 color2)
    {
        var mm = ps.main;
        ParticleSystem.MinMaxGradient c = new ParticleSystem.MinMaxGradient();
        c.colorMin = color1;
        c.colorMax = color2;
        c.mode = ParticleSystemGradientMode.TwoColors;
        mm.startColor = c;
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("enemy")) return;
        EnemyCore ec = other.GetComponent<EnemyCore>();
        innerList.Add(ec);
        ec.DieAction += DelEnemy;

        bool dizzy = ec.ei_.mass < 2;
        ec.ppc_.ContinueAbsorb(transform, 20,
            dizzy, 0.7f, EC => !innerList.Contains(EC));
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("enemy")) return;
        EnemyCore ec = other.GetComponent<EnemyCore>();
        innerList.Remove(ec);
        ec.DieAction -= DelEnemy;
    }

    private void DelEnemy(BattleCore bc)
    {
        innerList.Remove((EnemyCore) bc);
    }
}
