using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    public float timeDelta = 0.1f;
    private bool timeSlow = false;
    
    void Update()
    {

        if (timeSlow)
        {
            Time.timeScale = timeDelta;
        }
        else
        {
            Time.timeScale = 1;
        }
        
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (timeSlow)
            {
                Time.timeScale = 1;
                timeSlow = false;
            }
            else
            {
                Time.timeScale = timeDelta;
                timeSlow = true;
            }
        }
    }
}
