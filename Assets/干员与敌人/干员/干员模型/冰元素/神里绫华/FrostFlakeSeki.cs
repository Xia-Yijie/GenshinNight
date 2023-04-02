using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class FrostFlakeSeki : MonoBehaviour
{
    private List<EnemyCore> enemyList = new List<EnemyCore>();
    private Ayaka ayaka;
    private ElementTimer timer;
    private float endTime;
    private FourDirection direction;
    private Vector3 dirVector;
    private float speed = 0.5f;

    private void Awake()
    {
        ayaka = transform.parent.GetComponent<Ayaka>();
        timer = new ElementTimer(ayaka, 2f);
    }

    public void Init(Vector3 pos, FourDirection direction_)
    {
        gameObject.SetActive(true);
        transform.position = pos;
        endTime = ayaka.Skill3_DurTime + 1f;
        ayaka.DieAction += Die;
        direction = direction_;
        dirVector = direction switch
        {
            FourDirection.Right => new Vector3(1, 0, 0),
            FourDirection.UP => new Vector3(0, 0, 1),
            FourDirection.Left => new Vector3(-1, 0, 0),
            FourDirection.Down => new Vector3(0, 0, -1),
            _ => Vector3.zero,
        };
        StartCoroutine(DamageIE());
    }

    private void Update()
    {
        endTime -= Time.deltaTime;
        if (endTime < 0)
        {
            endTime = 1e9f;
            ayaka.DieAction -= Die;
            gameObject.SetActive(false);
        }
    }


    IEnumerator DamageIE()
    {
        float t = ayaka.Skill3_DurTime;
        float coolTime = ayaka.Skill3_AtkCoolTime;

        while (t >= 0)
        {
            t -= Time.deltaTime;
            coolTime -= Time.deltaTime;
            if (coolTime <= 0)
            {
                coolTime = ayaka.Skill3_AtkCoolTime;
                foreach (var tarEC in enemyList)
                {
                    float dam = ayaka.atk_.val * ayaka.Skill3_Multi[ayaka.skillLevel[2]];
                    ElementSlot cryoSlot = new ElementSlot(ElementType.Cryo, 1f);
                    ayaka.Battle(tarEC, dam, DamageMode.Magic, cryoSlot, timer, true);

                    GameObject hit = PoolManager.GetObj(ayaka.Skill2Hit);
                    hit.transform.SetParent(tarEC.transform);
                    hit.transform.localPosition = tarEC.animTransform.localPosition + new Vector3(0, 0.05f, 0.35f);
                    DurationRecycleObj recycleObj = new DurationRecycleObj(hit, 1f, tarEC, true);
                    BuffManager.AddBuff(recycleObj);
                }

                if (enemyList.Count > 0)
                    AudioManager.PlayEFF(ayaka.qHitAudio);
            }
            
            // 向dirction的方向移动
            transform.Translate(dirVector * (speed * Time.deltaTime));

            yield return null;
        }
    }

    public void Die(BattleCore bc)
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("enemy")) return;
        EnemyCore ec_ = other.GetComponent<EnemyCore>();
        enemyList.Add(ec_);
        ec_.DieAction += EnemyDie;
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("enemy")) return;
        EnemyCore ec_ = other.GetComponent<EnemyCore>();
        enemyList.Remove(ec_);
        ec_.DieAction -= EnemyDie;
    }

    private void EnemyDie(BattleCore dying_bc)
    {
        enemyList.Remove((EnemyCore) dying_bc);
    }
}
