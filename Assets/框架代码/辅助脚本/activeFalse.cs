using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class activeFalse : MonoBehaviour
{
    private float ActiveFalseDelay;
    
    public void DESTROY()
    {
        Destroy(gameObject);
    }

    public void ActiveFalse()
    {
        gameObject.SetActive(false);
    }

    public void DelayActiveFalse(float delay)
    {
        gameObject.SetActive(true);
        ActiveFalseDelay = delay;
    }

    private void Update()
    {
        ActiveFalseDelay -= Time.deltaTime;
        if (ActiveFalseDelay <= 0) gameObject.SetActive(false);
    }
}
