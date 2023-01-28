using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lifeLine : MonoBehaviour
{
    private Yelan yelan;
    private float maxDuration;
    private ElementTimer timer;
    private List<EnemyCore> enemyList = new List<EnemyCore>();

    private void Awake()
    {
        yelan = transform.parent.parent.GetComponent<Yelan>();
    }

    private void Start()
    {
        timer = new ElementTimer(yelan, 2f);
    }

    public void Init(Vector3 yelanPos, int pianyi, FourDirection direction, float duration)
    {
        maxDuration = duration;
        Vector3 pos = yelanPos;
        Vector3 rol = Vector3.zero;
        pos.y = 0;
        switch (direction)
        {
            case FourDirection.Right:
                pos.x += pianyi;
                rol.y = 90;
                break;
            case FourDirection.Left:
                pos.x -= pianyi;
                rol.y = 90;
                break;
            case FourDirection.UP:
                pos.z += pianyi;
                break;
            case FourDirection.Down:
                pos.z -= pianyi;
                break;
        }

        transform.position = pos;
        transform.eulerAngles = rol;
        enemyList.Clear();
        gameObject.SetActive(true);
    }

    private void Update()
    {
        maxDuration -= Time.deltaTime;
        if (maxDuration <= 0)
        {
            gameObject.SetActive(false);
            return;
        }

        foreach (var ec in enemyList)
        {
            if (timer.AttachElement(ec))
            {
                yelan.LifeLineAtk(ec);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("enemy")) return;
        EnemyCore ec = other.GetComponent<EnemyCore>();
        enemyList.Add(ec);
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("enemy")) return;
        EnemyCore ec = other.GetComponent<EnemyCore>();
        enemyList.Remove(ec);
    }
}
